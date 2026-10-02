using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string abilityName;
    [TextArea] public string description;
    public float cooldown = 5f;
    public abstract bool TryUse(Hero caster, BattleManager battle);
}