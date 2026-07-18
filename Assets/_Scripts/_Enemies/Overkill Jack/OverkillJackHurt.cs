using UnityEngine;
using DG.Tweening;

public class OverkillJackHurt : StateMachineBehaviour
{
    private OverkillJack overkillJack;
    private Transform boss;

    [Header("Pushback Settings")]
    public float pushbackDistance = 1.5f;
    public float pushbackDuration = 0.2f;
    public float flipPauseDuration = 0.3f;

    override public void OnStateEnter(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        overkillJack = animator.GetComponent<OverkillJack>();
        boss = animator.transform;

        DoPushbackSequence(animator);
    }

    private void DoPushbackSequence(Animator animator)
    {
        if (overkillJack == null || boss == null) return;

        // Figure out which direction to push back
        // Push AWAY from the player
        Transform target = overkillJack.GetCurrentTarget();
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
            overkillJack.FlipAway();
        });

        // Step 3 — Pause facing away
        hurtSequence.AppendInterval(flipPauseDuration);

        // Step 4 — Flip back toward hero
        hurtSequence.AppendCallback(() =>
        {
            overkillJack.LookAtPlayer();
        });
    }

    override public void OnStateExit(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Hurt_t");
    }
}