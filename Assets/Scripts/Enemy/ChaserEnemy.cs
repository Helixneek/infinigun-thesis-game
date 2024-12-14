using UnityEngine;

public class ChaserEnemy : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 1.5f;

    private Rigidbody2D _rb;
    private Transform _playerTransform;

    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 moveDirection = (_playerTransform.position - transform.position).normalized;

        MoveEnemy(moveDirection * _movementSpeed);
    }

    public void MoveEnemy(Vector2 velocity)
    {
        _rb.linearVelocity = velocity;
    }
}
