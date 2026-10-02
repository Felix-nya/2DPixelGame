using UnityEngine;

[CreateAssetMenu(fileName = "NewHero", menuName = "Game/Hero")]
public class HeroData : ScriptableObject
{
    public string heroName;
    public HeroRole role;
    public Sprite sprite;
    public Color tint = Color.white;
    public Stats baseStats;
    public TargetingType targetingType;
    public Ability ability;
}