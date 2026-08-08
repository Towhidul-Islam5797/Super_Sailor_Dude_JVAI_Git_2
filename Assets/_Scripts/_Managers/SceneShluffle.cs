using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneShluffle : MonoBehaviour
{
    public void MainMenuScene()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void LevelSelection()
    {
        SceneManager.LoadScene("Level Selection");
    }

    public void TutorialScene()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void DockSideScene()
    {
        SceneManager.LoadScene("Level 1 Dockside");
    }
    public void AlleyWayScene()
    {
        SceneManager.LoadScene("Level 2 Alleyway After Hours");
    }
    public void ConcreteJungleParkScene()
    {
        SceneManager.LoadScene("Level 3 Concrete Jungle Park");
    }
    public void FlooderdShipyard()
    {
        SceneManager.LoadScene("Level 4 Flooded Shipyard");
    }
}
