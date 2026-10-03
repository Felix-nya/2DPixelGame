using UnityEngine;

[CreateAssetMenu(fileName = "NewHeroReward", menuName = "Game/Rewards/New Hero")]
public class NewHeroReward : Reward
{
    public HeroData hero;

    public override bool NeedsHeroTarget => false;

    public override bool IsAvailable(PlayerData player) => !player.TeamIsFull;

    public override void Apply(PlayerData player, HeroRuntimeData target)
    {
        player.AddHero(hero);
    }
}