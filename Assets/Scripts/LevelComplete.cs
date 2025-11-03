using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    public void CompleteLevel(int levelNumber)
    {
        int nextLevel = levelNumber + 1;

        // Unlock the next level
        PlayerPrefs.SetInt("Level" + nextLevel + "Unlocked", 1);
        PlayerPrefs.Save();

        Debug.Log("Level " + nextLevel + " unlocked!");

        // Go back to the level selection menu
        SceneManager.LoadScene("Level Scene");
    }

    // For testing - reset all progress
    public void ResetProgress()
    {
        // You can reset all levels at once
        for (int i = 2; i <= 10; i++) // change 10 to your total level count
        {
            PlayerPrefs.SetInt("Level" + i + "Unlocked", 0);
        }
        PlayerPrefs.Save();
        Debug.Log("Progress reset!");
    }
}
