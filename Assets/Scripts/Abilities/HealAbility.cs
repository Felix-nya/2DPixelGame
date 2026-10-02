using UnityEngine;

[CreateAssetMenu(fileName = "HealAbility", menuName = "Game/Abilities/Heal")]
public class HealAbility : Ability
{
    public float healAmount = 30f;

    public override bool TryUse(Hero caster, BattleManager battle)
    {
        Hero best = null;
        float lowestRatio = 1f; // учитываем только раненых (меньше 100%)

        foreach (var ally in battle.Heroes)
        {
            if (!ally.IsAlive || ally.Team != caster.Team) continue;

            float ratio = ally.CurrentHealth / ally.Stats.maxHealth;
            if (ratio < lowestRatio)
            {
                lowestRatio = ratio;
                best = ally;
            }
        }

        if (best == null) return false;

        best.Heal(healAmount);
        return true;
    }
}