using UnityEngine;

public class FallilngCrateTrigger : MonoBehaviour
{
    [SerializeField] private FallingCrateSpawner spawner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBody"))
        {
            spawner.StartSpawning();
        }
    }
}