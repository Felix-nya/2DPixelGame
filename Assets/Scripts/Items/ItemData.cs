using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    [SerializeField] private bool affectsAbilities = true;

    public virtual bool AppliesTo(DamageSource source)
    {
        return source != DamageSource.Ability || affectsAbilities;
    }
    public virtual float ModifyOutgoingDamage(Hero owner, Hero target, float damage, DamageSource source)
    {
        return damage;
    }
    public virtual void OnDamageDealt(Hero owner, Hero target, float damageDealt, DamageSource source) { }
    public virtual void OnDamageTaken(Hero owner, Hero attacker, float damageTaken, DamageSource source) { }
}