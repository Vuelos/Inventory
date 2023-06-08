namespace RPG;

public class ItemStatsCollection //: Inventory.Item
{
    public List<KeyValuePair<ItemStats, int>> stats;

    //public RPGItem(ItemType type, KeyValuePair<ItemStats, int>[] stats) : base(type, 1)
    public ItemStatsCollection(List<KeyValuePair<ItemStats, int>> stats)
    {
        this.stats = stats;
    }

    //public RPGItem(ItemType type, int ilevel, int statsGroups) : base(type, 1)
    public ItemStatsCollection(int ilevel, int statsGroups)
    {
        this.stats = StatsGenerator.GenerateRandomStats(ilevel, statsGroups);
    }

    //public override RPGItem Clone() => new(Type, stats);
}

public enum ItemStats
{
    Strength,
    Defense,
    HP,
    Speed
};

public static class StatsGenerator
{
    static private int RandInt(int max) => (int)(GD.Randi() % max);
    static int ItemStatsLenght = System.Enum.GetValues<ItemStats>().Length;
    static List<ItemStats> ItemStatsValues = System.Enum.GetValues<ItemStats>().ToList();
    static ItemStats GetRandomItemStat() => ItemStatsValues.ElementAt(RandInt(ItemStatsLenght));
    static ItemStats GetRandomItemStat(List<ItemStats> excluded)
    {
        var possibleStats = ItemStatsValues.Where(v => !excluded.Contains(v));
        return possibleStats.ElementAt(RandInt(possibleStats.Count()));
    }

    //Linear scaling - needs to be defined for each ItemStats
    static KeyValuePair<ItemStats, int>[] statScaling = new KeyValuePair<ItemStats, int>[]
    {
        new KeyValuePair<ItemStats, int>(ItemStats.Strength, 1),
        new KeyValuePair<ItemStats, int>(ItemStats.Defense, 1),
        new KeyValuePair<ItemStats, int>(ItemStats.HP, 10),
        new KeyValuePair<ItemStats, int>(ItemStats.Speed, 1)
    };

    static int GetStatScaling(ItemStats stat) => statScaling.SingleOrDefault(s => s.Key == stat).Value;

    /// <summary>
    /// Divide parametrized int into parametrized amount of int with random distribution
    /// ex: 50,3 -> 20,20,10
    /// </summary>
    /// <param name="number"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    private static int[] DivideNumber(int number, int amount)
    {
        if (amount < 1) amount = 1;
        if (amount > number) amount = number;

        int[] result = new int[amount];

        int quotient = number / amount; //100 / 3 -> 33
        int remainder = number % amount; //100 % 3 -> 1

        for (int i = 0; i < amount - 1; i++)
        {
            int randomOffset = RandInt(quotient/2 - quotient/4); //offset between -quotient *.25 and quotient *.25
            remainder += randomOffset;
            result[i] = quotient - randomOffset;
        }

        result[amount-1] = quotient + remainder;

        return result;
    }

    /// <summary>
    /// Generate new List of "stat" - "value" based on parametrized ilevel multiplied by stats scaling
    /// and maximum number of stats
    /// </summary>
    /// <param name="ilevel"></param>
    /// <param name="statsMaxAmount"></param>
    /// <returns></returns>
    public static List<KeyValuePair<ItemStats, int>> GenerateRandomStats(int ilevel, int statsGroups)
    {
        //validate not more stats groups are required than existing
        if (statsGroups > ItemStatsLenght) statsGroups = ItemStatsLenght;

        var itemStats = new List<KeyValuePair<ItemStats, int>>();
        var excludedStats = new List<ItemStats>();

        var statsDistribution = DivideNumber(ilevel, statsGroups);

        foreach (int val in statsDistribution)
        {
            var stat = GetRandomItemStat(excludedStats);
            excludedStats.Add(stat);
            itemStats.Add(new KeyValuePair<ItemStats, int>(stat, GetStatScaling(stat) * val));
        }

        return itemStats;
    }
}
