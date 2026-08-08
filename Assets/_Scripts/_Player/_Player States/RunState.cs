using UnityEngine;

public class RunState : StateMachineBehaviour
{

    public float _speed = 2f;

    Transform _player;
    PlayerStateManager _stateManager;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _player = animator.transform;
        _stateManager = animator.GetComponent<PlayerStateManager>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_stateManager.moveInput != 0)
        {
            _stateManager.rb.linearVelocity = new Vector2(_stateManager.moveInput * _stateManager.moveSpeed, _stateManager.rb.linearVelocity.y);

        }

        else if(_stateManager.moveInput == 0)
        {
            _stateManager.rb.linearVelocity = new Vector2(0f, _stateManager.rb.linearVelocity.y);
            animator.SetBool("run_b", false);
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
