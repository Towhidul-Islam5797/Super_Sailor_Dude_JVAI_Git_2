using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    private PlayerStateManager _stateManager;

    [Header("Damage Settings")]
    public int damageAmount = 5;         // প্রতিবারে ৫ করে হেলথ কমবে
    public float damageCooldown = 1.0f;  // ১ সেকেন্ড পরপর আঘাত কার্যকর হবে (একসাথে বার বার কমবে না)
    private float lastDamageTime;

    private void Start()
    {
        _stateManager = GetComponentInParent<PlayerStateManager>();

        if (_stateManager == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _stateManager = playerObj.GetComponent<PlayerStateManager>();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_stateManager == null) return;

        // Health Boost
        if (collision.gameObject.CompareTag("Health"))
        {
            if (_stateManager.healthPowerUp != null) _stateManager.healthPowerUp.Play();
            _stateManager.currentHealth += _stateManager.healthToBoost;
            if (_stateManager.healthBar != null) _stateManager.healthBar.SetHealth(_stateManager.currentHealth);

            collision.gameObject.SetActive(false);
        }

        // Energy Boost
        if (collision.gameObject.CompareTag("Energy"))
        {
            if (_stateManager.energyPowerUp != null) _stateManager.energyPowerUp.Play();
            if (_stateManager.energyText != null)
                _stateManager.energyText.text = (_stateManager.energyNumber + _stateManager.energyToUpgrade).ToString();

            collision.gameObject.SetActive(false);
        }

        // Enemy Hit Box-এ লাগলে ৫ ড্যামেজ দেবে (কোলেশন টাইমার সহ)
        if (collision.gameObject.CompareTag("Enemy Hit Box"))
        {
            ApplyDamage();
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            _stateManager.isHurt = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_stateManager == null) return;

        if (collision.gameObject.CompareTag("CaptainBodyBumpHit") || collision.gameObject.CompareTag("Enemy Hit Box"))
        {
            ApplyDamage();
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            _stateManager.isHurt = true;
        }
    }

    // ৫ ড্যামেজ প্রয়োগ ও টাইমার হ্যান্ডেল করার মেথড
    private void ApplyDamage()
    {
        if (Time.time >= lastDamageTime + damageCooldown)
        {
            _stateManager.PlayerTakeDamage(damageAmount); // ৫ পয়েন্ট ড্যামেজ
            lastDamageTime = Time.time;
        }
    }
}