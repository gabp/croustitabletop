using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CroustiAPI;

[Authorize]
public class PlayerHub : Hub
{
    public static string HubUrl = "/player-hub";

    public void HighlightCard(GameStateService gameStateService, string cardId, bool on)
    {
        var playerColor = Context.User.Identity.Name;

        gameStateService.ToggleCardHighlight(new CardHighlight {
            Color = playerColor,
            Guid = cardId,
            On = on,
        });
    }
}