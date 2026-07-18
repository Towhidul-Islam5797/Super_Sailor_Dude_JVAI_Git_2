using UnityEngine;
using System.Collections;

public class TagKingBottle : MonoBehaviour
{
    public float speed = 6f;
    public float maxDistance = 15f;
    public float arcHeight = 2f;

    private Vector3 startPosition;
    private float direction;
    private bool initialized = false;
    private bool isDestroying = false;

    public void Init(float moveDirection)
    {
        direction = moveDirection;
        startPosition = transform.position;
        initialized = true;
    }

    private void Update()
    {
        if (!initialized || isDestroying) return;

        // Move horizontally
        transform.position += new Vector3(
            direction * speed * Time.deltaTime, 0f, 0f);

        // Add arc/parabola effect — bottle thrown in arc
        float distanceTravelled = Mathf.Abs(
            transform.position.x - startPosition.x);
        float normalizedDistance = distanceTravelled / maxDistance;

        // Arc using sine curve — goes up then comes down
        float arcY = Mathf.Sin(normalizedDistance * Mathf.PI) * arcHeight;
        transform.position = new Vector3(
            transform.position.x,
            startPosition.y + arcY,
            transform.position.z
        );

        // Rotate bottle while flying
        transform.Rotate(0f, 0f, direction * -300f * Time.deltaTime);

        // Destroy if travelled too far
        if (distanceTravelled >= maxDistance)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroying) return;

        if (collision.gameObject.CompareTag("PlayerBody"))
        {
            isDestroying = true;
            StartCoroutine(DestroyNextFrame());
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            isDestroying = true;
            StartCoroutine(DestroyNextFrame());
        }
    }

    private IEnumerator DestroyNextFrame()
    {
        yield return null;
        Destroy(gameObject);
    }
}