using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class DockRat : MonoBehaviour
{
    [Header("Visual")]
    public Animator animator;
    public Transform ratBody;

    [Header("Edge Check Ray")]
    public Transform rayOrigin;
    public float length;

    // Components
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;

    public bool isFlipped = false;
    private bool isDead = false; // Flag to prevent logic override on death

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        // If dead, stop checking edges or updating animator parameters
        if (isDead) return;

        EdgeCheck();
    }

    private void EdgeCheck()
    {
        // Raycast logic
        if (Physics2D.Raycast(rayOrigin.position, -rayOrigin.up, length))
        {
            Debug.DrawRay(rayOrigin.position, -rayOrigin.up * length, Color.red);
            animator.SetBool("Run_b", true);
        }
        else
        {
            Debug.DrawRay(rayOrigin.position, -rayOrigin.up * length, Color.green);

            animator.SetBool("Run_b", false);

            // Flip logic
            Vector3 flipped = ratBody.localScale;
            flipped.z *= -1;
            ratBody.localScale = flipped;
            ratBody.Rotate(0f, 180f, 0f);
            isFlipped = !isFlipped;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return; // Don't process collisions if already dead

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

        // Force the movement parameters to false so it stops moving
        animator.SetBool("Run_b", false);
        animator.SetTrigger("Die_t");

        // Optional: Disable physics so it doesn't keep colliding
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        // Destroy after a few seconds
        Destroy(gameObject, 1f);
    }
}