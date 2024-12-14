using UnityEngine;

public interface IPickup
{
    GameObject Player { get; set; }

    bool IsCollected { get; set; }

    int Pickup();
}
