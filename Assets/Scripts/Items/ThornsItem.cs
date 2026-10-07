using UnityEngine;

[CreateAssetMenu(fileName = "ThornsItem", menuName = "Game/Items/Thorns")]
public class ThornsItem : ItemData
{
    public float reflectPercent = 10f;

    public override void OnDamageTaken(Hero owner, Hero attacker, float damageTaken, DamageSource source)
    {
        if (!attacker.IsAlive) return;

        // чистый урон без брони и предметов (attacker не передаём), чтобы не было цепочек отражений
        attacker.TakeDamage(damageTaken * reflectPercent / 100f);
    }
}