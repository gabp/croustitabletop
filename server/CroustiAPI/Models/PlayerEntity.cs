namespace CroustiAPI;

public record PlayerEntity
{
    public string Color { get; set; }

    public List<Card> Cards { get; set; } = new ();
}
