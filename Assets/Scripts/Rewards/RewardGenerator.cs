using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class RewardGenerator
{
    public static List<Reward> Generate(IReadOnlyList<Reward> pool, PlayerData player, int count, RarityWeights weights)
    {
        int rarityCount = Enum.GetValues(typeof(CardRares)).Length;
        var byRarity = new List<Reward>[rarityCount];
        for (int i = 0; i < rarityCount; i++)
            byRarity[i] = new List<Reward>();

        foreach (var reward in pool)
        {
            if (reward != null && reward.IsAvailable(player))
                byRarity[(int)reward.rare].Add(reward);
        }

        var result = new List<Reward>();
        while (result.Count < count)
        {
            int rarityIndex = RollRarity(byRarity, weights);
            if (rarityIndex < 0) break;

            List<Reward> group = byRarity[rarityIndex];
            int index = Random.Range(0, group.Count);
            result.Add(group[index]);
            group.RemoveAt(index);
        }
        return result;
    }

    private static int RollRarity(List<Reward>[] byRarity, RarityWeights weights)
    {
        float total = 0f;
        for (int i = 0; i < byRarity.Length; i++)
        {
            if (byRarity[i].Count > 0)
                total += weights.Get((CardRares)i);
        }

        if (total > 0f)
        {
            float roll = Random.value * total;
            for (int i = 0; i < byRarity.Length; i++)
            {
                if (byRarity[i].Count == 0) continue;

                roll -= weights.Get((CardRares)i);
                if (roll <= 0f) return i;
            }
        }

        for (int i = byRarity.Length - 1; i >= 0; i--)
        {
            if (byRarity[i].Count > 0) return i;
        }
        return -1;
    }
}