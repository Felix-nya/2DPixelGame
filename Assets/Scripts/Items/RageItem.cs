using UnityEngine;

[CreateAssetMenu(fileName = "RageItem", menuName = "Game/Items/Rage")]
public class RageItem : ItemData
{
    public float healthThresholdPercent = 30f;
    public float damageMultiplier = 1.5f;

    public override float ModifyOutgoingDamage(Hero owner, Hero target, float damage, DamageSource source)
    {
        float ratio = owner.CurrentHealth / owner.Stats.maxHealth;
        return ratio < healthThresholdPercent / 100f ? damage * damageMultiplier : damage;
    }
}