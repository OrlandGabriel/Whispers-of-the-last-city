using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader1_2 : MonoBehaviour
{
    [Header("Next Level Settings")]
    public string nextLevelName = "Level 2";
    public string levelKey = "UnlockedLevel"; // Key for saving progress

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure your player has the "Player" tag
        {
            UnlockNextLevel();
            LoadNextLevel();
        }
    }

    void UnlockNextLevel()
    {
        // Get current progress
        int unlockedLevel = PlayerPrefs.GetInt(levelKey, 1); // Default is level 1

        // If level 2 is higher than current progress, unlock it
        if (unlockedLevel < 2)
        {
            PlayerPrefs.SetInt(levelKey, 2); // Save that Level 2 is now unlocked
            PlayerPrefs.Save();
            Debug.Log("✅ Level 2 unlocked and progress saved!");
        }
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(nextLevelName);
    }
}
