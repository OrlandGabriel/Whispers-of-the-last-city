using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    // Call this function when the player completes a level
    public void CompleteLevel(int levelNumber)
    {
        int nextLevel = levelNumber + 1;

        // Get current highest unlocked level (default is 1)
        int currentUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // Update if the player has reached a new highest level
        if (nextLevel > currentUnlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
            Debug.Log($"Saved progress: Level {nextLevel} unlocked!");
        }

        // Also store individual unlock flag (optional for menus)
        PlayerPrefs.SetInt($"Level{nextLevel}Unlocked", 1);
        PlayerPrefs.Save();

        Debug.Log($"Level {nextLevel} unlocked!");

        // Go back to the level selection scene (make sure this exists)
        if (Application.CanStreamedLevelBeLoaded("Level Scene"))
        {
            SceneManager.LoadScene("Level Scene");
        }
        else
        {
            Debug.LogWarning("Scene 'Level Scene' not found in Build Settings!");
        }
    }

    // For testing - reset all progress
    public void ResetProgress()
    {
        // Reset highest unlocked level tracker
        PlayerPrefs.SetInt("UnlockedLevel", 1);

        // Reset each level’s unlocked flag
        for (int i = 2; i <= 10; i++) // Change 10 to your total level count
        {
            PlayerPrefs.SetInt($"Level{i}Unlocked", 0);
        }

        PlayerPrefs.Save();
        Debug.Log("Progress reset! Only Level 1 unlocked.");
    }
}
