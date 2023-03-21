namespace CroustiAPI;

public record Player
{
    public string Color { get; set; }

    public List<Card> Cards { get; set; } = new ();
}
