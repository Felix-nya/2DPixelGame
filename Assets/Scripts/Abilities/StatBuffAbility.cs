using UnityEngine;

[CreateAssetMenu(fileName = "StatBuffAbility", menuName = "Game/Abilities/Stat Buff")]
public class StatBuffAbility : Ability
{
    [SerializeField] private StatType stat;
    [SerializeField] private float flat = 5f;
    [SerializeField] private float percent;
    [SerializeField] private float duration = 4f;
    public override bool TryUse(Hero caster, BattleManager battle)
    {
        caster.AddModifier(new StatModifier(stat, flat, percent, duration));
        return true;
    }
}