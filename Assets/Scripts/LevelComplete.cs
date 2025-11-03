using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    public void CompleteLevel()
    {
        // Get the current level index (based on Build Settings order)
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int nextLevel = currentLevel + 1;

        // Save the highest level the player has unlocked
        if (PlayerPrefs.GetInt("UnlockedLevel", 1) < nextLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
            PlayerPrefs.Save();
            Debug.Log("Unlocked Level " + nextLevel);
        }

        // Return to the level select scene (change to your actual scene name)
        SceneManager.LoadScene("Level Scene");
    }

    // Optional: Reset all progress (for testing)
    public void ResetProgress()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();
        Debug.Log("Progress reset. Only Level 1 unlocked.");
    }
}
