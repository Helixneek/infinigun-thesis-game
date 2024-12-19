using UnityEngine;

public class ShooterEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private BulletBehaviour bullet;

    [Header("Settings")]
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float fireRateVariance;
    [SerializeField] private float bulletSpeed;

    private Transform _playerTransform;

    private float _shotTimer = 0f;
    private float _currentVariance = 0f;
    private bool _isShooting = false;

    private void Start()
    {
        SetFireRateVariance();
    }

    private void Update()
    {
        if(_playerTransform == null)
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        _shotTimer += Time.deltaTime;

        if(_shotTimer >= fireRate + _currentVariance && !_isShooting)
        {
            _isShooting = true;
            ShootAtPlayer();
        }

        if(_playerTransform != null)
        {
            LookAt2D(_playerTransform.position);
        }
    }

    private void ShootAtPlayer()
    {
        bullet.normalBulletDamage = damage;
        //bullet.normalBulletSpeed = bulletSpeed;
        BulletBehaviour bulletInstance = Instantiate(bullet, bulletSpawnPoint.position, transform.rotation);

        SetFireRateVariance();

        _isShooting = false;
        _shotTimer = 0f;
    }

    private void LookAt2D(Vector2 target)
    {
        Vector2 current = transform.position;
        var direction = target - current;

        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void SetFireRateVariance()
    {
        _currentVariance = Random.Range(-fireRateVariance, fireRateVariance);
    }
}
