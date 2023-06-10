
using CroustiAPI;

public static class EntityMappers
{
    public static Card ToCard(this UnityCardEntity unityCard)
    {
        return new Card
        {
            Id = unityCard.Id,
            ImageBytes = unityCard.ImageBytes,
        };
    }

    public static List<Card> ToCards(this IEnumerable<UnityCardEntity> unityCards)
    {
        return unityCards.Select(ToCard).ToList();
    }

    public static WebAppCardEntity ToWebAppCardEntity(this Card card)
    {
        return new WebAppCardEntity
        {
            Guid = card.Id,
            ImageURL = $"data:image/jpg;base64, {Convert.ToBase64String(card.ImageBytes)}",
        };
    }

    public static List<WebAppCardEntity> ToWebAppCardEntities(this IEnumerable<Card> cards)
    {
        return cards.Select(ToWebAppCardEntity).ToList();
    }
}