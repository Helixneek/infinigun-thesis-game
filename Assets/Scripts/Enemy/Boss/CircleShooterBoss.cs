using UnityEngine;
using UnityEngine.UIElements;

public class CircleShooterBoss : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] bulletSpawnPoints;
    [SerializeField] private BulletBehaviour bullet;

    [Header("Settings")]
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float bulletSpeed;

    [Space]
    [SerializeField] private float rotationSpeed = 10f;

    private float _shotTimer = 0f;
    private float _currentVariance = 0f;
    private bool _isShooting = false;

    private void Update()
    {
        RotateObject();

        _shotTimer += Time.deltaTime;

        if (_shotTimer >= fireRate && !_isShooting)
        {
            _isShooting = true;
            ShootAround();
        }
    }

    private void ShootAround()
    {
        bullet.normalBulletDamage = damage;

        for (int i = 0; i < bulletSpawnPoints.Length; i++)
        {
            var bulletInstance = Instantiate(bullet, bulletSpawnPoints[i].position, bulletSpawnPoints[i].rotation);
        }

        _isShooting = false;
        _shotTimer = 0f;
    }

    private void RotateObject()
    {
        transform.RotateAround(transform.position, new Vector3(0, 0, 1), rotationSpeed * Time.deltaTime);
    }
}
