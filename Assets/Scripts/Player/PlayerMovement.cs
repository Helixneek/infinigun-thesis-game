using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D _rb;
    private Vector2 _moveInput;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = UserInput.MoveInput * moveSpeed;
    }

    //private void OnMove(InputValue inputValue)
    //{
    //    _moveInput = inputValue.Get<Vector2>();
    //}
}
