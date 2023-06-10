using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace CroustiAPI;

public class GameStateService
{
    private IHubContext<PlayerHub> playerHub;
    private IHubContext<TableHub> tableHub;
    private GameState gameState = new ();
    //private HashSet<CardHighlight> cardsToHighlightOnOrOff = new();

    private Players players = new ();

    public GameStateService(IHubContext<PlayerHub> playerHub, IHubContext<TableHub> tableHub)
    {
        this.playerHub = playerHub;
        this.tableHub = tableHub;
    }

    public bool TryGetPlayer(string playerColor, out Player existingPlayer)
    {
        return this.players.TryGetPlayer(playerColor, out existingPlayer);
    }

    public void UpdatePlayer(string playerColor, List<Card> cards)
    {
        var player = new Player
        {
            Color = playerColor,
        };

        player.AddCards(cards);

        this.players.PutPlayer(player);

        this.playerHub.Clients.User(playerColor.ToLower()).SendAsync(PlayerHub.PlayerHandUpdatedEvent, player.Cards.ToWebAppCardEntities());
    }

    public void ToggleCardHighlight(UnityCardHighlightEntity card)
    {
        //this.cardsToHighlightOnOrOff.Add(card);

        this.tableHub.Clients.All.SendAsync(TableHub.HighlighCardToggledEvent, card);
    }

    /*public HashSet<CardHighlight> GetCardsToHighlightOnOrOff()
    {
        var toReturn = new HashSet<CardHighlight>(cardsToHighlightOnOrOff);
        this.cardsToHighlightOnOrOff.Clear();

        return toReturn;
    }*/
}
