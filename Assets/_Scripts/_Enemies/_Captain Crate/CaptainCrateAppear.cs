using UnityEngine;

public class CaptainCrateAppear : MonoBehaviour
{
    [SerializeField] private GameObject _captainCrate;
    [SerializeField] private GameObject _bodyBumpPos;
    [SerializeField] private GameObject _healthBar;
    [SerializeField] private Collider2D _leftCollider;
    [SerializeField] private Collider2D _rightCollider;



    private void Start()
    {
        _captainCrate.SetActive(false);
        _bodyBumpPos.SetActive(false);
        _healthBar.SetActive(false);

        _leftCollider.enabled = false;
        _rightCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBody"))
        {
            AppearCaptainCrate();
        }
    }
    private void AppearCaptainCrate()
    {
        _captainCrate.SetActive(true);
        _bodyBumpPos.SetActive(true);
        _healthBar.SetActive(true);

        _leftCollider.enabled = true;
        _rightCollider.enabled = true;
    }
}
