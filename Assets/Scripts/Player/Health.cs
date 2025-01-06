using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 5f;
    [SerializeField] private Slider healthSlider;

    private float _currentHealth = 0;

    private void Start()
    {
        if(_currentHealth <= 0)
        {
            _currentHealth = maxHealth;
        }

        healthSlider.maxValue = maxHealth;
        healthSlider.value = _currentHealth;
    }

    public void HealPlayer(float healAmount)
    {
        _currentHealth += healAmount;
        healthSlider.value = _currentHealth;

        if (_currentHealth > maxHealth)
        {
            _currentHealth = maxHealth;
        }
    }

    public void Damage(float damageAmount)
    {
        _currentHealth -= damageAmount;
        healthSlider.value = _currentHealth;

        if (_currentHealth <= 0)
        {
            // Player dies
            // Start game over
            Debug.Log("GAME OVER");
            SceneManager.LoadScene("Game Over");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Heart>(out Heart heart))
        {
            _currentHealth += heart.Pickup();

            if(_currentHealth > maxHealth)
            {
                _currentHealth = maxHealth;
            }

            healthSlider.value = _currentHealth;
        }
    }
}
