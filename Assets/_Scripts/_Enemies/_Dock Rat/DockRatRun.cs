using UnityEngine;

public class DockRatRun : StateMachineBehaviour
{

    public float speed;

    DockRat dockRat;
    Transform ratTransform;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        dockRat = animator.GetComponent<DockRat>();
        ratTransform = animator.transform;

        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Right
        if (dockRat.isFlipped)
        {
            ratTransform.Translate(ratTransform.right * speed * Time.deltaTime);

        }
        // Left
        else if(!dockRat.isFlipped)
        {
            ratTransform.Translate(ratTransform.right * -speed * Time.deltaTime);

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
