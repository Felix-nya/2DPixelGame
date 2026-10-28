using System.Collections.Generic;
using UnityEngine;

public class HeroRuntimeData
{
    public HeroData Data { get; }
    public Stats Stats { get; set; }
    public List<ItemData> Items { get; } = new();
    public Vector2 Position { get; set; }
    public bool IsPlaced { get; set; }
    public string ItemsText
    {
        get
        {
            if (Items.Count == 0) return "нет";
            var names = Items.ConvertAll(i => string.IsNullOrEmpty(i.itemName) ? i.name : i.itemName);
            return string.Join(", ", names);
        }
    }

    public HeroRuntimeData(HeroData data)
    {
        Data = data;
        Stats = data.baseStats;
    }

    public bool HasItem(ItemData item)
    {
        return Items.Contains(item);
    }

    public bool AddItem(ItemData item)
    {
        if (item == null || HasItem(item)) return false;
        Items.Add(item);
        return true;
    }
}