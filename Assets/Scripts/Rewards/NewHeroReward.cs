using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHeroReward", menuName = "Game/Rewards/New Hero")]
public class NewHeroReward : Reward
{
    [SerializeField] private List<HeroData> heroes;
    public override bool NeedsHeroTarget => false;
    public override bool IsAvailable(PlayerData player) => !player.TeamIsFull;
    public override void Apply(PlayerData player, HeroRuntimeData target)
    {
        foreach (var hero in heroes) player.AddHero(hero);
    }
}