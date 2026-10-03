using UnityEngine;

[CreateAssetMenu(fileName = "VampirismItem", menuName = "Game/Items/Vampirism")]
public class VampirismItem : ItemData
{
    public float lifestealPercent = 10f;

    public override void OnDamageDealt(Hero owner, Hero target, float damageDealt)
    {
        if (!owner.IsAlive) return;

        float healed = damageDealt * lifestealPercent / 100f;
        Debug.Log($"[временно] {owner.Data.heroName}: вампиризм лечит на {healed:0.0}");
        owner.Heal(healed);
    }
}