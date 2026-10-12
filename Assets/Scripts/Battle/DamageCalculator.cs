using UnityEngine;

public static class DamageCalculator
{
    public static float Calculate(float damage, float armor)
    {
        return Mathf.Max(1f, damage - armor);
    }
    public static float RollCrit(float damage, float critChancePercent, float critDamagePercent, out bool isCrit)
    {
        isCrit = Random.value * 100f < critChancePercent;
        if (!isCrit) return damage;
        return damage * Mathf.Max(1f, critDamagePercent / 100f);
    }
}