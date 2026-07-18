using UnityEngine;

public class FallingCrate : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            ObjectPooler.Instance.Despawn(transform);
        }
    }
}