using UnityEngine;
using DG.Tweening;

public class TagKingAppear : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _tagKing;
    [SerializeField] private GameObject _healthBar;
    [SerializeField] private Collider2D _leftCollider;
    [SerializeField] private Collider2D _rightCollider;

    [Header("Entrance Settings")]
    [SerializeField] private float _startOffsetX = 10f;
    [SerializeField] private float _enterDuration = 1.5f;
    private float _targetX;

    private TagKing _tagKingScript;
    private Animator _animator;
    private bool _hasAppeared = false;

    private void Start()
    {
        _tagKing.SetActive(false);
        _healthBar.SetActive(false);
        _leftCollider.enabled = false;
        _rightCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hasAppeared) return;

        if (collision.gameObject.CompareTag("PlayerBody"))
            AppearTagKing();
    }

    private void AppearTagKing()
    {
        _hasAppeared = true;
        _targetX = _tagKing.transform.position.x;

        _tagKing.SetActive(true);
        _healthBar.SetActive(true);
        _leftCollider.enabled = true;
        _rightCollider.enabled = true;

        _tagKingScript = _tagKing.GetComponent<TagKing>();
        _animator = _tagKing.GetComponent<Animator>();

        // Start off screen to the right
        Vector3 startPos = _tagKing.transform.position;
        startPos.x += _startOffsetX;
        _tagKing.transform.position = startPos;

        // Set Walk animation while entering
        _animator.SetBool("Walk_b", true);

        // DoTween slide in
        _tagKing.transform.DOMoveX(_targetX, _enterDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // Already walking so nothing needed here
            });
    }
}