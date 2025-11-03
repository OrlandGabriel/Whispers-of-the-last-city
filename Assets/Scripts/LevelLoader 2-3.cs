using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader2_3 : MonoBehaviour
{
    private LevelComplete levelComplete;

    private void Start()
    {
        // Locate the LevelComplete script in the scene
        levelComplete = FindObjectOfType<LevelComplete>();
        if (levelComplete == null)
        {
            Debug.LogWarning("No LevelComplete script found in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player reached the exit
        if (other.CompareTag("Player"))
        {
            Debug.Log("✅ Player reached the end of Level 2!");

            if (levelComplete != null)
            {
                // Mark Level 2 as complete (unlocks Level 3)
                levelComplete.CompleteLevel(2);
            }
            else
            {
                Debug.LogWarning("LevelComplete not found! Loading next level anyway...");
                SceneManager.LoadScene("Level 3");
            }
        }
        else
        {
            Debug.Log("❌ Non-player object triggered: " + other.name);
        }
    }
}
