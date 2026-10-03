using UnityEngine;

public abstract class Reward : ScriptableObject
{
    public string rewardName;
    [TextArea] public string description;
    public Sprite icon;

    // нужно ли игроку выбирать героя, которому достанется награда
    public virtual bool NeedsHeroTarget => true;

    // можно ли предлагать награду этому игроку (например, команда уже полная)
    public virtual bool IsAvailable(PlayerData player) => true;

    public abstract void Apply(PlayerData player, HeroRuntimeData target);
}