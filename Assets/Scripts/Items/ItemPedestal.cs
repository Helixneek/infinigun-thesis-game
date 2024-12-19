using UnityEngine;

public class ItemPedestal : MonoBehaviour
{
    [SerializeField] private UpgradeList upgradeList;
    [SerializeField] private SpriteRenderer itemSprite;

    private GunUpgrade _upgrade;
    private bool isClaimed = false;

    private void Start()
    {
        AssignRandomItem();
    }

    private void AssignRandomItem()
    {
        int randomIndex = Random.Range(0, upgradeList.gunUpgrades.Count);

        _upgrade = upgradeList.gunUpgrades[randomIndex];

        itemSprite.sprite = _upgrade.sprite;
    }

    public GunUpgrade GiveUpgrade()
    {
        if (!isClaimed)
        {
            itemSprite.sprite = null;
            isClaimed = true;

            return _upgrade;
        }
        else
        {
            return null;
        }
       
    }
}
