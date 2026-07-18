using Unity.Cinemachine;
using UnityEngine;


public class PlayerSelection : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private GameObject superSailor;
    [SerializeField] private GameObject ninja;
    [SerializeField] private GameObject selectionPanel;

    [Header("Cinemachine Camera")]
    [SerializeField] public CinemachineCamera camera;

    private void Start()
    {
        superSailor.SetActive(false);
        ninja.SetActive(false);
        selectionPanel.SetActive(true);
        
    }

    // Choose Super sailor
    public void Choose_SuperSailor()
    {
        superSailor.SetActive(true);
        ninja.SetActive(false);
        selectionPanel.SetActive(false);
        //Instantiate(superSailor);

        var target = camera.Target;
        target.TrackingTarget = superSailor.transform;
        camera.Target = target;
    }

    // Choose Ninja
    public void Choose_Ninja()
    {
        superSailor.SetActive(false);
        ninja.SetActive(true);
        selectionPanel.SetActive(false);
        //Instantiate(ninja);


        var target = camera.Target;
        target.TrackingTarget = ninja.transform;
        camera.Target = target;
    }

}
