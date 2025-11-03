using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader2_3 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER DETECTED! Collided with: " + other.name + " | Tag: " + other.tag);
        
        // Check if it's the Player entering the exit
        if (other.CompareTag("Player") || other.name == "Player")
        {
            Debug.Log("✅ Player detected! Loading Level 3...");
            
            // Unlock Level 3
            PlayerPrefs.SetInt("Level3Unlocked", 1);
            PlayerPrefs.Save();
            Debug.Log("Level 3 unlocked and saved!");
            
            SceneManager.LoadScene("Level 3");
        }
        else
        {
            Debug.Log("❌ Not the player. Got: " + other.name);
        }
    }
}