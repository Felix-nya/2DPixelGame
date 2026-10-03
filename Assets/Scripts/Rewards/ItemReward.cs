using UnityEngine;

[CreateAssetMenu(fileName = "ItemReward", menuName = "Game/Rewards/Item")]
public class ItemReward : Reward
{
    public ItemData item;

    public override void Apply(PlayerData player, HeroRuntimeData target)
    {
        target.Items.Add(item);
    }
}