using System.Collections.Generic;

public class HeroRuntimeData
{
    public HeroData Data { get; }
    public Stats Stats { get; set; }
    public List<ItemData> Items { get; } = new List<ItemData>();   // новое

    public HeroRuntimeData(HeroData data)
    {
        Data = data;
        Stats = data.baseStats;
    }
}