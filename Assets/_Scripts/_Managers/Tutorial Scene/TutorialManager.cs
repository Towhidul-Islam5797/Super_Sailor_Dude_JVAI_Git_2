using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject jumpButton;
    public GameObject punchButton;
    public GameObject kickButton;

    private void Start()
    {
        jumpButton.SetActive(true);
        kickButton.SetActive(false);
        punchButton.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Punch Pointer"))
        {
            punchButton.SetActive(true);
        }
        if(collision.gameObject.CompareTag("Kick Pointer"))
        {
            kickButton.SetActive(true);
        }
    }
}
