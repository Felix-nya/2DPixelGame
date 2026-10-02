using UnityEngine;

public static class DamageCalculator
{
    public static float Calculate(float damage, float armor)
    {
        return Mathf.Max(1f, damage - armor);
    }
}