using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewHero", menuName = "Game/Hero")]
public class HeroData : ScriptableObject
{
    [SerializeField] private RuntimeAnimatorController animatorLeftTeam;
    [SerializeField] private RuntimeAnimatorController animatorRightTeam;
    [SerializeField] private float attackHitDelay = 0.25f;
    public float AttackHitDelay => attackHitDelay;
    public float ReferenceAttackSpeed => baseStats.attackSpeed;
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
    public RuntimeAnimatorController GetAnimator(Team team, out bool mirrored)
    {
        mirrored = false;
        if (team == Team.Left) return animatorLeftTeam;
        if (animatorRightTeam != null) return animatorRightTeam;

        mirrored = animatorLeftTeam != null;
        return animatorLeftTeam;
    }
}