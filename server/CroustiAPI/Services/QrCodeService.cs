using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CroustiAPI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using QRCoder;

public class QrCodeService
{
    private string clientUrl;
    private string serverUrl;
    //private IHubContext<PlayerHub> hubContext;
    private TokenProviderService tokenProviderService;

    public QrCodeService(IConfiguration config, TokenProviderService tokenProviderService /*IHubContext<PlayerHub> hubContext*//*, UserManager<IdentityUser> userManager*/)
    {
        this.clientUrl = config.GetValue<string>("CroustiTabletop:ClientUrl");
        this.serverUrl = config.GetValue<string>("CroustiTabletop:ServerUrl");

        //this.hubContext = hubContext;

        this.tokenProviderService = tokenProviderService;
    }

    public byte[] GetQrCode(string playerColor)
    {
        //this.hubContext.Clients.User("blue").SendAsync("broadcastMessage", "From QrCode Service", "Wowow");

        var token = this.tokenProviderService.Generate(playerColor);
        var playersUrl = $"{this.clientUrl}/?serverUrl={this.serverUrl}&playerColor={playerColor}&access_token={token}";

        Console.WriteLine(playersUrl);

        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(playersUrl, QRCodeGenerator.ECCLevel.Q);

        QRCode qrCode = new QRCode(qrCodeData);
        Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, (Bitmap)Bitmap.FromFile("./resources/croustiquiche.png"), iconSizePercent: 35);

        using (var stream = new MemoryStream())
        {
            qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
    }
}