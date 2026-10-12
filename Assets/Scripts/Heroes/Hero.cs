using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Hero : MonoBehaviour
{
    private const float RetargetInterval = 0.25f;
    private static readonly StatType[] AllStatTypes = (StatType[])Enum.GetValues(typeof(StatType));

    [SerializeField] private HealthBar healthBar;

    public Team Team { get; private set; }
    public HeroData Data { get; private set; }
    public Stats Stats { get; private set; }
    public float CurrentHealth { get; private set; }
    public bool IsAlive => CurrentHealth > 0f;
    public Hero CurrentTarget => target;
    public float Armor => Stats.armor;
    public event Action<Hero> Died;
    public event Action<Hero, float, bool> DamageTaken;
    public event Action<Hero, float> Healed;

    private readonly List<StatModifier> modifiers = new();
    private IReadOnlyList<ItemData> items = new List<ItemData>();
    private Stats baseStats;
    private BattleManager battle;
    private ITargetingStrategy targeting;
    private Hero target;
    private float attackTimer;
    private float abilityTimer;
    private float retargetTimer;

    private void FixedUpdate()
    {
        if (battle == null || !IsAlive || !battle.IsActive) return;
        float dt = Time.fixedDeltaTime;
        TickModifiers(dt);
        UpdateTarget(dt);
        if (target == null) return;
        UpdateAbility(dt);
        MoveOrAttack(dt);
    }

    public void Setup(HeroRuntimeData runtime, Team team, BattleManager battle)
    {
        HeroData data = runtime.Data;
        Data = data;
        Team = team;
        baseStats = runtime.Stats;
        Stats = baseStats;
        modifiers.Clear();
        items = runtime.Items;
        this.battle = battle;
        targeting = TargetingFactory.Create(data.targetingType);
        CurrentHealth = Stats.maxHealth;
        abilityTimer = data.ability != null ? data.ability.cooldown : 0f;
        var sr = GetComponent<SpriteRenderer>();
        Sprite teamSprite = team == Team.Left ? data.spriteLeftTeam : data.spriteRightTeam;
        if (teamSprite != null) sr.sprite = teamSprite;
        sr.color = data.tint;
        healthBar.SetColor(team == Team.Left ? Color.green : Color.red);
        UpdateHealthBar();
    }

    public void DealDamage(Hero victim, float baseDamage, DamageSource source, bool canCrit = true)
    {
        if (!IsAlive || !victim.IsAlive) return;
        float damage = baseDamage;
        foreach (var item in items)
        {
            if (item.AppliesTo(source)) damage = item.ModifyOutgoingDamage(this, victim, damage, source);
        }
        bool isCrit = false;
        if (canCrit) damage = DamageCalculator.RollCrit(damage, Stats.critChance, Stats.critDamage, out isCrit);
        float dealt = DamageCalculator.Calculate(damage, victim.Armor);
        victim.TakeDamage(dealt, this, source, isCrit);
        foreach (var item in items)
        {
            if (item.AppliesTo(source)) item.OnDamageDealt(this, victim, dealt, source);
        }
    }

    public void TakeDamage(float amount, Hero attacker = null, DamageSource source = DamageSource.Attack, bool isCrit = false)
    {
        if (!IsAlive) return;
        CurrentHealth -= amount;
        UpdateHealthBar();
        DamageTaken?.Invoke(this, amount, isCrit);
        if (CurrentHealth <= 0f)
        {
            Died?.Invoke(this);
            gameObject.SetActive(false);
            return;
        }
        if (attacker == null) return;
        foreach (var item in items)
        {
            if (item.AppliesTo(source)) item.OnDamageTaken(this, attacker, amount, source);
        }
    }

    public void Heal(float amount)
    {
        float before = CurrentHealth;
        CurrentHealth = Mathf.Min(CurrentHealth + amount, Stats.maxHealth);
        UpdateHealthBar();
        float healed = CurrentHealth - before;
        if (healed > 0f) Healed?.Invoke(this, healed);
    }

    public void AddModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
        RecalculateStats();
    }

    private void TickModifiers(float dt)
    {
        bool changed = false;
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            modifiers[i].Remaining -= dt;
            if (modifiers[i].Remaining <= 0f)
            {
                modifiers.RemoveAt(i);
                changed = true;
            }
        }
        if (changed) RecalculateStats();
    }

    private void RecalculateStats()
    {
        Stats result = baseStats;
        foreach (var type in AllStatTypes)
        {
            float flat = 0f;
            float percent = 0f;
            foreach (var m in modifiers)
            {
                if (m.Stat != type) continue;
                flat += m.Flat;
                percent += m.Percent;
            }
            result.Set(type, baseStats.Get(type) * (1f + percent / 100f) + flat);
        }
        Stats = result;
    }

    private void UpdateTarget(float dt)
    {
        retargetTimer -= dt;
        if (target == null || !target.IsAlive || retargetTimer <= 0f)
        {
            target = targeting.ChooseTarget(this, battle.Heroes);
            retargetTimer = RetargetInterval;
        }
    }

    private void UpdateAbility(float dt)
    {
        var ability = Data.ability;
        if (ability == null) return;
        abilityTimer -= dt;
        if (abilityTimer <= 0f && ability.TryUse(this, battle)) abilityTimer = ability.cooldown;
    }

    private void MoveOrAttack(float dt)
    {
        float distance = Vector2.Distance(transform.position, target.transform.position);
        if (distance > Stats.range)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Stats.moveSpeed * dt);
            return;
        }
        attackTimer -= dt;
        if (attackTimer <= 0f)
        {
            DealDamage(target, Stats.damage, DamageSource.Attack);
            attackTimer += 1f / Stats.attackSpeed;
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null) healthBar.SetRatio(CurrentHealth / Stats.maxHealth);
    }
}