using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    // изменить урон, который владелец собирается нанести
    public virtual float ModifyOutgoingDamage(Hero owner, Hero target, float damage)
    {
        return damage;
    }

    // владелец нанёс урон
    public virtual void OnDamageDealt(Hero owner, Hero target, float damageDealt) { }

    // владелец получил урон
    public virtual void OnDamageTaken(Hero owner, Hero attacker, float damageTaken) { }
}