using UnityEngine;

public class OverkillJackWalk : StateMachineBehaviour
{
    public float speed = 2f;
    public float fireRange = 5f;

    private Transform player;
    private Transform boss;
    private OverkillJack overkillJack;

    override public void OnStateEnter(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        overkillJack = animator.GetComponent<OverkillJack>();
        boss = animator.transform;
    }

    override public void OnStateUpdate(Animator animator,
    AnimatorStateInfo stateInfo, int layerIndex)
{
    if (overkillJack == null || boss == null) return;

    player = overkillJack.GetCurrentTarget();
    if (player == null) return;

    overkillJack.LookAtPlayer();

    Vector3 target = new Vector3(player.position.x,
        boss.position.y, boss.position.z);
    boss.position = Vector3.MoveTowards(
        boss.position, target, speed * Time.deltaTime);

    float distance = Mathf.Abs(player.position.x - boss.position.x);
    
    // ✅ Add this debug line
    Debug.Log("Distance to player: " + distance + " | Fire Range: " + fireRange);

    if (distance < fireRange)
    {
        Debug.Log("Setting Fire_b true!");
        animator.SetBool("Fire_b", true);
        animator.SetBool("Walk_b", false);
    }
}

    override public void OnStateExit(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Walk_b", false);
    }
}