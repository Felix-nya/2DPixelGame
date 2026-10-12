using UnityEngine;

[CreateAssetMenu(fileName = "ThornsItem", menuName = "Game/Items/Thorns")]
public class ThornsItem : ItemData
{
    [SerializeField] private float reflectPercent = 10f;

    public override void OnDamageTaken(Hero owner, Hero attacker, float damageTaken, DamageSource source)
    {
        if (!attacker.IsAlive) return;
        attacker.TakeDamage(damageTaken * reflectPercent / 100f);
    }
}