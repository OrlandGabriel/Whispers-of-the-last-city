using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    /// <summary>
    /// Call this when a level is completed.
    /// Example: CompleteLevel(1) will unlock Level 2.
    /// </summary>
    public void CompleteLevel(int levelNumber)
    {
        int nextLevel = levelNumber + 1;

        // Get the current highest unlocked level (default = 1)
        int highestUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // Unlock the next level only if it's higher than current progress
        if (nextLevel > highestUnlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
            PlayerPrefs.Save();
            Debug.Log($"✅ Progress saved: Level {nextLevel} unlocked!");
        }

        // Load the Level Select screen (or next scene if desired)
        LoadLevelSelect();
    }

    /// <summary>
    /// Loads the Level Select screen safely.
    /// </summary>
    private void LoadLevelSelect()
    {
        // Change "Level Scene" to your actual level select scene name
        string levelSelectScene = "Level Scene";

        if (Application.CanStreamedLevelBeLoaded(levelSelectScene))
        {
            SceneManager.LoadScene(levelSelectScene);
        }
        else
        {
            Debug.LogWarning($"⚠️ Scene '{levelSelectScene}' not found in Build Settings!");
        }
    }

    /// <summary>
    /// Optional: resets all progress (for testing).
    /// </summary>
    public void ResetProgress()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();
        Debug.Log("🔄 Progress reset! Only Level 1 unlocked.");
    }
}
