using System.Collections.Generic;
using UnityEngine;

public static class RewardGenerator
{
    // выбирает count случайных наград без повторов из тех, что доступны игроку
    public static List<Reward> Generate(IReadOnlyList<Reward> pool, PlayerData player, int count)
    {
        var available = new List<Reward>();
        foreach (var reward in pool)
        {
            if (reward != null && reward.IsAvailable(player))
                available.Add(reward);
        }

        var result = new List<Reward>();
        while (result.Count < count && available.Count > 0)
        {
            int index = Random.Range(0, available.Count);
            result.Add(available[index]);
            available.RemoveAt(index);
        }
        return result;
    }
}