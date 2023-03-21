using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace CroustiAPI;

public class GameStateService
{
    private IHubContext<PlayerHub> hubContext;
    private GameState gameState = new ();
    private HashSet<CardHighlight> cardsToHighlightOnOrOff = new();

    public GameStateService(IHubContext<PlayerHub> hubContext)
    {
        this.hubContext = hubContext;
    }

    public GameState GetGameState()
    {
        return this.gameState;
    }

    public void UpdateGameState(GameState newGameState)
    {
        this.gameState = newGameState;

        this.gameState.Players.ForEach(player => {
            this.hubContext.Clients.User(player.Color.ToLower()).SendAsync("PlayerHandUpdated", player.Cards);
        });
    }

    public void ToggleCardHighlight(CardHighlight card)
    {
        this.cardsToHighlightOnOrOff.Add(card);
    }

    public HashSet<CardHighlight> GetCardsToHighlightOnOrOff()
    {
        var toReturn = new HashSet<CardHighlight>(cardsToHighlightOnOrOff);
        this.cardsToHighlightOnOrOff.Clear();

        return toReturn;
    }
}
