using UnityEngine;

public static class DamageCalculator
{
    // урон после брони, минимум 1
    public static float Calculate(float damage, float armor)
    {
        return Mathf.Max(1f, damage - armor);
    }

    // бросок крита: возвращает урон (увеличенный при крите)
    public static float RollCrit(float damage, float critChancePercent, float critDamagePercent, out bool isCrit)
    {
        isCrit = Random.value * 100f < critChancePercent;
        if (!isCrit) return damage;

        return damage * Mathf.Max(1f, critDamagePercent / 100f);
    }
}