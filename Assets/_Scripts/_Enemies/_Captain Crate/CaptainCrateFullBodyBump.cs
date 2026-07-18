using UnityEngine;

public class CaptainCrateFullBodyBump : StateMachineBehaviour
{
    public float speed;

    CaptainCrate captainCrate;
    Transform boss;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        captainCrate = animator.GetComponent<CaptainCrate>();
        boss = animator.GetComponent<Transform>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss.position.x != captainCrate.leftPoint.position.x &&
            boss.position.x != captainCrate.rightPoint.position.x)
        {
            // Go to the right side
            if (captainCrate.isFlipped)
            {
                Vector3 target = new Vector3(captainCrate.rightPoint.position.x, boss.position.y, boss.position.z);

                boss.position = Vector3.MoveTowards(boss.position, target, speed * Time.deltaTime);
            }

            // Go to left
            else if(!captainCrate.isFlipped)
            {
                Vector3 target = new Vector3(captainCrate.leftPoint.position.x, boss.position.y, boss.position.z);

                boss.position = Vector3.MoveTowards(boss.position, target, speed * Time.deltaTime);
            }
        }
        else if(boss.position.x == captainCrate.leftPoint.position.x ||
                boss.position.x == captainCrate.rightPoint.position.x)
        {
            animator.SetBool("FullBodyBump_b", false);
        }

        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
