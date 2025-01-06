using System.Collections.Generic;
using UnityEngine;

public class PlayerReset : MonoBehaviour
{
    private PlayerUpgradeInventory _upgradeInventory;
    private PlayerWallet _wallet;

    private void Start()
    {
        
    }

    public void ResetValues()
    {
        if(!PlayerDataManager.Instance.isFirst) { return; }

        _upgradeInventory = GetComponent<PlayerUpgradeInventory>();
        _wallet = GetComponent<PlayerWallet>();

        Debug.Log("Wallet: " + _wallet.Coins + " / " + _wallet.Gems);
        Debug.Log("Upgrade Count: " + _upgradeInventory.upgrades.Count);

        // Reset wallet
        _wallet.Coins = 0;
        _wallet.Gems = 0;
        _wallet.UpdateText();

        // Reset upgrades
        _upgradeInventory.upgrades = new List<GunUpgrade>();
        _upgradeInventory.CalculateStats();

        PlayerDataManager.Instance.isFirst = false;
    }
}
