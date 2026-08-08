using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerStateManager : MonoBehaviour
{
    //// State Variables
    //PlayerBaseState currentState;
    //public PlayerIdleState IdleState;
    //public PlayerRunningState RunningState;
    //public PlayerJumpingState JumpingState;
    //public PlayerPunchState PunchState;
    //public PlayerKickState KickState;

    // Components
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public PlayerInput playerInput;


    // Input Values
    [HideInInspector] public float moveInput;
    [HideInInspector] public bool jumpPressed;

    // Hurt
    [HideInInspector] public bool isHurt;
    [HideInInspector] public bool isDead = false;

    [Header("Visual")]
    public Animator animator;
    public Transform visualTransform;
    [HideInInspector] public bool facingRight = true;

    [Header("Player Audio Settings")]
    public AudioSource punchAuido;
    public AudioSource jumpAudio;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadious = 0.15f;
    public LayerMask groundLayer;

    [Header("Jump Settings")]
    public float jumpForce = 12f;
    public float fallMultiplier;
    [Space]

    [Header("Health Settings")]
    public ParticleSystem healthPowerUp;
    public int maxHealth;
    public int currentHealth;
    public int healthToBoost;
    public HealthBar healthBar;

    [Space]

    [Header("Energy Settings")]
    public ParticleSystem energyPowerUp;
    public float energyNumber;
    public float energyToUpgrade;
    public TextMeshProUGUI energyText;
    [Space]


    [HideInInspector] public Vector2 vecGravity;
    [HideInInspector] public bool isGrounded;

    [Header("Double Jump Settings")]
    public bool canDoubleJump = true;
    [HideInInspector] public bool hasDoubleJumped = false;

    [Header("Attack Cooldown Settings")]
    public float attackCooldown = 0.25f; // minimum milliseconds delay between combo presses
    private float lastAttackTime = -10f;

    #region Punch Variables
    [Header("Punch Settings")]
    public Collider2D punchCollider;
    [HideInInspector] public bool punchPressed;
    #endregion

    #region Kick Variables
    [Header("Kick Settings")]
    public Collider2D kickCollider;
    [HideInInspector] public bool kickPressed;
    #endregion



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        vecGravity = new Vector2(0, -Physics2D.gravity.y);

        // setting strings
        energyText.text = energyNumber.ToString();

        // Setting current health to maxhealth and setting healthbars max health to players maxhealth
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        punchCollider.enabled = false;
        kickCollider.enabled = false;
        hasDoubleJumped = false;
    }

    void Update()
    {
        if (isFrozen) return;

        HandleFlip();
        SwitchState();
    }

    void FixedUpdate()
    {
        // Continous Ground Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadious, groundLayer);
        if (isGrounded)
        {
            hasDoubleJumped = false;
        }

        // Prevent horizontal sliding when player is grounded and there is no input
        if (isGrounded && moveInput == 0f)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }


        GravityMultiplication();
    }

    public void SwitchState()
    {
        animator.SetBool("isGrounded_b", isGrounded);

        if (moveInput != 0)
        {
            animator.SetBool("run_b", true);
        }
        else
        {
            animator.SetBool("run_b", false);
        }
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
        if (jumpPressed == true)
        {
            if (isGrounded == true)
            {
                animator.SetTrigger("Jump_t");
                jumpPressed = false;
            }
            else if (canDoubleJump && !hasDoubleJumped)
            {
                hasDoubleJumped = true;
                animator.SetTrigger("Jump_t");
                jumpPressed = false;
            }
            else
            {
                jumpPressed = false;
            }
        }
        if (isHurt)
        {
            animator.SetTrigger("Hurt_t");
            isHurt = false;
        }
        if(currentHealth <= 0)
        {
            animator.SetTrigger("Die_t");
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Health Boost
        if (collision.gameObject.CompareTag("Health"))
        {
            healthPowerUp.Play();

            // Boosting Player Health
            currentHealth += healthToBoost;
            healthBar.SetHealth(currentHealth);

            // Deactivating The Health boost pick up
            collision.gameObject.SetActive(false);
        }

        // Energy Boost
        if (collision.gameObject.CompareTag("Energy"))
        {
            energyPowerUp.Play();
            energyText.text = (energyNumber + energyToUpgrade).ToString();

            collision.gameObject.SetActive(false);
        }

        if (collision.gameObject.CompareTag("Enemy Hit Box"))
        {
            PlayerTakeDamage(10);
        }
    }

    #region GroundCheck Debug
    private void OnDrawGizmosSelected()
    {
        if (!groundCheck)
        {
            return;
        }
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadious);
    }
    #endregion

    #region Gravity Multipliyer
    private void GravityMultiplication()
    {
        // if the character moves down we increase speed
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity -= vecGravity * fallMultiplier * Time.fixedDeltaTime;
            //Debug.Log("Gravity multiplication");
        }
    }
    #endregion

    #region Input Methodes
    // Input Methodes
    public void Move(InputAction.CallbackContext context)
    {
        if (isFrozen) return;
        moveInput = context.ReadValue<float>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // "Jump" (Button) from Player Input component
        if (context.performed)
        {
            jumpPressed = true;

        }


    }

    // Punch Input
    public void Punch(InputAction.CallbackContext context)
    {
        if (isFrozen || isDead) return;
        if (context.performed)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                punchPressed = true;
                lastAttackTime = Time.time;
            }
        }
    }

    public void Kick(InputAction.CallbackContext context)
    {
        if (isFrozen || isDead) return;
        if (context.performed)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                kickPressed = true;
                lastAttackTime = Time.time;
            }
        }
    }
    #endregion

    #region Character Flip
    public void HandleFlip()
    {
        if(isDead == false)
        {
            if (moveInput > 0 && !facingRight)
            {
                Flip();
            }
            else if (moveInput < 0 && facingRight)
            {
                Flip();
            }
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = visualTransform.localScale;
        scale.x *= -1;
        visualTransform.localScale = scale;
    }
    #endregion

    #region Health Methods
    public void PlayerTakeDamage(int damage)
    {
        if(currentHealth > 0)
        {
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
        }
        
    }

    #endregion

    #region Punch Animation Events
    public void EnablePunchCollider()
    {
        punchCollider.enabled = true;
    }

    public void DisablePunchCollider()
    {
        punchCollider.enabled = false;

    }
    #endregion

    #region Kick Animation Events
    public void EnableKickCollider()
    {
        kickCollider.enabled = true;
    }
    public void DisableKickCollider()
    {
        kickCollider.enabled = false;
    }
    #endregion

    #region Die
    public void Die()
    {
        isDead = true;
        Destroy(gameObject, 2f);
    }
    #endregion

    #region Sticker Freeze
    public bool isFrozen = false;

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
        rb.gravityScale = 1f; // restore default gravity scale
    }
    #endregion

    #region Attack Cooldown Checks
    public bool IsAttacking()
    {
        if (animator == null) return false;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Punch") || stateInfo.IsName("Kick") || stateInfo.IsName("Hurt") || stateInfo.IsName("Die") || animator.IsInTransition(0);
    }
    #endregion

}