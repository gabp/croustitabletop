public class DuckDnsService
{
    private string serverIp;
    private string domain;
    private string token;

    private HttpClient httpClient;

    public DuckDnsService(IConfiguration configs)
    {
        this.serverIp = configs.GetValue<string>("CroustiTabletop:DuckDns:ServerIp");
        this.domain = configs.GetValue<string>("CroustiTabletop:DuckDns:DomainName");
        this.token = configs.GetValue<string>("CroustiTabletop:DuckDns:Token");

        this.httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://www.duckdns.org")
        };
    }

    public async Task UpdateDns()
    {
        var response = await this.httpClient.GetAsync($"update?domains={this.domain}&token={this.token}&ip={this.serverIp}");

        if (!response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"{jsonResponse}\n");
        }
    }

    public async Task UpdateDns(string txtRecord)
    {
        var url = $"update?domains={this.domain}&token={this.token}&txt={txtRecord}";
        var response = await this.httpClient.GetAsync(url);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode || responseContent != "OK")
        {
            throw new Exception("Could not update dns with payload: " + url);
        }
    }

    public string GetCroustiServerUrl()
    {
        return $"https://{this.GetCroustiServerDomainName()}";
    }

    public string GetCroustiServerDomainName()
    {
        return $"{this.domain}.duckdns.org";;
    }
}