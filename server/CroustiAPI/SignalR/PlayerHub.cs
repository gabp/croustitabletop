using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CroustiAPI;

[Authorize]
public class PlayerHub : Hub
{
    public static string HubUrl = "/player-hub";
    public static string PlayerHandUpdatedEvent = "PlayerHandUpdatedEvent";

    public void HighlightCard(GameStateService gameStateService, string cardId, bool isHighlighted)
    {
        var playerColor = Context.User.Identity.Name;

        gameStateService.ToggleCardHighlight(new UnityCardHighlightEntity {
            PlayerColor = playerColor,
            CardId = cardId,
            IsHighlighted = isHighlighted,
        });
    }
}