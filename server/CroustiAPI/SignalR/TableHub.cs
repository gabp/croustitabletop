using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CroustiAPI;

public class TableHub : Hub
{
    public static string HubUrl = "/table-hub";
    public static string HighlighCardToggledEvent = "HighlighCardToggledEvent";

    public void UpdateCards(GameStateService gameStateService, string playerColor, List<UnityCardEntity> unityCards)
    {
        gameStateService.UpdatePlayer(playerColor, unityCards.ToCards());
    }
}