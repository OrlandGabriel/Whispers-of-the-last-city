using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public void GoToPauseMenu()
    {
        Time.timeScale = 0f; // Pause the game
        SceneManager.LoadScene("Pause"); // Load pause scene (replaces current scene)
    }
}