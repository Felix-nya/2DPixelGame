using System;
using System.Collections;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public Team Team { get; private set; }
    public HeroData Data { get; private set; }
    public Stats Stats { get; private set; }

    public float CurrentHealth { get; private set; }
    public bool IsAlive => CurrentHealth > 0f;

    public Hero CurrentTarget => target;
    public float Armor => Stats.armor + bonusArmor;

    public event Action<Hero> Died;

    private BattleManager battle;
    private ITargetingStrategy targeting;
    private Hero target;
    private float attackTimer;
    private float abilityTimer;
    private float bonusArmor;

    public void Setup(HeroData data, Team team, BattleManager battle)
    {
        Data = data;
        Team = team;
        Stats = data.baseStats;
        this.battle = battle;
        targeting = TargetingFactory.Create(data.targetingType);
        CurrentHealth = Stats.maxHealth;
        abilityTimer = data.ability != null ? data.ability.cooldown : 0f;

        var sr = GetComponent<SpriteRenderer>();
        if (data.sprite != null) sr.sprite = data.sprite;
        sr.color = data.tint;
    }

    private void Update()
    {
        if (battle == null || !IsAlive || !battle.IsActive) return;

        if (target == null || !target.IsAlive)
            target = targeting.ChooseTarget(this, battle.Heroes);
        if (target == null) return;

        UpdateAbility();

        float distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance > Stats.range)
        {
            transform.position = Vector2.MoveTowards(
                transform.position, target.transform.position, Stats.moveSpeed * Time.deltaTime);
        }
        else
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                target.TakeDamage(DamageCalculator.Calculate(Stats.damage, target.Armor));
                attackTimer = 1f / Stats.attackSpeed;
            }
        }
    }

    private void UpdateAbility()
    {
        var ability = Data.ability;
        if (ability == null) return;

        abilityTimer -= Time.deltaTime;
        if (abilityTimer <= 0f && ability.TryUse(this, battle))
        {
            abilityTimer = ability.cooldown;
            Debug.Log($"{Data.heroName} использует {ability.abilityName}");
        }
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0f)
        {
            Died?.Invoke(this);
            gameObject.SetActive(false);
        }
    }

    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, Stats.maxHealth);
    }

    public void AddArmorBuff(float amount, float duration)
    {
        StartCoroutine(ArmorBuffRoutine(amount, duration));
    }

    private IEnumerator ArmorBuffRoutine(float amount, float duration)
    {
        bonusArmor += amount;
        yield return new WaitForSeconds(duration);
        bonusArmor -= amount;
    }
}