using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewHero", menuName = "Game/Hero")]
public class HeroData : ScriptableObject
{
    [SerializeField] private RuntimeAnimatorController animatorLeftTeam;
    [SerializeField] private RuntimeAnimatorController animatorRightTeam;
    [SerializeField] private float attackHitDelay = 0.25f;
    [Tooltip("Полная длина анимации удара в секундах при обычной скорости")]
    [SerializeField] private float attackClipLength = 0.6f;
    public float AttackClipLength => Mathf.Max(attackClipLength, attackHitDelay);
    public float AttackHitDelay => attackHitDelay;
    public float ReferenceAttackSpeed => baseStats.attackSpeed;
    public string heroName;
    public HeroRole role;
    [SerializeField, Min(0.1f)] private float placementRadius = 0.6f;

    public float PlacementRadius => placementRadius;

    [Header("Спрайты по командам")]
    [FormerlySerializedAs("sprite")]
    public Sprite spriteLeftTeam;
    public Sprite spriteRightTeam;
    [SerializeField] private bool spritesFaceRight;
    public int NativeFacing => spritesFaceRight ? 1 : -1;

    [Header("Снаряд (для дальнего боя)")]
    [SerializeField] private Projectile projectilePrefab;

    [Tooltip("Где появляется снаряд относительно героя, когда он смотрит вправо")]
    [SerializeField] private Vector2 projectileSpawnOffset = new Vector2(0.4f, 0.1f);

    public Projectile ProjectilePrefab => projectilePrefab;
    public Vector2 ProjectileSpawnOffset => projectileSpawnOffset;

    public Color tint = Color.white;
    public Stats baseStats;
    public TargetingType targetingType;
    public Ability ability;
    public RuntimeAnimatorController GetAnimator(Team team)
    {
        if (team == Team.Left) return animatorLeftTeam;
        return animatorRightTeam != null ? animatorRightTeam : animatorLeftTeam;
    }
}