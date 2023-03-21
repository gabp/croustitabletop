using Microsoft.AspNetCore.Mvc;

namespace CroustiAPI.Controllers;

[ApiController]
[LocalhostOnly]
[Route("[controller]")]
public class QrCodesController : ControllerBase
{
    private readonly ILogger<QrCodesController> _logger;
    private readonly QrCodeService qrCodeService;

    public QrCodesController(ILogger<QrCodesController> logger, QrCodeService qrCodeService)
    {
        _logger = logger;
        this.qrCodeService = qrCodeService;
    }

    [HttpGet("{playerColor}")]
    public IActionResult GenerateQrCode(string playerColor)
    {
        var qrCode = this.qrCodeService.GetQrCode(playerColor);

        return File(qrCode, "image/png");
    }
}
