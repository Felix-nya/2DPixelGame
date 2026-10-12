using UnityEngine;

[CreateAssetMenu(fileName = "ItemReward", menuName = "Game/Rewards/Item")]
public class ItemReward : Reward
{
    public ItemData item;
    public override bool CanTarget(HeroRuntimeData hero)
    {
        return !hero.HasItem(item);
    }
    public override void Apply(PlayerData player, HeroRuntimeData target)
    {
        target.AddItem(item);
    }
}