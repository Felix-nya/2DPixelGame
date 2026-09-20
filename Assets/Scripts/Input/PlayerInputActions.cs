using UnityEngine;

public class PlayerInputActions : MonoBehaviour
{
    public static PlayerInputActions Instance { get; private set; }
    private InputSystem _inputSystem;

    private void Awake()
    {
        Instance = this;
        _inputSystem = new InputSystem();
        _inputSystem.Enable();
    }

    public Vector2 GetMovementVector() => _inputSystem.Player.Move.ReadValue<Vector2>();
}
