using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewHero", menuName = "Game/Hero")]
public class HeroData : ScriptableObject
{
    public string heroName;
    public HeroRole role;

    [Header("Спрайты по командам")]
    [FormerlySerializedAs("sprite")]
    public Sprite spriteLeftTeam;
    public Sprite spriteRightTeam;

    public Color tint = Color.white;
    public Stats baseStats;
    public TargetingType targetingType;
    public Ability ability;
}