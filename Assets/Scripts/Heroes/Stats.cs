using System;
using UnityEngine;

[Serializable]
public struct Stats
{
    public float maxHealth;
    public float damage;
    public float attackSpeed;
    public float range;
    public float moveSpeed;
    public float armor;
    public float critChance;
    public float critDamage;
    public float Get(StatType type)
    {
        switch (type)
        {
            case StatType.Damage: return damage;
            case StatType.AttackSpeed: return attackSpeed;
            case StatType.MoveSpeed: return moveSpeed;
            case StatType.Range: return range;
            case StatType.Armor: return armor;
            case StatType.CritChance: return critChance;
            case StatType.CritDamage: return critDamage;
            default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    public void Set(StatType type, float value)
    {
        switch (type)
        {
            case StatType.Damage: damage = value; break;
            case StatType.AttackSpeed: attackSpeed = value; break;
            case StatType.MoveSpeed: moveSpeed = value; break;
            case StatType.Range: range = value; break;
            case StatType.Armor: armor = value; break;
            case StatType.CritChance: critChance = value; break;
            case StatType.CritDamage: critDamage = value; break;
            default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}