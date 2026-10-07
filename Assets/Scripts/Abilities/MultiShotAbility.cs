using UnityEngine;

[CreateAssetMenu(fileName = "MultiShotAbility", menuName = "Game/Abilities/Multi Shot")]
public class MultiShotAbility : Ability
{
    public int shots = 3;
    public float damagePerShot = 8f;
    public float castRange = 6f;

    public override bool TryUse(Hero caster, BattleManager battle)
    {
        Hero target = caster.CurrentTarget;
        if (target == null || !target.IsAlive) return false;

        if (Vector2.Distance(caster.transform.position, target.transform.position) > castRange)
            return false;

        for (int i = 0; i < shots; i++)
        {
            if (!target.IsAlive || !caster.IsAlive) break;
            caster.DealDamage(target, damagePerShot, DamageSource.Ability);
        }
        return true;
    }
}