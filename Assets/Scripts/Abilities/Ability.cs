using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string abilityName;
    [TextArea] public string description;
    public float cooldown = 5f;
    [Tooltip("Может ли урон этой способности быть критическим")]
    public bool canCrit = true;
    public abstract bool TryUse(Hero caster, BattleManager battle);
}