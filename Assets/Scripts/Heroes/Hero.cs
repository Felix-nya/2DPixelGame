using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(HeroVisual))]
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
    private HeroVisual visual;
    private ITargetingStrategy targeting;
    private Hero target;
    private float attackTimer;
    private float abilityTimer;
    private float retargetTimer;
    private bool swinging;
    private float swingTimer;
    private Hero swingTarget;

    private void Awake()
    {
        visual = GetComponent<HeroVisual>();
    }

    private void FixedUpdate()
    {
        if (battle == null || !IsAlive) return;
        if (!battle.IsActive)
        {
            visual.SetMoving(false);
            return;
        }
        float dt = Time.fixedDeltaTime;
        TickModifiers(dt);
        UpdateTarget(dt);
        if (target == null)
        {
            visual.SetMoving(false);
            return;
        }
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
        swinging = false;
        swingTarget = null;
        Sprite teamSprite = team == Team.Left ? data.spriteLeftTeam : data.spriteRightTeam;
        RuntimeAnimatorController controller = data.GetAnimator(team);
        visual.Setup(controller, teamSprite, data.tint, data.NativeFacing);
        visual.FaceDirection(team == Team.Left ? 1f : -1f);
        healthBar.SetColor(team == Team.Left ? Color.green : Color.red);
        UpdateHealthBar();
    }

    public DamagePacket PrepareDamage(Hero victim, float baseDamage, DamageSource source, bool canCrit = true)
    {
        float damage = baseDamage;
        foreach (var item in items)
        {
            if (item.AppliesTo(source)) damage = item.ModifyOutgoingDamage(this, victim, damage, source);
        }
        bool isCrit = false;
        if (canCrit) damage = DamageCalculator.RollCrit(damage, Stats.critChance, Stats.critDamage, out isCrit);
        return new DamagePacket(damage, isCrit, source);
    }

    public void ApplyDamage(Hero victim, DamagePacket packet)
    {
        if (victim == null || !victim.IsAlive) return;
        float dealt = DamageCalculator.Calculate(packet.Amount, victim.Armor);
        victim.TakeDamage(dealt, this, packet.Source, packet.IsCrit);
        foreach (var item in items)
        {
            if (item.AppliesTo(packet.Source)) item.OnDamageDealt(this, victim, dealt, packet.Source);
        }
    }

    public void DealDamage(Hero victim, float baseDamage, DamageSource source, bool canCrit = true)
    {
        if (!IsAlive || !victim.IsAlive) return;
        ApplyDamage(victim, PrepareDamage(victim, baseDamage, source, canCrit));
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
            visual.PlayDeath();
            if (healthBar != null) healthBar.gameObject.SetActive(false);
            return;
        }
        visual.PlayHurt();
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
        if (swinging)
        {
            TickSwing(dt);
            return;
        }
        visual.FaceDirection(target.transform.position.x - transform.position.x);
        float distance = Vector2.Distance(transform.position, target.transform.position);
        if (distance > Stats.range)
        {
            visual.SetMoving(true);
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Stats.moveSpeed * dt);
            return;
        }
        visual.SetMoving(false);
        attackTimer -= dt;
        if (attackTimer <= 0f) StartSwing();
    }

    private void StartSwing()
    {
        float reference = Mathf.Max(0.01f, Data.ReferenceAttackSpeed);
        float animSpeed = Mathf.Max(1f, Stats.attackSpeed / reference);
        swinging = true;
        swingTarget = target;
        swingTimer = Data.AttackHitDelay / animSpeed;
        attackTimer += 1f / Stats.attackSpeed;
        visual.PlayAttack(animSpeed);
    }

    private void TickSwing(float dt)
    {
        attackTimer -= dt;
        swingTimer -= dt;
        if (swingTimer > 0f) return;

        swinging = false;
        Hero victim = swingTarget;
        swingTarget = null;
        if (victim == null || !victim.IsAlive) return;

        if (Data.ProjectilePrefab != null)
            FireProjectile(victim);
        else
            DealDamage(victim, Stats.damage, DamageSource.Attack);
    }

    private void FireProjectile(Hero victim)
    {
        DamagePacket packet = PrepareDamage(victim, Stats.damage, DamageSource.Attack);

        Vector2 offset = Data.ProjectileSpawnOffset;
        Vector3 spawn = transform.position + new Vector3(offset.x * visual.Facing, offset.y, 0f);

        Projectile projectile = Instantiate(Data.ProjectilePrefab, spawn, Quaternion.identity);
        projectile.Launch(this, victim, packet, battle);
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null) healthBar.SetRatio(CurrentHealth / Stats.maxHealth);
    }
}