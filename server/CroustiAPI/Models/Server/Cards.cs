using System.Diagnostics.CodeAnalysis;

namespace CroustiAPI;

public class Cards : HashSet<Card>
{
    public Cards() : base (new CardComparer())
    {
        
    }

    public void AddRange(List<Card> cards)
    {
        cards.ForEach(c => this.Add(c));
    }

    public void RemoveRange(List<Card> cards)
    {
        cards.ForEach(c => this.Remove(c));
    }
}

public class CardComparer : IEqualityComparer<Card>
{
    public bool Equals(Card x, Card y)
    {
        return x.Id == y.Id;
    }

    public int GetHashCode([DisallowNull] Card card)
    {
        return card.Id.GetHashCode();
    }
}