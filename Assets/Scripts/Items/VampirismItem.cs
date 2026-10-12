using UnityEngine;

[CreateAssetMenu(fileName = "VampirismItem", menuName = "Game/Items/Vampirism")]
public class VampirismItem : ItemData
{
    [SerializeField] private float lifestealPercent = 10f;

    public override void OnDamageDealt(Hero owner, Hero target, float damageDealt, DamageSource source)
    {
        if (!owner.IsAlive) return;
        owner.Heal(damageDealt * lifestealPercent / 100f);
    }
}