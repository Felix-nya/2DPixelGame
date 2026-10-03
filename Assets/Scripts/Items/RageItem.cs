using UnityEngine;

[CreateAssetMenu(fileName = "RageItem", menuName = "Game/Items/Rage")]
public class RageItem : ItemData
{
    public float healthThresholdPercent = 30f;
    public float damageMultiplier = 1.5f;

    public override float ModifyOutgoingDamage(Hero owner, Hero target, float damage)
    {
        float ratio = owner.CurrentHealth / owner.Stats.maxHealth;
        if (ratio >= healthThresholdPercent / 100f) return damage;

        Debug.Log($"[временно] {owner.Data.heroName}: ярость, урон x{damageMultiplier}");
        return damage * damageMultiplier;
    }
}