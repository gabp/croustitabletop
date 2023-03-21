namespace CroustiAPI;

public record CardHighlight
{
    public string Guid { get; set; }

    public string Color { get; set; }

    public bool On { get; set; }
}
