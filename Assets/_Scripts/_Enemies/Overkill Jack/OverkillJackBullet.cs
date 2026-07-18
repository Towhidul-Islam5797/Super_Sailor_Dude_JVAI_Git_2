using UnityEngine;

public class OverkillJackBullet : MonoBehaviour
{
    public float speed = 8f;
    public float maxDistance = 15f;

    private Vector3 startPosition;
    private float direction;
    private bool initialized = false;

    public void Init(float moveDirection)
    {
        direction = moveDirection;
        startPosition = transform.position;
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;

        transform.position += new Vector3(direction * speed * Time.deltaTime, 0f, 0f);

        if (Vector3.Distance(transform.position, startPosition) > maxDistance)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ✅ Player body hit — PlayerBody.cs handles damage automatically
        if (collision.gameObject.CompareTag("PlayerBody"))
        {
            Destroy(gameObject);
        }

        // ✅ Destroy on ground only (removed Wall tag)
        if (collision.gameObject.CompareTag("Ground"))
            Destroy(gameObject);
    }
}