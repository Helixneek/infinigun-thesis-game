using TMPro;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI gemsText;

    public int Coins = 0;
    public int Gems = 0;

    public void ModifyCoins(int value)
    {
        Coins += value;
    }

    public void ModifyGems(int value)
    {
       Gems += value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Coin>(out Coin coin)) {
            Coins += coin.Pickup();
        }
        else if(collision.TryGetComponent<Gem>(out Gem gem))
        {
            Gems += gem.Pickup();
        }

        UpdateText();
    }

    public void UpdateText()
    {
        coinsText.text = Coins.ToString();
        gemsText.text = Gems.ToString();
    }

    public bool CheckCoins(int value)
    {
        if(Coins >= value)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
