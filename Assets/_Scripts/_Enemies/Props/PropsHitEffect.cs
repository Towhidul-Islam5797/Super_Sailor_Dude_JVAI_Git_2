using UnityEngine;

public class PropsHitEffect : MonoBehaviour
{
    public float force;
    public Transform player;
    private Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.CompareTag("Punch hit box") ||
            collision.CompareTag("Kick hit box"))
        {
            // Direction from player to this object
            Vector2 direction = (transform.position - player.position).normalized;

            // Apply force away from player
            rb.AddForce(direction * force, ForceMode2D.Impulse);

        }

    }
}