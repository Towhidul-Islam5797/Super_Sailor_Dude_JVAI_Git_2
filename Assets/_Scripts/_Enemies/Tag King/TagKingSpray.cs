using UnityEngine;

public class TagKingSpray : StateMachineBehaviour
{
    private TagKing tagKing;
    public float sprayRange = 3f;

    override public void OnStateEnter(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        tagKing = animator.GetComponent<TagKing>();

        // Enable spray collider when spraying starts
        tagKing.EnableSprayCollider();
    }

    override public void OnStateUpdate(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (tagKing == null) return;

        // Keep facing player while spraying
        tagKing.LookAtPlayer();

        var player = tagKing.GetCurrentTarget();
        var boss = animator.transform;

        if (player == null) return;

        // If player runs away go back to walking
        if (Mathf.Abs(player.position.x - boss.position.x) >= sprayRange)
        {
            animator.SetBool("Spray_b", false);
            animator.SetBool("Walk_b", true);
        }
    }

    override public void OnStateExit(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Disable spray collider when spraying stops
        tagKing.DisableSprayCollider();
        animator.SetBool("Spray_b", false);
    }
}