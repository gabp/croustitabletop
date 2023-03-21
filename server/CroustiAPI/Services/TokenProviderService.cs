using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class TokenProviderService
{
    private string audience;
    private string issuer;
    private SigningCredentials signingCredentials;
    private int expirationInHours;
    private JwtSecurityTokenHandler tokenHandler = new ();

    public TokenProviderService(IConfiguration config)
    {
        this.audience = config.GetValue<string>("CroustiTabletop:Tokens:Audience");
        this.issuer = config.GetValue<string>("CroustiTabletop:Tokens:Issuer");
        this.expirationInHours = config.GetValue<int>("CroustiTabletop:Tokens:ExpirationInHours");

        var secret = config.GetValue<string>("CroustiTabletop:Tokens:Secret");

        this.signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                SecurityAlgorithms.HmacSha512Signature);
    }

    public string Generate(string playerColor)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, playerColor) }),
            Expires = DateTime.UtcNow.AddHours(this.expirationInHours),
            Issuer = this.issuer,
            Audience = this.audience,
            SigningCredentials = this.signingCredentials,
        };

        return this.tokenHandler.CreateEncodedJwt(tokenDescriptor);
    }
}