using UnityEngine;

public class CaptainCrateRun : StateMachineBehaviour
{
    public float speed = 2.5f;
    public float attackRange;

    private Transform player;
    private Transform boss;
    private CaptainCrate captainCrate;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        captainCrate = animator.GetComponent<CaptainCrate>();
        boss = animator.transform;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (captainCrate == null || boss == null)
            return;

        player = captainCrate.GetCurrentTarget();

        if (player == null)
            return;

        captainCrate.LookAtPlayer();

        Vector3 target = new Vector3(player.position.x, boss.position.y, boss.position.z);
        boss.position = Vector3.MoveTowards(boss.position, target, speed * Time.deltaTime);

        if (Mathf.Abs(player.position.x - boss.position.x) < attackRange)
        {
            animator.SetTrigger("Attack_t");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack_t");
    }
}