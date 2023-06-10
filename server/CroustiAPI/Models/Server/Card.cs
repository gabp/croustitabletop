namespace CroustiAPI;

public record Card
{
    public string Id { get; set; }

    public byte[] ImageBytes { get; set; }
}