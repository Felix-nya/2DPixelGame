using System;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    private const float RetargetInterval = 0.25f;   // как часто пересчитываем цель

    [SerializeField] private HealthBar healthBar;

    public Team Team { get; private set; }
    public HeroData Data { get; private set; }
    public Stats Stats { get; private set; }

    public float CurrentHealth { get; private set; }
    public bool IsAlive => CurrentHealth > 0f;

    public Hero CurrentTarget => target;
    public float Armor => Stats.armor + BonusArmor;

    public event Action<Hero> Died;

    private class ArmorBuff
    {
        public float amount;
        public float remaining;
    }

    private readonly List<ArmorBuff> armorBuffs = new List<ArmorBuff>();
    private IReadOnlyList<ItemData> items = new List<ItemData>();

    private BattleManager battle;
    private ITargetingStrategy targeting;
    private Hero target;
    private float attackTimer;
    private float abilityTimer;
    private float retargetTimer;

    private float BonusArmor
    {
        get
        {
            float sum = 0f;
            foreach (var buff in armorBuffs) sum += buff.amount;
            return sum;
        }
    }

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

        healthBar.SetColor(team == Team.Left ? Color.green : Color.red);
        UpdateHealthBar();
    }

    // Вся боевая логика идёт фиксированными шагами, поэтому не зависит от FPS
    private void FixedUpdate()
    {
        if (battle == null || !IsAlive || !battle.IsActive) return;

        float dt = Time.fixedDeltaTime;

        TickBuffs(dt);
        UpdateTarget(dt);
        if (target == null) return;

        UpdateAbility(dt);
        MoveOrAttack(dt);
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
        if (abilityTimer <= 0f && ability.TryUse(this, battle))
            abilityTimer = ability.cooldown;
    }

    private void MoveOrAttack(float dt)
    {
        float distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance > Stats.range)
        {
            transform.position = Vector2.MoveTowards(
                transform.position, target.transform.position, Stats.moveSpeed * dt);
            return;
        }

        attackTimer -= dt;
        if (attackTimer <= 0f)
        {
            DealDamage(target, Stats.damage, DamageSource.Attack);
            attackTimer += 1f / Stats.attackSpeed;   // += сохраняет остаток времени
        }
    }

    // ---------- Урон ----------

    // Единственный путь нанесения урона: атаки и способности вызывают его.
    public void DealDamage(Hero victim, float baseDamage, DamageSource source)
    {
        if (!IsAlive || !victim.IsAlive) return;

        float damage = baseDamage;
        foreach (var item in items)
        {
            if (item.AppliesTo(source))
                damage = item.ModifyOutgoingDamage(this, victim, damage, source);
        }

        float dealt = DamageCalculator.Calculate(damage, victim.Armor);
        victim.TakeDamage(dealt, this, source);

        foreach (var item in items)
        {
            if (item.AppliesTo(source))
                item.OnDamageDealt(this, victim, dealt, source);
        }
    }

    // Применяет уже готовый урон (броня учтена). attacker нужен только для реакций предметов.
    public void TakeDamage(float amount, Hero attacker = null, DamageSource source = DamageSource.Attack)
    {
        if (!IsAlive) return;

        CurrentHealth -= amount;
        UpdateHealthBar();

        if (CurrentHealth <= 0f)
        {
            Died?.Invoke(this);
            gameObject.SetActive(false);
            return;
        }

        if (attacker == null) return;

        foreach (var item in items)
        {
            if (item.AppliesTo(source))
                item.OnDamageTaken(this, attacker, amount, source);
        }
    }

    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, Stats.maxHealth);
        UpdateHealthBar();
    }

    // ---------- Баффы ----------

    public void AddArmorBuff(float amount, float duration)
    {
        armorBuffs.Add(new ArmorBuff { amount = amount, remaining = duration });
    }

    private void TickBuffs(float dt)
    {
        for (int i = armorBuffs.Count - 1; i >= 0; i--)
        {
            armorBuffs[i].remaining -= dt;
            if (armorBuffs[i].remaining <= 0f)
                armorBuffs.RemoveAt(i);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.SetRatio(CurrentHealth / Stats.maxHealth);
    }
}