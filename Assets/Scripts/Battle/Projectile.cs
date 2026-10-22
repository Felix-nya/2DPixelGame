using UnityEngine;

public class Projectile : MonoBehaviour
{
    private const float ArriveDistance = 0.02f;

    [SerializeField] private float speed = 10f;

    private Hero owner;
    private Hero target;
    private BattleManager battle;
    private DamagePacket packet;
    private Vector2 lastTargetPosition;

    public void Launch(Hero owner, Hero target, DamagePacket packet, BattleManager battle)
    {
        this.owner = owner;
        this.target = target;
        this.packet = packet;
        this.battle = battle;
        lastTargetPosition = target.transform.position;
        Rotate(lastTargetPosition - (Vector2)transform.position);
    }

    private void FixedUpdate()
    {
        if (battle == null || !battle.IsActive)
        {
            Destroy(gameObject);
            return;
        }
        bool targetAlive = target != null && target.IsAlive;
        if (targetAlive)
            lastTargetPosition = target.transform.position;
        Vector2 current = transform.position;
        Vector2 next = Vector2.MoveTowards(current, lastTargetPosition, speed * Time.fixedDeltaTime);
        transform.position = next;
        Rotate(lastTargetPosition - current);
        if (Vector2.Distance(next, lastTargetPosition) > ArriveDistance) return;
        if (targetAlive && owner != null)
            owner.ApplyDamage(target, packet);

        Destroy(gameObject);
    }

    private void Rotate(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.0001f) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}