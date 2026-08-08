using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManager : MonoBehaviour
{
    public void MainMenuScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }

    public void LevelSelection()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level Selection");
    }

    public void TutorialScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
    }

    public void DockSideScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level 1 Dockside");
    }
    public void AlleyWayScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level 2 Alleyway After Hours");
    }
    public void ConcreteJungleParkScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level 3 Concrete Jungle Park");
    }
    public void FlooderdShipyard()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level 4 Flooded Shipyard");
    }
}
