using UnityEngine;

[CreateAssetMenu(fileName = "StatBuffReward", menuName = "Game/Rewards/Stat Buff")]
public class StatBuffReward : Reward
{
    [Header("Проценты (можно отрицательные)")]
    public float healthPercent;
    public float damagePercent;

    [Header("Плоские добавки")]
    public float attackSpeedFlat;
    public float armorFlat;

    public override void Apply(PlayerData player, HeroRuntimeData target)
    {
        Stats s = target.Stats;

        s.maxHealth = Mathf.Max(1f, s.maxHealth * (1f + healthPercent / 100f));
        s.damage = Mathf.Max(1f, s.damage * (1f + damagePercent / 100f));
        s.attackSpeed = Mathf.Max(0.1f, s.attackSpeed + attackSpeedFlat);
        s.armor += armorFlat;

        target.Stats = s;
    }
}