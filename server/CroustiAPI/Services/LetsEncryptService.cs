using Certes;
using Certes.Acme;

public class LetsEncryptService
{
    private string acmeAccountFileLocation = "certs/acme-account-key.pem";
    private string certFileLocation = "certs/crousti-cert.pfx";
    private string issuersCertFileLocation = "certs/isrg-root-x2.der";
    private string acmeAccountEmail;


    private DuckDnsService duckDnsService;

    public LetsEncryptService(IConfiguration configs, DuckDnsService duckDnsService)
    {
        this.acmeAccountEmail = configs.GetValue<string>("CroustiTabletop:LetsEncrypt:AccountEmail");

        this.duckDnsService = duckDnsService;
    }

    public async Task GenerateCert()
    {
        if(!File.Exists(this.certFileLocation))
        {
            await this.Order();
        }
    }


    public async Task<AcmeContext> GetAccount()
    {
        if(File.Exists(this.acmeAccountFileLocation))
        {
            var accountPemKey = await File.ReadAllTextAsync(this.acmeAccountFileLocation);
            var accountKey = KeyFactory.FromPem(accountPemKey);

            return new AcmeContext(WellKnownServers.LetsEncryptV2, accountKey);
        }
        else
        {
            var acmeContext = new AcmeContext(WellKnownServers.LetsEncryptV2);
            var account = await acmeContext.NewAccount(this.acmeAccountEmail, true);

            // save key for next time
            var pemKey = acmeContext.AccountKey.ToPem();
            await File.WriteAllTextAsync(this.acmeAccountFileLocation, pemKey);

            return acmeContext;
        }
        
    }

    public async Task Order()
    {
        var acme = await this.GetAccount();

        // request an order
        var order = await acme.NewOrder(new[] { this.duckDnsService.GetCroustiServerDomainName() });
        var authorization = (await order.Authorizations()).First();
        var dnsChallenge = await authorization.Dns();
        var dnsTxtAnswer = acme.AccountKey.DnsTxt(dnsChallenge.Token);

        // update the txt record of the domain
        await this.duckDnsService.UpdateDns(dnsTxtAnswer);

        // now ask for validation
        var challengeState = await dnsChallenge.Validate();


        while (true)
        {
            var result = await dnsChallenge.Resource();

            if (result.Status == Certes.Acme.Resource.ChallengeStatus.Valid)
            {
                // get the issuer's cert (on https://letsencrypt.org/certificates/ get the der of ISRG Root X2 since we use es256 for the key)
                var issuersCert = await File.ReadAllBytesAsync(this.issuersCertFileLocation);

                var privateKey = KeyFactory.NewKey(KeyAlgorithm.ES256);
                var cert = await order.Generate(new CsrInfo
                {
                    CountryName = "CA",
                    State = "Quebec",
                    Locality = "Montreal",
                    Organization = "Croustiquiche Inc.",
                    OrganizationUnit = "Reasearch",
                    CommonName = this.duckDnsService.GetCroustiServerDomainName(),
                }, privateKey);

                var pfxBuilder = cert.ToPfx(privateKey);
                pfxBuilder.AddIssuer(issuersCert);
                var pfx = pfxBuilder.Build("crousti-cert", "");

                await File.WriteAllBytesAsync(this.certFileLocation, pfx);

                return;
            }
            else if (result.Status == Certes.Acme.Resource.ChallengeStatus.Invalid)
            {
                throw new Exception(result.Error.Detail);
            }

            await Task.Delay(2000);
        }        
    }

    public string GetCertLocation()
    {
        return this.certFileLocation;
    }
}