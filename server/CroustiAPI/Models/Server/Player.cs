namespace CroustiAPI;

public record Player
{
    public string Color { get; set; }

    public Cards Cards { get; set; } = new ();

    public void AddCards(List<Card> cards)
    {
        this.Cards.AddRange(cards);
    }

    public void RemoveCards(List<Card> cards)
    {
        this.Cards.RemoveRange(cards);
    }
}
