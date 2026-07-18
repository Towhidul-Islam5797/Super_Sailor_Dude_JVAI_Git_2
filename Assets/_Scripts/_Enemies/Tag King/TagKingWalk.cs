using UnityEngine;

public class TagKingWalk : StateMachineBehaviour
{
    public float speed = 2.5f;
    public float sprayRange = 3f;

    private Transform player;
    private Transform boss;
    private TagKing tagKing;

    override public void OnStateEnter(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        tagKing = animator.GetComponent<TagKing>();
        boss = animator.transform;
    }

    override public void OnStateUpdate(Animator animator,
    AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (tagKing == null || boss == null) return;

        player = tagKing.GetCurrentTarget();
        if (player == null) return;

        tagKing.LookAtPlayer();

        Vector3 target = new Vector3(player.position.x,
            boss.position.y, boss.position.z);
        boss.position = Vector3.MoveTowards(
            boss.position, target, speed * Time.deltaTime);

        // Clamp within boundaries
        if (tagKing.leftBoundary != null && tagKing.rightBoundary != null)
        {
            float clampedX = Mathf.Clamp(boss.position.x, 
                tagKing.leftBoundary.position.x, 
                tagKing.rightBoundary.position.x);
            boss.position = new Vector3(clampedX, boss.position.y, boss.position.z);
        }

        if (Mathf.Abs(player.position.x - boss.position.x) < sprayRange)
        {
            animator.SetBool("Spray_b", true);
            animator.SetBool("Walk_b", false);
        }
    }

    override public void OnStateExit(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Walk_b", false);
    }
}