using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader1_2 : MonoBehaviour
{
    private LevelComplete levelComplete;

    private void Start()
    {
        // Find LevelComplete script in the scene
        levelComplete = FindObjectOfType<LevelComplete>();
        if (levelComplete == null)
        {
            Debug.LogWarning("No LevelComplete script found in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if player reached the end of the level
        if (other.CompareTag("LevelExit"))
        {
            if (levelComplete != null)
            {
                // Mark level 1 as complete
                levelComplete.CompleteLevel(1);
            }
            else
            {
                // Fallback in case the reference is missing
                Debug.LogWarning("LevelComplete not found! Loading next level anyway...");
                SceneManager.LoadScene("Level 2");
            }
        }
    }
}
