using TMPro;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI gemsText;

    private int coins = 0;
    private int gems = 0;

    public int Coins { get { return coins; } }
    public int Gems { get { return gems; } }

    public void ModifyCoins(int value)
    {
        coins += value;
    }

    public void ModifyGems(int value)
    {
        gems += value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Coin>(out Coin coin)) {
            coins += coin.Pickup();
        }
        else if(collision.TryGetComponent<Gem>(out Gem gem))
        {
            gems += gem.Pickup();
        }

        UpdateText();
    }

    public void UpdateText()
    {
        coinsText.text = coins.ToString();
        gemsText.text = gems.ToString();
    }

    public bool CheckCoins(int value)
    {
        if(coins >= value)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
