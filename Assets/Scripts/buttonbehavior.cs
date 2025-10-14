using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonbehavior : MonoBehaviour
{
    [SerializeField] private string newGameLevel = "Level 1";
    public void StartButton() 
    {
        SceneManager.LoadScene(newGameLevel);
    } 
    
    public void ExitButton() 
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }
    
    public void GoToPauseMenu()
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene("Pause");
    }
}