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
        itemSprite.sprite = null;

        if(_wallet.CheckCoins(_price))
        {
            Debug.Log("You have enough money!");

            _inventory.AddUpgradeExternal(_upgrade);

            _wallet.ModifyCoins(-_price);
        }
        else
        {
            Debug.Log("Not enough money!");
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
                _price = 7;
                break;

            case 1:
                _price = 15;
                break;
        }
    }
}
