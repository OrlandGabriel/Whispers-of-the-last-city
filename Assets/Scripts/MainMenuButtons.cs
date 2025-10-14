using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] private string newGameLevel = "Level 1";
    [SerializeField] private GameObject exitConfirmationPanel;
    [SerializeField] private GameObject optionsPanel;
    
    public void NewGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(newGameLevel);
    }
    
    public void ContinueGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(newGameLevel);
    }
    
    public void Options()
    {
        optionsPanel.SetActive(true);
    }
    
    public void ExitGame()
    {
        exitConfirmationPanel.SetActive(true);
    }
    
    public void ConfirmExit()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }
    
    public void CancelExit()
    {
        exitConfirmationPanel.SetActive(false);
    }
}