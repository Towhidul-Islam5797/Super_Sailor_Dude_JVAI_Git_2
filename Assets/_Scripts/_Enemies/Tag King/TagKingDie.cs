using UnityEngine;
using DG.Tweening;

public class TagKingDie : StateMachineBehaviour
{
    private TagKing tagKing;
    private Transform boss;

    [Header("Death Settings")]
    public float flipDuration = 0.15f;
    public float fadeDuration = 0.5f;
    public float delayBeforeFade = 0.6f;

    public GameObject explosionPrefab;

    private bool _hasPlayed = false;

    override public void OnStateEnter(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_hasPlayed) return;
        _hasPlayed = true;

        tagKing = animator.GetComponent<TagKing>();
        boss = animator.transform;

        // Stop all movement
        animator.SetBool("Walk_b", false);
        animator.SetBool("Spray_b", false);

        PlayDeathSequence(animator);
    }

    private void PlayDeathSequence(Animator animator)
    {
        SpriteRenderer sr = boss.GetComponentInChildren<SpriteRenderer>();

        Sequence deathSequence = DOTween.Sequence();

        // Step 1 — Flip twice quickly
        deathSequence.Append(
            tagKing.bossBody
                .DORotate(new Vector3(0f, 180f, 0f), flipDuration)
                .SetEase(Ease.InOutQuad)
                .SetRelative(true)
        );
        deathSequence.Append(
            tagKing.bossBody
                .DORotate(new Vector3(0f, 180f, 0f), flipDuration)
                .SetEase(Ease.InOutQuad)
                .SetRelative(true)
        );

        // Step 2 — Spawn explosion
        deathSequence.AppendCallback(() =>
        {
            if (explosionPrefab != null)
                GameObject.Instantiate(
                    explosionPrefab,
                    boss.position,
                    Quaternion.identity
                );
        });

        // Step 3 — Pause briefly
        deathSequence.AppendInterval(delayBeforeFade);

        // Step 4 — Fade out sprite
        if (sr != null)
        {
            deathSequence.Append(
                sr.DOFade(0f, fadeDuration)
            );
        }

        // Step 5 — Show win panel and disable
        deathSequence.AppendCallback(() =>
        {
            UIManager.Instance.ShowWinPanel();
            boss.gameObject.SetActive(false);
        });
    }

    override public void OnStateExit(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Die_t");
        animator.enabled = false;
    }
}