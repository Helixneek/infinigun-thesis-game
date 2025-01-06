using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private SpriteRenderer enemySprite;
    [SerializeField] private Color enemyWithGemColor;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 5f;

    [Header("Drops")]
    public bool canDropCoins = true;
    public bool canDropGems = false;
    public bool canDropHearts = false;

    [Space]
    [SerializeField] private Coin coinPrefab;
    [SerializeField] private float coinSpread = 2f;
    [Range(0, 5)]
    [SerializeField] private int maxCoinsDropped = 3;
    [SerializeField] private LayerMask coinLayer;

    [Space]
    [SerializeField] private Gem gemPrefab;
    [SerializeField] private int maxGemsDropped = 1;

    [Space]
    [SerializeField] private Heart heartPrefab;
    [SerializeField] private int maxHeartsDropped = 1;

    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if(currentHealth <= 0)
        {
            // Enemy dies
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        // Drop gems
        // Is given priority
        if (canDropGems)
        {
            for (int i = 0; i < maxGemsDropped; i++)
            {
                float angle = Random.Range(0f, 2 * Mathf.PI);
                float distance = Random.Range(0f, coinSpread);
                Vector2 spawnPosition = (Vector2)transform.position + new Vector2(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance);
                Instantiate(gemPrefab, spawnPosition, Quaternion.identity);
            }
        }

        // Choose if the enemy drops either coins or hearts
        else
        {
            // Get the random number
            var weights = new (int, int)[]
            {
                   (1, 80), // Coins
                   (2, 20) // Hearts
            };
            var rng = new WeightedRNG(weights);
            int result = rng.GetRandomIntItem();

            switch(result)
            {
                case 1:
                    canDropCoins = true;
                    canDropHearts = false;
                    break;

                case 2:
                    canDropCoins = false;
                    canDropHearts = true;
                    break;
            }
            
            if(canDropCoins)
            {
                for (int i = 0; i < maxCoinsDropped; i++)
                {
                    float angle = Random.Range(0f, 2 * Mathf.PI);
                    float distance = Random.Range(0f, coinSpread);
                    Vector2 spawnPosition = (Vector2)transform.position + new Vector2(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance);
                    Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
                }
            }
            
            if(canDropHearts)
            {
                for (int i = 0; i < maxHeartsDropped; i++)
                {
                    float angle = Random.Range(0f, 2 * Mathf.PI);
                    float distance = Random.Range(0f, coinSpread);
                    Vector2 spawnPosition = (Vector2)transform.position + new Vector2(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance);
                    Instantiate(heartPrefab, spawnPosition, Quaternion.identity);
                }
            }
            
        }

        Destroy(gameObject);
    }

    private Vector2 GetSpawnPoint()
    {
        while(true)
        {
            Vector2 spawnPoint = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * coinSpread;

            int numColliders = Physics2D.OverlapCircleAll(spawnPoint, coinSpread, coinLayer).Length;

            if(numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }

    public void ChangeColor()
    {
        enemySprite.color = enemyWithGemColor;
    }
}
