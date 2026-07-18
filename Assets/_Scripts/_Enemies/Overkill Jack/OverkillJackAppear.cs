using UnityEngine;
using DG.Tweening;

public class OverkillJackAppear : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _overkillJack;
    [SerializeField] private GameObject _healthBar;
    [SerializeField] private Collider2D _leftCollider;
    [SerializeField] private Collider2D _rightCollider;

    [Header("Entrance Settings")]
    [SerializeField] private float _startOffsetX = 10f;  // how far off screen to start
    [SerializeField] private float _enterDuration = 1.5f; // how long slide takes
    [SerializeField] private float _targetX;              // where Jack stops

    private OverkillJack _overkillJackScript;
    private Animator _animator;
    private bool _hasAppeared = false;

    private void Start()
    {
        _overkillJack.SetActive(false);
        _healthBar.SetActive(false);

        _leftCollider.enabled = false;
        _rightCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hasAppeared) return;

        if (collision.gameObject.CompareTag("PlayerBody"))
        {
            AppearOverkillJack();
        }
    }

    private void AppearOverkillJack()
    {
        _hasAppeared = true;

        // Store where Jack should stop
        _targetX = _overkillJack.transform.position.x;

        // Activate everything
        _overkillJack.SetActive(true);
        _healthBar.SetActive(true);
        _leftCollider.enabled = true;
        _rightCollider.enabled = true;

        // Get references
        _overkillJackScript = _overkillJack.GetComponent<OverkillJack>();
        _animator = _overkillJack.GetComponent<Animator>();

        // Move Jack off screen to the right before sliding in
        Vector3 startPos = _overkillJack.transform.position;
        startPos.x += _startOffsetX;
        _overkillJack.transform.position = startPos;

        // Set Walk animation while entering
        _animator.SetBool("Walk_b", true);

        // DoTween slide in from right to target position
        _overkillJack.transform.DOMoveX(_targetX, _enterDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // Arrived — stop walking, go to idle
                _animator.SetBool("Walk_b", false);
            });
    }
}