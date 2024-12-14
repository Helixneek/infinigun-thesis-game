using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    public static Vector2 MoveInput;

    public static bool WasShootPressed;
    public static bool WasInteractPressed;

    private InputAction _moveAction;
    private InputAction _shootAction;
    private InputAction _interactAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _shootAction = PlayerInput.actions["Shoot"];
        _interactAction = PlayerInput.actions["Interact"];
    }

    private void Update()
    {
        MoveInput = _moveAction.ReadValue<Vector2>();

        WasShootPressed = _shootAction.WasPressedThisFrame();
        WasInteractPressed = _interactAction.WasPressedThisFrame();
    }
}
