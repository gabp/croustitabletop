namespace CroustiAPI;

public record GameState
{
    public List<Player> Players { get; set; } = new ();
}
