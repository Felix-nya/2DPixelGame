using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    [Tooltip("Реагирует ли предмет на урон от способностей")]
    public bool affectsAbilities = true;

    // Работает ли предмет с уроном из этого источника.
    // Наследник может переопределить, если нужно особое правило.
    public virtual bool AppliesTo(DamageSource source)
    {
        return source != DamageSource.Ability || affectsAbilities;
    }

    // изменить урон, который владелец собирается нанести
    public virtual float ModifyOutgoingDamage(Hero owner, Hero target, float damage, DamageSource source)
    {
        return damage;
    }

    // владелец нанёс урон
    public virtual void OnDamageDealt(Hero owner, Hero target, float damageDealt, DamageSource source) { }

    // владелец получил урон
    public virtual void OnDamageTaken(Hero owner, Hero attacker, float damageTaken, DamageSource source) { }
}