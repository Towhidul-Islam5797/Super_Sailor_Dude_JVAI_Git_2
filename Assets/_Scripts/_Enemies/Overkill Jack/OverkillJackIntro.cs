using UnityEngine;

public class OverkillJackIntro : StateMachineBehaviour
{
    OverkillJack overkillJack;

    override public void OnStateEnter(Animator animator, 
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        overkillJack = animator.GetComponent<OverkillJack>();

        // Make sure everything is disabled during intro
        overkillJack.DisableHitBoxes();
    }

    override public void OnStateUpdate(Animator animator, 
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Face the player even during intro
        overkillJack.LookAtPlayer();
    }

    override public void OnStateExit(Animator animator, 
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Nothing needed, transitioning to Idle automatically
    }
}