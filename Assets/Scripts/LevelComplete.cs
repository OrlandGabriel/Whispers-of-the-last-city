using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    // Call this function when the player completes a level
    public void CompleteLevel(int levelNumber)
    {
        int nextLevel = levelNumber + 1;

        // Get the current highest unlocked level (default is 1)
        int highestUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // Unlock the next level permanently
        if (nextLevel > highestUnlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
            PlayerPrefs.Save(); // 🔒 Saves immediately and stays even after exiting
            Debug.Log("Progress saved: Level " + nextLevel + " unlocked!");
        }

        // Optional: unlock individual flag for UI use
        PlayerPrefs.SetInt("Level" + nextLevel + "Unlocked", 1);
        PlayerPrefs.Save();

        // Return to level selection or next scene
        if (Application.CanStreamedLevelBeLoaded("Level Scene"))
        {
            SceneManager.LoadScene("Level Scene");
        }
        else
        {
            Debug.LogWarning("Scene 'Level Scene' not found in Build Settings!");
        }
    }

    // Optional: Reset progress (for testing)
    public void ResetProgress()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        for (int i = 2; i <= 10; i++) // change 10 to your max level
        {
            PlayerPrefs.SetInt("Level" + i + "Unlocked", 0);
        }
        PlayerPrefs.Save();
        Debug.Log("Progress reset! Only Level 1 unlocked.");
    }
}
