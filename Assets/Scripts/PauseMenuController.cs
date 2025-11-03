using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public void GoToPauseMenu()
    {
        // Save which scene we came from
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("LastPlayedScene", currentScene);
        PlayerPrefs.Save();
        
        Debug.Log("Pausing from scene: " + currentScene);
        
        // Pause and load pause menu on top
        Time.timeScale = 0f;
        SceneManager.LoadScene("Pause", LoadSceneMode.Additive);
    }
}