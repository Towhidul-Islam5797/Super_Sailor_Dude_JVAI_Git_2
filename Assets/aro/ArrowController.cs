using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform[] waypoints; // তীরটি যে পয়েন্টগুলো হয়ে যাবে
    public float moveSpeed = 8f;

    private int currentWaypointIndex = 0;
    private bool isMoving = false;

    void Update()
    {
        if (isMoving && currentWaypointIndex < waypoints.Length)
        {
            // বর্তমান টার্গেট পয়েন্ট সেট করা
            Transform targetPoint = waypoints[currentWaypointIndex];

            // পয়েন্টের দিকে মুভ করা
            transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

            // তীরের মুখ পথের দিকে ঘোরানো (Rotation)
            Vector3 direction = targetPoint.position - transform.position;
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // স্প্রাইটের উপর ভিত্তি করে -90 বদলাতে হতে পারে
            }

            // পয়েন্টে পৌঁছে গেলে পরবর্তী পয়েন্ট টার্গেট করা
            if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
        // সব পয়েন্ট পার হয়ে গেলে (Screen Escape)
        else if (isMoving && currentWaypointIndex >= waypoints.Length)
        {
            LevelManager manager = FindObjectOfType<LevelManager>();
            if (manager != null)
            {
                manager.ArrowEscaped();
            }
            Destroy(gameObject);
        }
    }

    private void OnMouseDown()
    {
        if (!isMoving)
        {
            isMoving = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // অন্য তীরের সাথে বা পথে বাধা পেলে থেমে যাবে
        if (collision.CompareTag("Arrow") || collision.CompareTag("Wall"))
        {
            isMoving = false;
        }
    }
}