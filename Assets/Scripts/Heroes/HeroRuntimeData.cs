using System.Collections.Generic;

public class HeroRuntimeData
{
    public HeroData Data { get; }
    public Stats Stats { get; set; }
    public List<ItemData> Items { get; } = new List<ItemData>();

    public HeroRuntimeData(HeroData data)
    {
        Data = data;
        Stats = data.baseStats;
    }

    public bool HasItem(ItemData item)
    {
        return Items.Contains(item);
    }

    // false, если такой предмет у героя уже есть
    public bool AddItem(ItemData item)
    {
        if (item == null || HasItem(item)) return false;

        Items.Add(item);
        return true;
    }

    // для карточек: «Шипы, Вампиризм» или «нет»
    public string ItemsText
    {
        get
        {
            if (Items.Count == 0) return "нет";

            var names = Items.ConvertAll(i => string.IsNullOrEmpty(i.itemName) ? i.name : i.itemName);
            return string.Join(", ", names);
        }
    }
}