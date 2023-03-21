using Microsoft.AspNetCore.Mvc;

namespace CroustiAPI.Controllers;

[ApiController]
[LocalhostOnly]
[Route("[controller]")]
public class GameStateController : ControllerBase
{
    private readonly ILogger<GameStateController> _logger;
    private readonly GameStateService gameStateService;

    public GameStateController(ILogger<GameStateController> logger, GameStateService gameStateService)
    {
        _logger = logger;
        this.gameStateService = gameStateService;
    }

    [HttpGet]
    public GameState GetGameState()
    {
        return this.gameStateService.GetGameState();
    }

    [HttpPut]
    public GameState UpdateGameState(GameState gameState)
    {        
        this.gameStateService.UpdateGameState(gameState);

        return this.gameStateService.GetGameState();
    }

    [HttpGet("cardsToHighlight")]
    public HashSet<CardHighlight> GetCardsToHighlightOnOrOff()
    {
        var cardsToHighlightOnOrOff = this.gameStateService.GetCardsToHighlightOnOrOff();

        return cardsToHighlightOnOrOff;
    }
}
