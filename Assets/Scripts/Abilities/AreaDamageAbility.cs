using UnityEngine;

[CreateAssetMenu(fileName = "AreaDamageAbility", menuName = "Game/Abilities/Area Damage")]
public class AreaDamageAbility : Ability
{
    public float damage = 25f;
    public float castRange = 5f;
    public float radius = 2f;

    public override bool TryUse(Hero caster, BattleManager battle)
    {
        Hero target = caster.CurrentTarget;
        if (target == null || !target.IsAlive) return false;

        if (Vector2.Distance(caster.transform.position, target.transform.position) > castRange)
            return false;

        Vector2 center = target.transform.position;

        foreach (var other in battle.Heroes)
        {
            if (!other.IsAlive || other.Team == caster.Team) continue;

            if (Vector2.Distance(center, other.transform.position) <= radius)
                other.TakeDamage(DamageCalculator.Calculate(damage, other.Armor));
        }
        return true;
    }
}