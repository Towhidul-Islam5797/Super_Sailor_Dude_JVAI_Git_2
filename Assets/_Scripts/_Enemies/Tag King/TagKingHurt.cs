using UnityEngine;
using DG.Tweening;

public class TagKingHurt : StateMachineBehaviour
{
    private TagKing tagKing;
    private Transform boss;

    [Header("Pushback Settings")]
    public float pushbackDistance = 1.5f;
    public float pushbackDuration = 0.2f;
    public float flipPauseDuration = 0.3f;

    override public void OnStateEnter(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        tagKing = animator.GetComponent<TagKing>();
        boss = animator.transform;

        DoPushbackSequence();
    }

    private void DoPushbackSequence()
    {
        if (tagKing == null || boss == null) return;

        Transform target = tagKing.GetCurrentTarget();
        if (target == null) return;

        float pushDirection = boss.position.x > target.position.x ? 1f : -1f;
        float targetX = boss.position.x + (pushbackDistance * pushDirection);

        Sequence hurtSequence = DOTween.Sequence();

        // Step 1 — Pushback
        hurtSequence.Append(
            boss.DOMoveX(targetX, pushbackDuration)
                .SetEase(Ease.OutQuad)
        );

        // Step 2 — Flip away from hero
        hurtSequence.AppendCallback(() =>
        {
            tagKing.FlipAway();
        });

        // Step 3 — Pause facing away
        hurtSequence.AppendInterval(flipPauseDuration);

        // Step 4 — Flip back toward hero
        hurtSequence.AppendCallback(() =>
        {
            tagKing.LookAtPlayer();
        });
    }

    override public void OnStateExit(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Hurt_t");
    }
}