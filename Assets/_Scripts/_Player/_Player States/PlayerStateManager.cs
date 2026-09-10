using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateManager : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public PlayerInput playerInput;

    [HideInInspector] public float moveInput;
    [HideInInspector] public bool jumpPressed;
    [HideInInspector] public bool isHurt;
    [HideInInspector] public bool isDead = false;

    [Header("Visual")]
    public Animator animator;
    public Transform visualTransform;
    [HideInInspector] public bool facingRight = true;

    [Header("Player Audio Settings")]
    public AudioSource punchAudio;
    public AudioSource jumpAudio;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Jump Settings")]
    public float jumpForce = 12f;
    public float fallMultiplier = 2.5f;

    [Header("Health Settings")]
    public ParticleSystem healthPowerUp;
    public int maxHealth = 100;
    public int currentHealth;
    public int healthToBoost = 20;
    public HealthBar healthBar;

    [Header("Energy Settings")]
    public ParticleSystem energyPowerUp;
    public float energyNumber;
    public float energyToUpgrade;
    public TextMeshProUGUI energyText;

    [Header("Double Jump Settings")]
    public bool canDoubleJump = true;
    [HideInInspector] public bool hasDoubleJumped = false;

    [Header("Attack Cooldown Settings")]
    public float attackCooldown = 0.25f;
    private float lastAttackTime = -10f;

    [Header("Combat Colliders")]
    public Collider2D punchCollider;
    public Collider2D kickCollider;
    [HideInInspector] public bool punchPressed;
    [HideInInspector] public bool kickPressed;

    [HideInInspector] public Vector2 vecGravity;
    [HideInInspector] public bool isGrounded;
    public bool isFrozen = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        vecGravity = new Vector2(0, -Physics2D.gravity.y);

        currentHealth = maxHealth;
        if (healthBar != null) healthBar.SetMaxHealth(maxHealth);

        UpdateEnergyUI();

        if (punchCollider) punchCollider.enabled = false;
        if (kickCollider) kickCollider.enabled = false;
        hasDoubleJumped = false;
    }

    void Update()
    {
        if (isFrozen || isDead) return;

        HandleFlip();
        SwitchState();
    }

    void FixedUpdate()
    {
        // Ground Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // মাটিতে পা ছোঁয়া মাত্রই ডাবল জাম্প রিসেট হয়ে যাবে
        if (isGrounded)
        {
            hasDoubleJumped = false;

            // নো-ইনপুট থাকলে আনুভূমিক স্লিপ বন্ধ করা
            if (moveInput == 0f)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
        }

        GravityMultiplication();
    }
    public void SwitchState()
    {
        animator.SetBool("isGrounded_b", isGrounded);
        animator.SetBool("run_b", moveInput != 0);

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Health"))
        {
            if (healthPowerUp) healthPowerUp.Play();

            // Max Health limit check
            currentHealth = Mathf.Min(currentHealth + healthToBoost, maxHealth);
            if (healthBar != null) healthBar.SetHealth(currentHealth);

            collision.gameObject.SetActive(false);
        }

        if (collision.CompareTag("Energy"))
        {
            if (energyPowerUp) energyPowerUp.Play();
            energyNumber += energyToUpgrade;
            UpdateEnergyUI();

            collision.gameObject.SetActive(false);
        }

        if (collision.CompareTag("Enemy Hit Box"))
        {
            PlayerTakeDamage(10);
        }
    }

    private void GravityMultiplication()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity -= vecGravity * fallMultiplier * Time.fixedDeltaTime;
        }
    }

    #region Input Handlers
    public void Move(InputAction.CallbackContext context)
    {
        if (isFrozen) return;
        moveInput = context.ReadValue<float>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isFrozen || isDead) return;

        if (context.performed)
        {
            // ১ম জাম্প: গ্রাউন্ডে থাকলে
            if (isGrounded)
            {
                ApplyJump();
                hasDoubleJumped = false; // ১ম জাম্প হলে ডাবল জাম্প বাকি থাকে
            }
            // ২য় জাম্প: হাওয়ায় থাকলে এবং ডাবল জাম্প না করে থাকলে
            else if (canDoubleJump && !hasDoubleJumped)
            {
                ApplyJump();
                hasDoubleJumped = true; // ডাবল জাম্প ব্যবহার হয়ে গেল
            }
        }
    }

    private void ApplyJump()
    {
        // Y অক্ষের ভেলোসিটি রিমেট করে নতুন জাম্প ফোর্সে লাফাবে
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // জাম্প অ্যানিমেশন প্লে
        animator.SetTrigger("Jump_t");

        // জাম্প সাউন্ড (যদি থাকে)
        if (jumpAudio != null) jumpAudio.Play();
    }

    public void Punch(InputAction.CallbackContext context)
    {
        if (isFrozen || isDead) return;
        if (context.performed && Time.time - lastAttackTime >= attackCooldown)
        {
            punchPressed = true;
            lastAttackTime = Time.time;
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
    #endregion

    #region Character Logic
    public void HandleFlip()
    {
        if ((moveInput > 0 && !facingRight) || (moveInput < 0 && facingRight))
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = visualTransform.localScale;
        scale.x *= -1;
        visualTransform.localScale = scale;
    }

    public void PlayerTakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (healthBar != null) healthBar.SetHealth(currentHealth);

        isHurt = true;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            animator.SetTrigger("Die_t");
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        Destroy(gameObject, 2f);
    }

    private void UpdateEnergyUI()
    {
        if (energyText != null) energyText.text = energyNumber.ToString();
    }
    #endregion

    #region Animation Events & Helpers
    public void EnablePunchCollider() => punchCollider.enabled = true;
    public void DisablePunchCollider() => punchCollider.enabled = false;
    public void EnableKickCollider() => kickCollider.enabled = true;
    public void DisableKickCollider() => kickCollider.enabled = false;

    public void FreezePlayer()
    {
        isFrozen = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        moveInput = 0f;
        animator.SetBool("run_b", false);
    }

    public void UnfreezePlayer()
    {
        isFrozen = false;
        rb.gravityScale = 1f;
    }

    public bool IsAttacking()
    {
        if (animator == null) return false;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Punch") || stateInfo.IsName("Kick") || stateInfo.IsName("Hurt") || stateInfo.IsName("Die") || animator.IsInTransition(0);
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}