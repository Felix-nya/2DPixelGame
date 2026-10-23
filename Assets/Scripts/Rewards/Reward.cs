using UnityEngine;

public enum CardRares
{
    Common,
    Rare,
    Legendary
}
public abstract class Reward : ScriptableObject
{
    public string rewardName;
    [TextArea] public string description;
    public Sprite icon;
    public CardRares rare;

    public virtual bool NeedsHeroTarget => true;
    public virtual bool CanTarget(HeroRuntimeData hero) => true;
    public virtual bool IsAvailable(PlayerData player)
    {
        if (!NeedsHeroTarget) return true;
        foreach (var hero in player.Team)
        {
            if (CanTarget(hero)) return true;
        }
        return false;
    }
    public abstract void Apply(PlayerData player, HeroRuntimeData target);
}