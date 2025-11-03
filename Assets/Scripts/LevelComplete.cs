using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    // Call this when the player finishes a level (e.g., from a "Finish" trigger or button)
    public void CompleteLevel(int levelNumber)
    {
        int nextLevel = levelNumber + 1;

        // Get the currently saved unlocked level (defaults to 1 if none exists)
        int highestUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // If the player just reached a new highest level, save it
        if (nextLevel > highestUnlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
            PlayerPrefs.Save(); // 🔒 Permanently save so it stays even after exiting
            Debug.Log("Progress saved! Level " + nextLevel + " unlocked permanently.");
        }

        // Optional: also store an individual flag (for button-based unlock checks)
        PlayerPrefs.SetInt("Level" + nextLevel + "Unlocked", 1);
        PlayerPrefs.Save();

        // Go back to the level selection screen
        if (Application.CanStreamedLevelBeLoaded("Level Scene"))
        {
            SceneManager.LoadScene("Level Scene");
        }
        else
        {
            Debug.LogWarning("Scene 'Level Scene' not found in Build Settings!");
        }
    }

    // Optional: reset progress manually for testing
    public void ResetProgress()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        for (int i = 2; i <= 10; i++)
        {
            PlayerPrefs.SetInt("Level" + i + "Unlocked", 0);
        }
        PlayerPrefs.Save();
        Debug.Log("Progress reset! Only Level 1 unlocked.");
    }
}
