using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllar : MonoBehaviour
{
    [Header("Visual & Animation")]
    public Animator animator;
    public Transform visualTransform;

    [Header("Audio")]
    public AudioSource punchAudio;
    public AudioSource jumpAudio;

    [Header("Movement & Jump")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float fallMultiplier = 2.5f;
    public bool canDoubleJump = true;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Wall Check & Force Settings")]
    public Transform leftWallCheck;
    public Transform rightWallCheck;
    public float wallCheckRadius = 0.15f;
    public LayerMask wallLayer;
    public float wallJumpForceX = 12f; // বিপরীত দিকে ধাক্কার পরিমাণ (Horizontal Impulse)
    public float wallJumpForceY = 14f; // ওপরের দিকে লাফানোর পরিমাণ (Vertical Impulse)
    public float wallJumpDuration = 0.2f; // ধাক্কা খাওয়ার পর কন্ট্রোল সাময়িক লক রাখার সময়

    [Header("Health System")]
    public int maxHealth = 100;
    public int healthToBoost = 20;
    public float damageCooldown = 0.5f;
    public HealthBar healthBar;
    public ParticleSystem healthPowerUp;

    [Header("Energy System")]
    public float energyNumber;
    public float energyToUpgrade;
    public TextMeshProUGUI energyText;
    public ParticleSystem energyPowerUp;

    [Header("Combat Settings")]
    public float attackCooldown = 0.25f;
    public int punchDamage = 10;
    public int kickDamage = 15;
    public Collider2D punchCollider;
    public Collider2D kickCollider;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public float moveInput;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isTouchingLeftWall;
    [HideInInspector] public bool isTouchingRightWall;
    [HideInInspector] public bool facingRight = true;
    [HideInInspector] public bool isHurt;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool isFrozen = false;
    [HideInInspector] public int currentHealth;

    private Vector2 vecGravity;
    private bool hasDoubleJumped = false;
    private bool punchPressed = false;
    private bool kickPressed = false;
    private float lastDamageTime = -Mathf.Infinity;
    private float lastAttackTime = -10f;
    private float wallJumpTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        vecGravity = new Vector2(0f, -Physics2D.gravity.y);
        currentHealth = maxHealth;

        if (healthBar != null) healthBar.SetMaxHealth(maxHealth);
        UpdateEnergyUI();

        DisablePunchCollider();
        DisableKickCollider();
    }

    private void Update()
    {
        if (isFrozen || isDead) return;

        HandleFlip();
        SwitchState();
    }

    private void FixedUpdate()
    {
        if (isFrozen || isDead) return;

        CheckSurroundings();

        if (isGrounded)
        {
            hasDoubleJumped = false;
        }

        // ধাক্কা খাওয়ার পর নির্দিস্ট সময় পর্যন্ত ম্যানুয়াল মুভমেন্ট লক থাকবে
        if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.fixedDeltaTime;
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        ApplyGravityMultiplication();
    }

    private void CheckSurroundings()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        if (leftWallCheck != null)
        {
            isTouchingLeftWall = Physics2D.OverlapCircle(leftWallCheck.position, wallCheckRadius, wallLayer);
        }

        if (rightWallCheck != null)
        {
            isTouchingRightWall = Physics2D.OverlapCircle(rightWallCheck.position, wallCheckRadius, wallLayer);
        }
    }

    public void SwitchState()
    {
        if (animator == null) return;

        animator.SetBool("isGrounded_b", isGrounded);
        animator.SetBool("run_b", moveInput != 0f);

        if (punchPressed)
        {
            animator.SetTrigger("Punch_t");
            punchPressed = false;
        }

        if (kickPressed)
        {
            animator.SetTrigger("Kick_t");
            kickPressed = false;
        }

        if (isHurt)
        {
            animator.SetTrigger("Hurt_t");
            isHurt = false;
        }
    }

    // =========================================================
    // HEAD STOMP & SOLID COLLISIONS
    // =========================================================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        DockRat enemy = collision.gameObject.GetComponent<DockRat>();
        if (enemy != null)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f && rb.linearVelocity.y <= 0.1f)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.8f);
                    if (jumpAudio != null) jumpAudio.Play();

                    enemy.Stomp();
                    break;
                }
            }
        }
    }

    // =========================================================
    // TRIGGER COLLISIONS (ATTACK & PICKUPS)
    // =========================================================

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleHit(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        HandleHit(collision);
    }

    private void HandleHit(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("Health"))
        {
            if (healthPowerUp != null) healthPowerUp.Play();
            currentHealth = Mathf.Min(currentHealth + healthToBoost, maxHealth);
            if (healthBar != null) healthBar.SetHealth(currentHealth);
            collision.gameObject.SetActive(false);
            return;
        }

        if (collision.CompareTag("Energy"))
        {
            if (energyPowerUp != null) energyPowerUp.Play();
            energyNumber += energyToUpgrade;
            UpdateEnergyUI();
            collision.gameObject.SetActive(false);
            return;
        }

        DockRat enemy = collision.GetComponent<DockRat>();
        if (enemy != null)
        {
            if (punchCollider != null && punchCollider.enabled)
            {
                enemy.TakeDamage(punchDamage);
                DisablePunchCollider();
            }
            else if (kickCollider != null && kickCollider.enabled)
            {
                enemy.TakeDamage(kickDamage);
                DisableKickCollider();
            }
        }
    }

    // =========================================================
    // INPUT HANDLING & JUMP LOGIC
    // =========================================================

    public void Move(InputAction.CallbackContext context)
    {
        if (isFrozen || isDead) return;
        moveInput = context.ReadValue<float>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isFrozen || isDead || !context.performed) return;

        // বাম পাশের পয়েন্ট দেওয়ালে লাগলে: ডান দিকে ফোর্স প্রয়োগ (oppositeDirection = +1)
        if (isTouchingLeftWall && !isGrounded)
        {
            ApplyWallJump(1f);
        }
        // ডান পাশের পয়েন্ট দেওয়ালে লাগলে: বাম দিকে ফোর্স প্রয়োগ (oppositeDirection = -1)
        else if (isTouchingRightWall && !isGrounded)
        {
            ApplyWallJump(-1f);
        }
        else if (isGrounded)
        {
            ApplyJump();
        }
        else if (canDoubleJump && !hasDoubleJumped)
        {
            ApplyJump();
            hasDoubleJumped = true;
        }
    }

    public void Punch(InputAction.CallbackContext context)
    {
        if (isFrozen || isDead) return;

        if (context.performed && Time.time - lastAttackTime >= attackCooldown)
        {
            punchPressed = true;
            lastAttackTime = Time.time;
            if (punchAudio != null) punchAudio.Play();
        }
    }

    public void Kick(InputAction.CallbackContext context)
    {
        if (isFrozen || isDead) return;

        if (context.performed && Time.time - lastAttackTime >= attackCooldown)
        {
            kickPressed = true;
            lastAttackTime = Time.time;
        }
    }

    // =========================================================
    // HELPER METHODS
    // =========================================================

    private void ApplyJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        if (animator != null) animator.SetTrigger("Jump_t");
        if (jumpAudio != null) jumpAudio.Play();
    }

    private void ApplyWallJump(float oppositeDirection)
    {
        // ১. পূর্বের সব গতিবেগ ক্লিয়ার করা
        rb.linearVelocity = Vector2.zero;

        // ২. দেওয়ালে স্পর্শ করার স্থানের বিপরীত অভিমুখে ইনস্ট্যান্ট ফোর্স (Impulse) প্রয়োগ
        Vector2 forceVector = new Vector2(oppositeDirection * wallJumpForceX, wallJumpForceY);
        rb.AddForce(forceVector, ForceMode2D.Impulse);

        wallJumpTimer = wallJumpDuration; // ইনপুট সাময়িক লক যাতে ধাক্কাটা মসৃণভাবে কাজ করে

        // ৩. ওয়ালের বিপরীতে মুখ ঘুরিয়ে দেওয়া
        if ((oppositeDirection > 0f && !facingRight) || (oppositeDirection < 0f && facingRight))
        {
            Flip();
        }

        if (animator != null) animator.SetTrigger("Jump_t");
        if (jumpAudio != null) jumpAudio.Play();
    }

    private void ApplyGravityMultiplication()
    {
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity -= vecGravity * fallMultiplier * Time.fixedDeltaTime;
        }
    }

    public void HandleFlip()
    {
        if (wallJumpTimer > 0f) return; // ধাক্কা খাওয়ার সময়ে হ্যান্ড ফ্লিপ স্থগিত থাকবে

        if ((moveInput > 0f && !facingRight) || (moveInput < 0f && facingRight))
        {
            Flip();
        }
    }

    public void Flip()
    {
        facingRight = !facingRight;
        if (visualTransform != null)
        {
            Vector3 scale = visualTransform.localScale;
            scale.x *= -1f;
            visualTransform.localScale = scale;
        }
    }

    // =========================================================
    // DAMAGE & DEATH LOGIC
    // =========================================================

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0) return;
        if (Time.time < lastDamageTime + damageCooldown) return;

        lastDamageTime = Time.time;
        currentHealth = Mathf.Max(currentHealth - damage, 0);

        if (healthBar != null) healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            isHurt = true;
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        moveInput = 0f;
        punchPressed = false;
        kickPressed = false;

        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetTrigger("Die_t");

        DisablePunchCollider();
        DisableKickCollider();

        Destroy(gameObject, 2f);
    }

    // =========================================================
    // ANIMATION EVENTS
    // =========================================================

    public void EnablePunchCollider()
    {
        if (!isDead && punchCollider != null) punchCollider.enabled = true;
    }

    public void DisablePunchCollider()
    {
        if (punchCollider != null) punchCollider.enabled = false;
    }

    public void EnableKickCollider()
    {
        if (!isDead && kickCollider != null) kickCollider.enabled = true;
    }

    public void DisableKickCollider()
    {
        if (kickCollider != null) kickCollider.enabled = false;
    }

    private void UpdateEnergyUI()
    {
        if (energyText != null) energyText.text = energyNumber.ToString();
    }

    public void FreezePlayer()
    {
        isFrozen = true;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
        }
        moveInput = 0f;
    }

    public void UnfreezePlayer()
    {
        isFrozen = false;
        if (rb != null) rb.gravityScale = 1f;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        Gizmos.color = Color.blue;
        if (leftWallCheck != null)
        {
            Gizmos.DrawWireSphere(leftWallCheck.position, wallCheckRadius);
        }

        if (rightWallCheck != null)
        {
            Gizmos.DrawWireSphere(rightWallCheck.position, wallCheckRadius);
        }
    }
}