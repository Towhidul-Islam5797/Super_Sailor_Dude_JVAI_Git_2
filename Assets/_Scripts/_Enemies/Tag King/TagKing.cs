using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TagKing : MonoBehaviour
{
    [Header("Player Targets")]
    public List<Transform> players = new List<Transform>();
    private Transform currentTarget;

    [Header("Boss Body")]
    public Transform bossBody;
    public bool isFlipped = false;

    [Header("Health Settings")]
    public int maxHealth = 300;
    public int currentHealth;
    public Slider slider;
    public HealthBar healthBar;

    [Header("Attack Settings")]
    public float sprayRange = 3f;
    public float throwRange = 8f;

    [Header("Spray Settings")]
    [SerializeField] private Collider2D sprayCollider;

    [Header("Bottle Settings")]
    public Transform bottleSpawnPoint;
    public GameObject bottlePrefab;

    [Header("Throw Timer")]
    public float throwInterval = 3f;
    private float throwTimer;

    [Header("Boundary Settings")]
    public Transform leftBoundary;
    public Transform rightBoundary;

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

        DisableSprayCollider();
        isDead = false;
        throwTimer = throwInterval;

        animator.SetBool("Walk_b", true);
    }

    private void Update()
    {
        if (isDead) return;

        currentTarget = GetClosestActivePlayer();
        if (currentTarget != null)
            LookAtTarget(currentTarget);

        // Throw timer — throws bottle periodically while walking
        throwTimer -= Time.deltaTime;
        if (throwTimer <= 0f)
        {
            ThrowBottle();
            throwTimer = throwInterval;
        }
    }

    private Transform GetClosestActivePlayer()
    {
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (Transform p in players)
        {
            if (p == null || !p.gameObject.activeInHierarchy) continue;
            float d = Vector2.Distance(transform.position, p.position);
            if (d < minDist) { minDist = d; closest = p; }
        }
        return closest;
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

    public void FlipBoss()
    {
        isFlipped = !isFlipped;
        bossBody.Rotate(0f, 180f, 0f);
    }

    public void FlipAway()
    {
        Transform target = GetCurrentTarget();
        if (target == null || bossBody == null) return;

        bool targetIsOnLeft = target.position.x < bossBody.position.x;
        bool targetIsOnRight = target.position.x > bossBody.position.x;

        if (targetIsOnLeft && !isFlipped)
            FlipBoss();
        else if (targetIsOnRight && isFlipped)
            FlipBoss();
    }

    public void EnableSprayCollider()
    {
        if (sprayCollider != null)
            sprayCollider.enabled = true;
    }

    public void DisableSprayCollider()
    {
        if (sprayCollider != null)
            sprayCollider.enabled = false;
    }

    public void ThrowBottle()
    {
        if (bottlePrefab == null || bottleSpawnPoint == null) return;

        animator.SetTrigger("Throw_t");

        GameObject bottle = Instantiate(
            bottlePrefab,
            bottleSpawnPoint.position,
            Quaternion.identity
        );

        TagKingBottle bottleScript = bottle.GetComponent<TagKingBottle>();
        if (bottleScript != null)
        {
            float direction = isFlipped ? 1f : -1f;
            bottleScript.Init(direction);
        }
    }

    public void TagKingTakeDamage(int damage)
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
            DisableSprayCollider();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Punch hit box") ||
            collision.gameObject.CompareTag("Kick hit box"))
        {
            TagKingTakeDamage(2);
            if (!isDead)
                animator.SetTrigger("Hurt_t");
        }
    }

    private void OnDisable()
    {
        DisableSprayCollider();
    }
}