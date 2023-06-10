namespace CroustiAPI;

public record UnityCardHighlightEntity
{
    public string CardId { get; set; }

    public string PlayerColor { get; set; }

    public bool IsHighlighted { get; set; }
}
