using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private Hero heroPrefab;
    [SerializeField] private Transform leftSpawn;
    [SerializeField] private Transform rightSpawn;
    [SerializeField] private float spacing = 1.5f;
    [SerializeField] private float battleTimeout = 60f;

    private readonly List<Hero> heroes = new List<Hero>();
    private bool battleActive;
    private float battleTimer;

    public IReadOnlyList<Hero> Heroes => heroes;
    public bool IsActive => battleActive;
    public float TimeLeft => Mathf.Max(0f, battleTimeout - battleTimer);

    public event Action<BattleResult> BattleEnded;

    public void StartBattle(List<HeroRuntimeData> leftTeam, List<HeroRuntimeData> rightTeam)
    {
        ClearBattle();
        SpawnTeam(leftTeam, Team.Left, leftSpawn);
        SpawnTeam(rightTeam, Team.Right, rightSpawn);
        battleTimer = 0f;
        battleActive = true;
    }

    public void ClearBattle()
    {
        foreach (var hero in heroes)
        {
            if (hero != null) Destroy(hero.gameObject);
        }
        heroes.Clear();
        battleActive = false;
    }

    private void SpawnTeam(List<HeroRuntimeData> team, Team side, Transform spawn)
    {
        for (int i = 0; i < team.Count; i++)
        {
            float offsetY = (i - (team.Count - 1) / 2f) * spacing;
            Vector3 position = spawn.position + Vector3.up * offsetY;

            Hero hero = Instantiate(heroPrefab, position, Quaternion.identity);
            hero.Setup(team[i], side, this);
            hero.Died += OnHeroDied;
            heroes.Add(hero);
        }
    }

    private void FixedUpdate()
    {
        if (!battleActive) return;

        battleTimer += Time.fixedDeltaTime;
        if (battleTimer >= battleTimeout)
            EndBattle(ResolveByHealth());
    }

    private void OnHeroDied(Hero dead)
    {
        if (!battleActive) return;

        bool leftAlive = heroes.Exists(h => h.IsAlive && h.Team == Team.Left);
        bool rightAlive = heroes.Exists(h => h.IsAlive && h.Team == Team.Right);

        if (!leftAlive || !rightAlive)
        {
            if (leftAlive) EndBattle(BattleResult.LeftWon);
            else if (rightAlive) EndBattle(BattleResult.RightWon);
            else EndBattle(BattleResult.Draw);
        }
    }

    // по таймауту побеждает команда с большим процентом оставшегося здоровья
    private BattleResult ResolveByHealth()
    {
        float left = HealthRatio(Team.Left);
        float right = HealthRatio(Team.Right);

        if (Mathf.Approximately(left, right)) return BattleResult.Draw;
        return left > right ? BattleResult.LeftWon : BattleResult.RightWon;
    }

    private float HealthRatio(Team team)
    {
        float current = 0f, max = 0f;
        foreach (var h in heroes)
        {
            if (h.Team != team) continue;
            max += h.Stats.maxHealth;
            if (h.IsAlive) current += h.CurrentHealth;
        }
        return max > 0f ? current / max : 0f;
    }

    private void EndBattle(BattleResult result)
    {
        battleActive = false;
        Debug.Log($"Бой окончен: {result}");
        BattleEnded?.Invoke(result);
    }
}