using UnityEngine;

public class DockRat : MonoBehaviour
{
    [Header("Visual")]
    public Animator animator;
    public Transform ratBody;

    [Header("Checkpoints & Speed")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    [Header("Attack Settings")]
    public string playerTag = "Player";
    public float attackRange = 1.5f;        // প্লেয়ার কতটা কাছে আসলে অ্যাটাক করবে
    public float attackCooldown = 1.5f;     // অ্যাটাকের বিরতি
    private Transform player;
    private float lastAttackTime;

    // Components
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;

    private Transform currentTarget;
    public bool isFlipped = false;
    private bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (pointB != null)
        {
            currentTarget = pointB;
        }
    }

    private void Update()
    {
        if (isDead) return;

        // প্লেয়ার অ্যাটাক রেঞ্জের ভেতরে থাকলে
        if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            LookAtPlayer();
            AttackPlayer();
        }
        else
        {
            MoveBetweenCheckpoints();
        }
    }

    private void LookAtPlayer()
    {
        // প্লেয়ার কোন দিকে আছে তা দেখে মুখ ঘুরিয়ে নেবে
        if ((player.position.x > transform.position.x && isFlipped) ||
            (player.position.x < transform.position.x && !isFlipped))
        {
            Flip();
        }
    }

    private void AttackPlayer()
    {
        animator.SetBool("Run_b", false);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack_t");
            lastAttackTime = Time.time;
        }
    }

    private void MoveBetweenCheckpoints()
    {
        if (pointA == null || pointB == null) return;

        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
        animator.SetBool("Run_b", true);

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            currentTarget = (currentTarget == pointB) ? pointA : pointB;
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 flipped = ratBody.localScale;
        flipped.z *= -1;
        ratBody.localScale = flipped;
        ratBody.Rotate(0f, 180f, 0f);
        isFlipped = !isFlipped;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            rb.gravityScale = 0f;
            capsuleCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Punch hit box") ||
            collision.gameObject.CompareTag("Kick hit box"))
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        animator.SetBool("Run_b", false);
        animator.SetTrigger("Die_t");

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        Destroy(gameObject, 1f);
    }
}