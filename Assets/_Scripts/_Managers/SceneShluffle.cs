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
    public void Level2Scene()
    {
        SceneManager.LoadScene("Level 2 Alleyway");
    }

    public void Level3Scene()
    {
        SceneManager.LoadScene("Level 3 Rooftop");
    }
}   
