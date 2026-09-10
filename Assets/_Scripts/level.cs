using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // ১. এই লাইনটি থাকা আবশ্যক

public class LevelMenuManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public Button levelButton;
        public GameObject lockImage;
        public GameObject unlockImage;
        public string sceneName;
    }

    [Header("Level List")]
    public LevelData[] levels;

    void Start()
    {
        int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);

        for (int i = 0; i < levels.Length; i++)
        {
            int levelNumber = i + 1;

            // Lambda Capture Issue এড়াতে Local Variable তৈরি
            string sceneToLoad = levels[i].sceneName;

            if (levelNumber <= reachedLevel)
            {
                // Level Unlocked
                levels[i].lockImage.SetActive(false);
                levels[i].unlockImage.SetActive(true);
                levels[i].levelButton.interactable = true;

                levels[i].levelButton.onClick.RemoveAllListeners();
                levels[i].levelButton.onClick.AddListener(() => OpenLevel(sceneToLoad));
            }
            else
            {
                // Level Locked
                levels[i].lockImage.SetActive(true);
                levels[i].unlockImage.SetActive(false);
                levels[i].levelButton.interactable = false;
            }
        }
    }

    // ২. নিশ্চিত করুন এই ফাংশনটি Class-এর ভেতরেই আছে
    void OpenLevel(string sceneName)
    {
        Debug.Log("Loading Scene: " + sceneName); // ক্লিক কাজ করছে কিনা চেক করার জন্য
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

}