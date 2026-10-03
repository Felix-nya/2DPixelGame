using System;
using System.Collections;
using System.Collections.Generic;
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
    private IReadOnlyList<ItemData> items = new List<ItemData>();

    public void Setup(HeroRuntimeData runtime, Team team, BattleManager battle)
    {
        HeroData data = runtime.Data;
        Data = data;
        Team = team;
        Stats = runtime.Stats;
        items = runtime.Items;
        this.battle = battle;
        targeting = TargetingFactory.Create(data.targetingType);
        CurrentHealth = Stats.maxHealth;
        abilityTimer = data.ability != null ? data.ability.cooldown : 0f;

        var sr = GetComponent<SpriteRenderer>();

        Sprite teamSprite = team == Team.Left ? data.spriteLeftTeam : data.spriteRightTeam;
        bool hasOwnSprite = teamSprite != null;
        if (!hasOwnSprite) teamSprite = data.spriteLeftTeam;   // запасной вариант

        if (teamSprite != null) sr.sprite = teamSprite;
        sr.flipX = team == Team.Right && !hasOwnSprite;        // зеркалим только запасной спрайт
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
                Attack(target);
                attackTimer = 1f / Stats.attackSpeed;
            }
        }
    }

    private void Attack(Hero victim)
    {
        float damage = Stats.damage;
        foreach (var item in items)
            damage = item.ModifyOutgoingDamage(this, victim, damage);

        float dealt = DamageCalculator.Calculate(damage, victim.Armor);
        victim.TakeDamage(dealt, this);

        foreach (var item in items)
            item.OnDamageDealt(this, victim, dealt);
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

    public void TakeDamage(float amount, Hero attacker = null)
    {
        CurrentHealth -= amount;

        if (CurrentHealth <= 0f)
        {
            Died?.Invoke(this);
            gameObject.SetActive(false);
            return;
        }

        if (attacker != null)
        {
            foreach (var item in items)
                item.OnDamageTaken(this, attacker, amount);
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