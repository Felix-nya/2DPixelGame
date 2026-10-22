using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class HeroVisual : MonoBehaviour
{
    private const float FaceDeadZone = 0.05f;

    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int AttackSpeedHash = Animator.StringToHash("AttackSpeed");

    [SerializeField] private Color hurtColor = new Color(1f, 0.35f, 0.35f);
    [SerializeField] private float hurtDuration = 0.15f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color baseColor = Color.white;
    private float hurtTimer;
    private bool isMoving;
    private int nativeFacing = 1;
    private int facing = 1;
    public int Facing => facing;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(RuntimeAnimatorController controller, Sprite staticSprite, Color tint, int nativeFacing)
    {
        bool animated = controller != null;

        animator.runtimeAnimatorController = controller;
        animator.enabled = animated;

        if (!animated && staticSprite != null)
            spriteRenderer.sprite = staticSprite;

        this.nativeFacing = nativeFacing;
        facing = nativeFacing;
        spriteRenderer.flipX = false;

        baseColor = tint;
        spriteRenderer.color = tint;
        hurtTimer = 0f;
        isMoving = false;
    }

    public void FaceDirection(float dx)
    {
        if (Mathf.Abs(dx) < FaceDeadZone) return;

        int desired = dx > 0f ? 1 : -1;
        if (desired == facing) return;

        facing = desired;
        spriteRenderer.flipX = facing != nativeFacing;
    }

    public void SetMoving(bool moving)
    {
        if (isMoving == moving) return;
        isMoving = moving;

        if (animator.enabled)
            animator.SetBool(IsMovingHash, moving);
    }

    public void PlayAttack(float speedMultiplier)
    {
        if (!animator.enabled) return;

        animator.SetFloat(AttackSpeedHash, speedMultiplier);
        animator.SetTrigger(AttackHash);
    }

    public void PlayDeath()
    {
        if (animator.enabled)
            animator.SetTrigger(DieHash);
    }

    public void PlayHurt()
    {
        hurtTimer = hurtDuration;
    }

    private void Update()
    {
        if (hurtTimer <= 0f) return;

        hurtTimer -= Time.deltaTime;
        float t = Mathf.Clamp01(hurtTimer / hurtDuration);
        spriteRenderer.color = Color.Lerp(baseColor, hurtColor, t);
    }
}