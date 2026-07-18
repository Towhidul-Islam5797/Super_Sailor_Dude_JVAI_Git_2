using UnityEngine;

public class JumpState : StateMachineBehaviour
{
    PlayerStateManager _stateManager;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _stateManager = animator.GetComponent<PlayerStateManager>();
        _stateManager.rb.linearVelocity = new Vector2(_stateManager.rb.linearVelocity.x, _stateManager.jumpForce);
        _stateManager.isGrounded = false;
        _stateManager.jumpPressed = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If We are grounded again and we are not moving upward, we "Land"
        /*if (_stateManager.isGrounded && _stateManager.rb.linearVelocity.y <= 0.01f)
        {
            if (Mathf.Abs(_stateManager.moveInput) > 0.1f)
            {
                animator.SetBool("run_b", true);
            }
            else
            {
                animator.SetBool("run_b",false);
            }
            
        }*/

        // Allow some horizontal control in air
        _stateManager.rb.linearVelocity = new Vector2(_stateManager.moveInput * _stateManager.moveSpeed, _stateManager.rb.linearVelocity.y);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Jump_t");
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
