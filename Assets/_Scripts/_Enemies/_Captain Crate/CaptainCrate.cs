using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptainCrate : MonoBehaviour
{
    [Header("Player Targets")]
    public List<Transform> players = new List<Transform>();
    private Transform currentTarget;

    [Header("Boss Body")]
    public Transform bossBody;
    public bool isFlipped = false;

    [Header("Health Settings")]
    public int maxHealth = 200;
    public int currentHealth;
    public Slider slider;
    public HealthBar healthBar;

    [Header("Body Bump Settings")]
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Colliders")]
    [SerializeField] private Collider2D punchCollider;
    [SerializeField] private Collider2D bodyBumpCollider;

    private Animator animator;
    public bool isDead;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);

        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = currentHealth;
        }

        DisablePunchCollider();
        DisableBodyBumpCollider();

        isDead = false;

        StartCoroutine(BodyBumpCoroutine());
    }

    private void Update()
    {
        if (isDead) return;

        currentTarget = GetClosestActivePlayer();

        if (currentTarget != null)
        {
            LookAtTarget(currentTarget);
        }
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
        {
            LookAtTarget(target);
        }
    }

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

    public void LookAtTarget(Transform target)
    {
        if (target == null || bossBody == null) return;

        bool targetIsOnLeft = target.position.x < bossBody.position.x;
        bool targetIsOnRight = target.position.x > bossBody.position.x;

        if (targetIsOnLeft && isFlipped)
        {
            FlipBoss();
        }
        else if (targetIsOnRight && !isFlipped)
        {
            FlipBoss();
        }
    }

    private void FlipBoss()
    {
        isFlipped = !isFlipped;
        bossBody.Rotate(0f, 180f, 0f);
    }

    public void EnablePunchCollider()
    {
        if (punchCollider != null)
            punchCollider.enabled = true;
    }

    public void DisablePunchCollider()
    {
        if (punchCollider != null)
            punchCollider.enabled = false;
    }

    public void EnableBodyBumpCollider()
    {
        if (bodyBumpCollider != null)
            bodyBumpCollider.enabled = true;
    }

    public void DisableBodyBumpCollider()
    {
        if (bodyBumpCollider != null)
            bodyBumpCollider.enabled = false;
    }

    public void StopBodyBump()
    {
        if (animator != null)
            animator.SetBool("FullBodyBump_b", false);
    }

    public void SetHealth(int health)
    {
        if (slider != null)
            slider.value = health;
    }

    public void CaptainCrateTakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        if (slider != null)
            slider.value = currentHealth;

        if (currentHealth <= 0)
        {
            isDead = true;
            animator.SetTrigger("Die_t");

            DisablePunchCollider();
            DisableBodyBumpCollider();
        }
    }

    public IEnumerator BodyBumpCoroutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(2f);

            if (!isDead)
            {
                animator.SetBool("FullBodyBump_b", true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Punch hit box") ||
            collision.gameObject.CompareTag("Kick hit box"))
        {
            CaptainCrateTakeDamage(2);

            if (!isDead)
            {
                animator.SetTrigger("Hurt_t");
            }
        }
    }

    private void OnDisable()
    {
        DisablePunchCollider();
        DisableBodyBumpCollider();
    }
}