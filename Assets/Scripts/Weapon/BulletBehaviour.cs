using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [Header("General Stats")]
    [SerializeField] private LayerMask bulletDestroyLayer;
    [SerializeField] private float destroyTime = 3f;

    [Header("Normal Bullet Stats")]
    public float normalBulletSpeed = 15f;
    public float normalBulletDamage = 1f;

    [Header("Physics Bullet Stats")]
    [SerializeField] private float physicsBulletSpeed = 17.5f;
    [SerializeField] private float physicsBulletDamage = 3f;
    [SerializeField] private float bulletGravity = 3f;

    private Rigidbody2D _rb;
    private float _damage;

    public enum BulletType
    {
        Normal,
        Physics
    }
    public BulletType bulletType;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        // Set destroy time for if bullet doesnt hit anything
        SetDestroyTime();

        // Change rigidbody stats based on bullet type
        SetRBStats();

        InitializeBulletStats();
    }


    private void FixedUpdate()
    {
        if(bulletType == BulletType.Physics)
        {
            // Rotate bullet in direction its facing
            transform.right = _rb.linearVelocity;
        }
    }

    private void InitializeBulletStats()
    {
        if(bulletType == BulletType.Normal)
        {
            SetStraightVelocity();

            _damage = normalBulletDamage;
        }

        else if(bulletType == BulletType.Physics)
        {
            SetPhysicsVelocity();

            _damage = physicsBulletDamage;
        }
    }

    public void ModifyDamage(float amount)
    {
        _damage += amount;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Is this collision is in the bullet destroy layerMask?
        if((bulletDestroyLayer.value & (1 << collision.gameObject.layer)) > 0) {
            // Spawn particles

            // Play sound

            // Hurt enemies
            IDamageable iDamageable = collision.gameObject.GetComponent<IDamageable>();

            if(iDamageable != null)
            {
                iDamageable.Damage(_damage);
            }

            // Destroy object
            Destroy(gameObject);
        }
    }

    private void SetDestroyTime()
    {
        Destroy(gameObject, destroyTime);
    }

    private void SetRBStats()
    {
        if(bulletType == BulletType.Normal)
        {
            _rb.gravityScale = 0f;
        }
        else if(bulletType == BulletType.Physics)
        {
            _rb.gravityScale = bulletGravity;
        }
    }

    private void SetStraightVelocity()
    {
        _rb.linearVelocity = transform.right * normalBulletSpeed;
    }

    private void SetPhysicsVelocity()
    {
        _rb.linearVelocity = transform.right * physicsBulletSpeed;
    }

}
