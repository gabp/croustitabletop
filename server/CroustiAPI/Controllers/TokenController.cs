using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CroustiAPI.Controllers;

[ApiController]
[LocalhostOnly]
[Route("[controller]")]
public class TokenController : ControllerBase
{
    private readonly ILogger<TokenController> _logger;
    private TokenProviderService tokenProviderService;

    public TokenController(ILogger<TokenController> logger, TokenProviderService tokenProviderService)
    {
        _logger = logger;
        this.tokenProviderService = tokenProviderService;
    }

    [HttpGet("{playerColor}")]
    public string GenerateToken(string playerColor)
    {
        return this.tokenProviderService.Generate(playerColor);
    }

    [Authorize]
    [HttpPost("test")]
    public string TestToken()
    {
        return "Valid!";
    }
}
