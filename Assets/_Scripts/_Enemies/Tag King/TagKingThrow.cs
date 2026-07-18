using UnityEngine;

public class TagKingThrow : StateMachineBehaviour
{
    private TagKing tagKing;

    override public void OnStateEnter(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        tagKing = animator.GetComponent<TagKing>();
    }

    override public void OnStateUpdate(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (tagKing == null) return;

        // Keep facing player while throwing
        tagKing.LookAtPlayer();
    }

    override public void OnStateExit(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Throw_t");
    }
}