using UnityEngine;

public class Gem : MonoBehaviour, IPickup
{
    [SerializeField] private int value = 1;

    public GameObject Player { get; set; }

    public bool IsCollected { get; set; }

    public int Pickup()
    {
        if (IsCollected)
        {
            return 0;
        }
        else
        {
            IsCollected = true;
            Destroy(gameObject);
            return value;
        }
    }
}
