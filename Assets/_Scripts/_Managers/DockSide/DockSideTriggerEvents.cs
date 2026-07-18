using Unity.VisualScripting;
using UnityEngine;

public class DockSideTriggerEvents : MonoBehaviour
{
    PlayerStateManager _stateManager;

    private void Start()
    {
        _stateManager = GetComponent<PlayerStateManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
