using System;
using UnityEngine;

[Serializable]
public struct Stats
{
    public float maxHealth;
    public float damage;
    public float attackSpeed;   // ударов в секунду
    public float range;         // дальность атаки
    public float moveSpeed;
    public float armor;

    [Tooltip("Шанс критического удара, %")]
    public float critChance;

    [Tooltip("Урон при крите в % от обычного (150 = x1.5)")]
    public float critDamage;
}