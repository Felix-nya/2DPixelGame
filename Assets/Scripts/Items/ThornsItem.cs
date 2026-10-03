using UnityEngine;

[CreateAssetMenu(fileName = "ThornsItem", menuName = "Game/Items/Thorns")]
public class ThornsItem : ItemData
{
    public float reflectPercent = 10f;

    public override void OnDamageTaken(Hero owner, Hero attacker, float damageTaken)
    {
        if (!attacker.IsAlive) return;

        float reflected = damageTaken * reflectPercent / 100f;
        Debug.Log($"[временно] {owner.Data.heroName}: шипы отражают {reflected:0.0}");

        // attacker не передаём, чтобы шипы не запускали цепочку отражений
        attacker.TakeDamage(reflected);
    }
}