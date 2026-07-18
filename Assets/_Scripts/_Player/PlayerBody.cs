using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    private PlayerStateManager _stateManager;

    private void Start()
    {
        _stateManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Health Boost
        if (collision.gameObject.CompareTag("Health"))
        {
            _stateManager.healthPowerUp.Play();

            // Boosting Player Health
            _stateManager.currentHealth += _stateManager.healthToBoost;
            _stateManager.healthBar.SetHealth(_stateManager.currentHealth);

            // Deactivating The Health boost pick up
            collision.gameObject.SetActive(false);
        }

        // Energy Boost
        if (collision.gameObject.CompareTag("Energy"))
        {
            _stateManager.energyPowerUp.Play();
            _stateManager.energyText.text = (_stateManager.energyNumber + _stateManager.energyToUpgrade).ToString();

            collision.gameObject.SetActive(false);
        }

        if (collision.gameObject.CompareTag("Enemy Hit Box"))
        {
            _stateManager.PlayerTakeDamage(10);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            _stateManager.isHurt = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("CaptainBodyBumpHit"))
        {
            _stateManager.PlayerTakeDamage(10);
        }
        if (collision.gameObject.CompareTag("Enemy Hit Box"))
        {
            _stateManager.PlayerTakeDamage(10);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            _stateManager.isHurt = true;
        }
    }
}
