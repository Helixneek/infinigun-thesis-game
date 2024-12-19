using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUpgradeInventory : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private TextMeshProUGUI upgradeName;
    [SerializeField] private TextMeshProUGUI upgradeDescription;

    [Space]
    [SerializeField] private float panelDuration = 3f;

    [Space]
    public List<GunUpgrade> upgrades;

    [Header("Upgrade Stats")]
    public float damageChange;
    public float fireRateChange;
    public float accuracyChange;
    public int maxAmmoChange;
    public float reloadSpeedChange;

    private PlayerGun _gun;

    private void Start()
    {
        _gun = GetComponent<PlayerGun>();

        CalculateStats();
    }

    public void AddUpgrade(GunUpgrade upgrade)
    {
        upgrades.Add(upgrade);

        CalculateStats();

        _gun.UpdateGunStats();
        _gun.UpdateAmmoText();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ItemPedestal>(out ItemPedestal pedestal))
        {
            GunUpgrade upgrade = pedestal.GiveUpgrade();

            if(upgrade != null)
            {
                AddUpgrade(upgrade);

                StartCoroutine(HandleUI(upgrade));
            }
            
        }
    }

    public void AddUpgradeExternal(GunUpgrade upgrade)
    {
        AddUpgrade(upgrade);

        StartCoroutine(HandleUI(upgrade));

        CalculateStats();

        _gun.UpdateGunStats();
        _gun.UpdateAmmoText();
    }

    private IEnumerator HandleUI(GunUpgrade upgrade)
    {
        upgradeName.text = upgrade.upgradeName;
        upgradeDescription.text = upgrade.flavorText;

        upgradePanel.SetActive(true);

        yield return new WaitForSeconds(panelDuration);

        upgradePanel.SetActive(false);
    }

    public void CalculateStats()
    {
        ResetStats();

        foreach (GunUpgrade upgrade in upgrades)
        {
            switch(upgrade.upgradeType)
            {
                case GunUpgradeType.Damage:
                    if(upgrade.isPercentage)
                    {
                        damageChange = damageChange * ((upgrade.upgradeValue + 100) / 100);
                    }
                    else
                    {
                        damageChange += upgrade.upgradeValue;
                    }

                    break;

                case GunUpgradeType.FireRate:
                    if (upgrade.isPercentage)
                    {
                        fireRateChange = fireRateChange * ((100 - upgrade.upgradeValue) / 100);
                    }
                    else
                    {
                        fireRateChange -= upgrade.upgradeValue;
                    }

                    break;

                case GunUpgradeType.Accuracy:
                    if (upgrade.isPercentage)
                    {
                        accuracyChange = accuracyChange * ((upgrade.upgradeValue + 100) / 100);
                    }
                    else
                    {
                        accuracyChange += upgrade.upgradeValue;
                    }

                    break;

                case GunUpgradeType.AmmoCapacity:
                    if (upgrade.isPercentage)
                    {
                        maxAmmoChange = (int)(maxAmmoChange * ((upgrade.upgradeValue + 100) / 100));
                    }
                    else
                    {
                        maxAmmoChange += (int)upgrade.upgradeValue;
                    }

                    break;

                case GunUpgradeType.ReloadSpeed:
                    if (upgrade.isPercentage)
                    {
                        reloadSpeedChange = reloadSpeedChange * ((100 - upgrade.upgradeValue) / 100);
                    }
                    else
                    {
                        reloadSpeedChange -= upgrade.upgradeValue;
                    }

                    break;
            }
        }
    }

    private void ResetStats()
    {
        damageChange = 0;
        fireRateChange = 0;
        accuracyChange = 0;
        maxAmmoChange = 0;
        reloadSpeedChange = 0;
    }
}
