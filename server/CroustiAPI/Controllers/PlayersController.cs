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
    public ActionResult<Player> GetPlayer(string playerColor)
    {
        if (this.gameStateService.TryGetPlayer(playerColor, out var player))
        {
            return player;
        }

        return NotFound();
    }

    /*[HttpPost("{playerColor}/{cardId}/highlight/{on}")]
    public void ToggleHighlightCard(string playerColor, string cardId, bool on)
    {
        this.gameStateService.ToggleCardHighlight(new CardHighlight
        {
            Guid = cardId,
            Color = playerColor,
            On = on,
        });
    }*/
}
