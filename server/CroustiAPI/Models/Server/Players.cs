using System.Diagnostics.CodeAnalysis;

namespace CroustiAPI;

public class Players : HashSet<Player>
{
    public Players() : base (new PlayerComparer())
    {
        
    }

    public bool TryGetPlayer(Player player, out Player existingPlayer)
    {
        return this.TryGetValue(player, out existingPlayer);
    }

    public bool TryGetPlayer(string playerColor, out Player existingPlayer)
    {
        return this.TryGetPlayer(new Player { Color = playerColor }, out existingPlayer);
    }

    public void PutPlayer(Player player)
    {
        this.Remove(player);
        this.Add(player);
    }
}

public class PlayerComparer : IEqualityComparer<Player>
{
    public bool Equals(Player x, Player y)
    {
        return x.Color == y.Color;
    }

    public int GetHashCode([DisallowNull] Player player)
    {
        return player.Color.GetHashCode();
    }
}