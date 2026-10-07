using UnityEngine;

[CreateAssetMenu(fileName = "ItemReward", menuName = "Game/Rewards/Item")]
public class ItemReward : Reward
{
    public ItemData item;

    // один и тот же предмет герою можно дать только один раз
    public override bool CanTarget(HeroRuntimeData hero)
    {
        return !hero.HasItem(item);
    }

    public override void Apply(PlayerData player, HeroRuntimeData target)
    {
        target.AddItem(item);
    }
}