using UnityEngine;

[CreateAssetMenu(fileName = "RageItem", menuName = "Game/Items/Rage")]
public class RageItem : ItemData
{
    [SerializeField] private float healthThresholdPercent = 30f;
    [SerializeField] private float damageMultiplier = 1.5f;

    public override float ModifyOutgoingDamage(Hero owner, Hero target, float damage, DamageSource source)
    {
        float ratio = owner.CurrentHealth / owner.Stats.maxHealth;
        return ratio <= healthThresholdPercent / 100f ? damage * damageMultiplier : damage;
    }
}