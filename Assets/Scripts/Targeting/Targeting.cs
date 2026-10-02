using System.Collections.Generic;
using UnityEngine;

public enum TargetingType { Nearest, LowestHealth, Random }

public interface ITargetingStrategy
{
    Hero ChooseTarget(Hero self, IReadOnlyList<Hero> allHeroes);
}

public class NearestEnemyTarget : ITargetingStrategy
{
    public Hero ChooseTarget(Hero self, IReadOnlyList<Hero> allHeroes)
    {
        Hero best = null;
        float bestDistance = float.MaxValue;

        foreach (var other in allHeroes)
        {
            if (!other.IsAlive || other.Team == self.Team) continue;

            float d = (other.transform.position - self.transform.position).sqrMagnitude;
            if (d < bestDistance)
            {
                bestDistance = d;
                best = other;
            }
        }
        return best;
    }
}

public class LowestHealthEnemyTarget : ITargetingStrategy
{
    public Hero ChooseTarget(Hero self, IReadOnlyList<Hero> allHeroes)
    {
        Hero best = null;
        float lowestHealth = float.MaxValue;

        foreach (var other in allHeroes)
        {
            if (!other.IsAlive || other.Team == self.Team) continue;

            if (other.CurrentHealth < lowestHealth)
            {
                lowestHealth = other.CurrentHealth;
                best = other;
            }
        }
        return best;
    }
}

public class RandomEnemyTarget : ITargetingStrategy
{
    public Hero ChooseTarget(Hero self, IReadOnlyList<Hero> allHeroes)
    {
        var enemies = new List<Hero>();
        foreach (var other in allHeroes)
        {
            if (other.IsAlive && other.Team != self.Team)
                enemies.Add(other);
        }
        return enemies.Count == 0 ? null : enemies[Random.Range(0, enemies.Count)];
    }
}

public static class TargetingFactory
{
    public static ITargetingStrategy Create(TargetingType type)
    {
        switch (type)
        {
            case TargetingType.LowestHealth: return new LowestHealthEnemyTarget();
            case TargetingType.Random: return new RandomEnemyTarget();
            default: return new NearestEnemyTarget();
        }
    }
}