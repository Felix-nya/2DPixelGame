using System;
using UnityEngine;

[Serializable]
public class RarityWeights
{
    [SerializeField, Min(0f)] private float common = 60f;
    [SerializeField, Min(0f)] private float rare = 30f;
    [SerializeField, Min(0f)] private float legendary = 10f;

    public float Get(CardRares rarity)
    {
        switch (rarity)
        {
            case CardRares.Common: return common;
            case CardRares.Rare: return rare;
            case CardRares.Legendary: return legendary;
            default: throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null);
        }
    }
}