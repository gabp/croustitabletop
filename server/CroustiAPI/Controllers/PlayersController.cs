using Microsoft.AspNetCore.Mvc;

namespace CroustiAPI.Controllers;

[ApiController]
[LocalhostOnly]
[Route("[controller]")]
public class PlayersController : ControllerBase
{
    private readonly ILogger<PlayersController> _logger;
    private readonly GameStateService gameStateService;

    public PlayersController(ILogger<PlayersController> logger, GameStateService gameStateService)
    {
        _logger = logger;
        this.gameStateService = gameStateService;
    }

    [HttpGet("{playerColor}")]
    public ActionResult<List<Card>> GetPlayer(string playerColor)
    {
        var gameState = this.gameStateService.GetGameState();
        var targetPlayer = gameState.Players.FirstOrDefault(p => p.Color.ToLower() == playerColor.ToLower());

        return targetPlayer?.Cards ?? new ();
    }

    [HttpPost("{playerColor}/{cardId}/highlight/{on}")]
    public void ToggleHighlightCard(string playerColor, string cardId, bool on)
    {
        this.gameStateService.ToggleCardHighlight(new CardHighlight
        {
            Guid = cardId,
            Color = playerColor,
            On = on,
        });
    }
}
