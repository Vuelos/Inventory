namespace Inventory;

public class Item
{
    public ItemType Type { get; set; }
    public int Count { get; set; }

    public RPG.ItemStatsCollection Stats { get; set; }

    public Item(ItemType type, int count)
    {
        Type = type;
        Count = count;
    }

    public Item(ItemType type, int count, RPG.ItemStatsCollection stats)
    {
        Type = type;
        Count = count;
        Stats = stats;
    }

    /// <summary>
    /// Generates item with random stats based on ilevel and statsGroups
    /// </summary>
    /// <param name="type"></param>
    /// <param name="ilevel"></param>
    /// <param name="statsGroups"></param>
    public Item(ItemType type, int ilevel, int statsGroups)
    {
        Type = type;
        Count = 1;
        Stats = new RPG.ItemStatsCollection(ilevel, statsGroups);
    }
    public Item(ItemType type, RPG.ItemStatsCollection stats)
    {
        Type = type;
        Count = 1;
        Stats = stats;
    }
    public virtual Item Clone() => new(Type, Count, Stats);
    public void Hide() => Type.Hide();
    public void Show() => Type.Show();

    public override string ToString() => $"{Count} {Type}{(Count == 1 ? "" : "s")}";
}
