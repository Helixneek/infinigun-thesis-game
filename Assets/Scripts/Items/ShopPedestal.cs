using TMPro;
using UnityEngine;

public class ShopPedestal : TriggerInteractionBase
{
    [SerializeField] private UpgradeList upgradeList;
    [SerializeField] private SpriteRenderer itemSprite;
    [SerializeField] private TextMeshProUGUI upgradePrice;

    private GunUpgrade _upgrade;
    private int _price;

    private PlayerUpgradeInventory _inventory;
    private PlayerWallet _wallet;

    private bool isBought = false;

    private void Start()
    {
        AssignRandomItem();

        _inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUpgradeInventory>();
        _wallet = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerWallet>();

        // Get the prices and set the text
        SetPrice();
        upgradePrice.text = _price.ToString() + " G";
    }

    public override void Interact()
    {
        if(!isBought && _wallet.CheckCoins(_price))
        {
            Debug.Log("You have enough money!");

            itemSprite.sprite = null;

            isBought = true;

            _inventory.AddUpgradeExternal(_upgrade);

            _wallet.ModifyCoins(-_price);
        }
        else
        {
            Debug.Log("Cannot be bought");
        }
    }

    private void AssignRandomItem()
    {
        int randomIndex = Random.Range(0, upgradeList.gunUpgrades.Count);

        _upgrade = upgradeList.gunUpgrades[randomIndex];

        itemSprite.sprite = _upgrade.sprite;
    }

    private void SetPrice()
    {
        switch(_upgrade.upgradeTier)
        {
            case 0:
                _price = 15;
                break;

            case 1:
                _price = 30;
                break;
        }
    }
}
