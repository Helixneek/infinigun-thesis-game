using UnityEngine;

public class Boss : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 20f;

    [Header("Boss Room")]
    [SerializeField] private ItemPedestal rewardItem;
    [SerializeField] private NextFloorTrigger nextFloorTrigger;
    public bool hasItem = true;
    public bool hasNextFloor = true;

    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;

        if(rewardItem == null && hasItem)
        {
            rewardItem = transform.parent.GetComponentInChildren<ItemPedestal>();
            rewardItem.gameObject.SetActive(false);
        }
        
        if(nextFloorTrigger == null && hasNextFloor)
        {
            nextFloorTrigger = transform.parent.GetComponentInChildren<NextFloorTrigger>();
            nextFloorTrigger.gameObject.SetActive(false);
        }
    }

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            // Enemy dies
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        if (hasItem)
        {
            rewardItem.gameObject.SetActive(true);
        }

        if (hasNextFloor)
        {
            nextFloorTrigger.gameObject.SetActive(true);
            nextFloorTrigger.isLocked = false;
        }

        Destroy(gameObject);
    }
}
