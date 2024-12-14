using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerGun : MonoBehaviour
{
    [Header("Gun Stats")]
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float accuracy;
    [Tooltip("Maximum amount of ammo that can be in the gun")]
    [SerializeField] private int maxAmmoPerMagazine;
    [Tooltip("Speed in which the player reloads their gun (in seconds)")]
    [SerializeField] private float reloadSpeed;

    [Header("Gun Objects")]
    [SerializeField] private GameObject gun;
    [SerializeField] private BulletBehaviour bullet;
    [SerializeField] private Transform bulletSpawnPoint;

    [Header("Gun References")]
    [SerializeField] private PlayerUpgradeInventory playerUpgradeInventory;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI currentAmmoText;
    [SerializeField] private Slider reloadSlider;

    private BulletBehaviour bulletInstance;

    private Vector2 _worldPosition;
    private Vector2 _direction;

    private float _currentAmmo;
    //private int _maxAmmoHeld;

    private float _angle;
    private float _shotTimer;
    private float _reloadBaseDuration = 2f;
    private float _fillTime = 0;

    private bool _isReloading = false;

    private void Start()
    {
        _currentAmmo = maxAmmoPerMagazine;

        reloadSlider.gameObject.SetActive(false);

        UpdateGunStats();

        UpdateAmmoText();
    }

    private void Update()
    {
        HandleGunRotation();

        // Handle firerate cooldown
        _shotTimer += Time.deltaTime;

        HandleGunShooting();

        if(_isReloading)
        {
            //FillReloadSlider();
        }
    }

    private void HandleGunShooting()
    {
        if(UserInput.WasShootPressed && _shotTimer >= fireRate && _currentAmmo > 0)
        {
            _shotTimer = 0;
            // Spawn the bullet
            bullet.normalBulletDamage = damage;
            bulletInstance = Instantiate(bullet, bulletSpawnPoint.position, gun.transform.rotation);

            _currentAmmo--;
            UpdateAmmoText();
        }
        else if(_currentAmmo <= 0)
        {
            StartCoroutine(ReloadGun());
        }
    }

    private void HandleGunRotation()
    {
        // Get mouse position
        _worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        // Get direction gun should be pointing to
        _direction = (_worldPosition - (Vector2)gun.transform.position).normalized;

        // Rotate the right side of the gun
        // The side used is the default direction of the gun
        gun.transform.right = _direction;

        // Get gun angle in degrees
        _angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

        // Flip the gun when rotation exceeds 90 degrees
        Vector3 localScale = Vector3.one;

        if(_angle > 90 || _angle < -90)
        {
            localScale.y = -1f;
        }
        else
        {
            localScale.y = 1f;
        }

        gun.transform.localScale = localScale;
    }

    private void UpdateGunStats()
    {
        damage += playerUpgradeInventory.damageChange;
        fireRate += playerUpgradeInventory.fireRateChange;
        accuracy += playerUpgradeInventory.accuracyChange;
        maxAmmoPerMagazine += playerUpgradeInventory.maxAmmoChange;
        reloadSpeed += playerUpgradeInventory.reloadSpeedChange;
    }

    private void UpdateAmmoText()
    {
        currentAmmoText.text = "Ammo: " + _currentAmmo + " / " + maxAmmoPerMagazine;
        Debug.Log("current ammo: " + _currentAmmo);
    }

    private IEnumerator ReloadGun()
    {
        _isReloading = true;

        yield return new WaitForSeconds(reloadSpeed);

        _currentAmmo = maxAmmoPerMagazine;

        UpdateAmmoText();

        _isReloading = false;
    }

    private void FillReloadSlider()
    {
        // Handle reload bar
        reloadSlider.gameObject.SetActive(true);

        reloadSlider.value = Mathf.Lerp(0f, 1f, _fillTime);

        _fillTime += Time.deltaTime / _reloadBaseDuration;

        //yield return new WaitForSeconds(_reloadBaseDuration);

        // Turn off reload bar
        reloadSlider.gameObject.SetActive(false);
    }
}
