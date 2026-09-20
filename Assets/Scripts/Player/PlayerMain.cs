using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMain : MonoBehaviour
{
    public static PlayerMain Instance { get; private set; }
    [SerializeField] private float _movingSpeed = 10f;
    private Rigidbody2D rb;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        Moving();
    }
    private void Moving()
    {
        Vector2 inputVector = PlayerInputActions.Instance.GetMovementVector();
        rb.MovePosition(rb.position + _movingSpeed * Time.fixedDeltaTime * inputVector);
    }
}
