using UnityEngine;

public abstract class Reward : ScriptableObject
{
    public string rewardName;
    [TextArea] public string description;
    public Sprite icon;

    // нужно ли игроку выбирать героя, которому достанется награда
    public virtual bool NeedsHeroTarget => true;

    // можно ли дать награду именно этому герою
    public virtual bool CanTarget(HeroRuntimeData hero) => true;

    // можно ли предлагать награду игроку: нужен хотя бы один подходящий герой
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