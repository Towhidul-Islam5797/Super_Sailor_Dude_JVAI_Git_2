using UnityEngine;

public class LevelManagers : MonoBehaviour
{
    // লেভেল শেষ হলে এই ফাংশনটি কল করুন
    public void CompleteLevel(int currentLevel)
    {
        int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);

        // যদি বর্তমান লেভেল পার করার পর নতুন লেভেল আনলক করার প্রয়োজন হয়
        if (currentLevel >= reachedLevel)
        {
            PlayerPrefs.SetInt("ReachedLevel", currentLevel + 1);
            PlayerPrefs.Save();
        }
    }
}