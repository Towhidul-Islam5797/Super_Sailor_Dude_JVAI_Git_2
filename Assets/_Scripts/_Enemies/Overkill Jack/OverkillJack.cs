using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverkillJack : MonoBehaviour
{
    [Header("Player Targets")]
    public List<Transform> players = new List<Transform>();
    private Transform currentTarget;

    [Header("Boss Body")]
    public Transform bossBody;
    public bool isFlipped = false;

    [Header("Health Settings")]
    public int maxHealth = 250;
    public int currentHealth;
    public Slider slider;
    public HealthBar healthBar;

    [Header("Gun Settings")]
    [SerializeField] private Collider2D leftHitBox;
    [SerializeField] private Collider2D rightHitBox;

    [Header("Bullet Settings")]
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;

    private Animator animator;
    public bool isDead;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null) healthBar.SetMaxHealth(maxHealth);
        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = currentHealth;
        }

        DisableHitBoxes();
        isDead = false;

        // ✅ Add this line
        animator.SetBool("Walk_b", true);
    }

    private void Update()
    {
        if (isDead) return;

        currentTarget = GetClosestActivePlayer();

        if (currentTarget != null)
            LookAtTarget(currentTarget);
    }

    // ✅ Missing method added
    private Transform GetClosestActivePlayer()
    {
        Transform closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform player in players)
        {
            if (player == null) continue;
            if (!player.gameObject.activeInHierarchy) continue;

            float distance = Vector2.Distance(transform.position, player.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        return closestPlayer;
    }

    public Transform GetCurrentTarget()
    {
        currentTarget = GetClosestActivePlayer();
        return currentTarget;
    }

    public void LookAtPlayer()
    {
        Transform target = GetCurrentTarget();
        if (target != null)
            LookAtTarget(target);
    }

    public void LookAtTarget(Transform target)
    {
        if (target == null || bossBody == null) return;

        bool targetIsOnLeft = target.position.x < bossBody.position.x;
        bool targetIsOnRight = target.position.x > bossBody.position.x;

        if (targetIsOnLeft && isFlipped)
            FlipBoss();
        else if (targetIsOnRight && !isFlipped)
            FlipBoss();
    }

    // Flips AWAY from player (opposite of LookAtPlayer)
    public void FlipAway()
    {
        Transform target = GetCurrentTarget();
        if (target == null || bossBody == null) return;

        bool targetIsOnLeft = target.position.x < bossBody.position.x;
        bool targetIsOnRight = target.position.x > bossBody.position.x;

        // Flip opposite direction
        if (targetIsOnLeft && !isFlipped)
            FlipBoss();
        else if (targetIsOnRight && isFlipped)
            FlipBoss();
    }

    // Make FlipBoss public so OverkillJackHurt can access it
    public void FlipBoss()
    {
        isFlipped = !isFlipped;
        bossBody.Rotate(0f, 180f, 0f);
    }

    public void EnableHitBoxes()
    {
        if (leftHitBox != null) leftHitBox.enabled = true;
        if (rightHitBox != null) rightHitBox.enabled = true;
    }

    public void DisableHitBoxes()
    {
        if (leftHitBox != null) leftHitBox.enabled = false;
        if (rightHitBox != null) rightHitBox.enabled = false;
    }

    public void OverkillJackTakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null) healthBar.SetHealth(currentHealth);
        if (slider != null) slider.value = currentHealth;

        if (currentHealth <= 0)
        {
            isDead = true;
            animator.SetTrigger("Die_t");
            DisableHitBoxes();
        }
    }

        public void FireBullet()
    {
        if (bulletPrefab == null || bulletSpawnPoint == null)
        {
            Debug.Log("Bullet prefab or spawn point is null!");
            return;
        }

        GameObject bullet = Instantiate(
            bulletPrefab,
            bulletSpawnPoint.position,
            Quaternion.identity
        );

        OverkillJackBullet bulletScript = bullet.GetComponent<OverkillJackBullet>();
        if (bulletScript != null)
        {
            float dir = isFlipped ? 1f : -1f;
            Debug.Log("Firing bullet in direction: " + dir);
            bulletScript.Init(dir);
        }
        else
        {
            Debug.Log("OverkillJackBullet script not found on prefab!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Punch hit box") ||
            collision.gameObject.CompareTag("Kick hit box"))
        {
            OverkillJackTakeDamage(2);
            if (!isDead)
                animator.SetTrigger("Hurt_t");
        }
    }

    private void OnDisable()
    {
        DisableHitBoxes();
    }
}