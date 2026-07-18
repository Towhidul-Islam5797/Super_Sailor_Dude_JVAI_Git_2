using UnityEngine;

public class OverkillJackFire : StateMachineBehaviour
{
    OverkillJack overkillJack;
    public float fireRange = 5f;
    public float fireInterval = 1.5f;
    private float fireTimer;

    override public void OnStateEnter(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        overkillJack = animator.GetComponent<OverkillJack>();
        // ✅ Set timer to interval so first shot waits
        fireTimer = fireInterval;
    }

    override public void OnStateUpdate(Animator animator,
    AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (overkillJack == null) return;

        overkillJack.LookAtPlayer();

        var player = overkillJack.GetCurrentTarget();
        var boss = animator.transform;
        if (player == null) return;

        // ✅ Count down timer
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            overkillJack.FireBullet();
            fireTimer = fireInterval; // reset timer
        }

        if (Mathf.Abs(player.position.x - boss.position.x) >= fireRange)
        {
            animator.SetBool("Fire_b", false);
            animator.SetBool("Walk_b", true);
        }
    }

    override public void OnStateExit(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        overkillJack?.DisableHitBoxes();
    }
}