using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseSceneManager : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;
    
    [Header("Exit Confirmation Panel")]
    [SerializeField] private GameObject exitConfirmationPanel;
    [SerializeField] private Button confirmExitButton;
    [SerializeField] private Button cancelExitButton;
    
    void Start()
    {
        // Make sure the exit confirmation panel is hidden at start
        if (exitConfirmationPanel != null)
        {
            exitConfirmationPanel.SetActive(false);
        }
        
        // Set up button listeners
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);
            
        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGameClicked);
            
        if (optionsButton != null)
            optionsButton.onClick.AddListener(OnOptionsClicked);
            
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);
            
        // Set up exit confirmation buttons
        if (confirmExitButton != null)
            confirmExitButton.onClick.AddListener(OnConfirmExitClicked);
            
        if (cancelExitButton != null)
            cancelExitButton.onClick.AddListener(OnCancelExitClicked);
    }
    
    void OnContinueClicked()
    {
        // Resume game time
        Time.timeScale = 1f;
        
        // Unload the pause scene
        SceneManager.UnloadSceneAsync("Pause");
    }
    
    void OnNewGameClicked()
    {
        // Resume time first
        Time.timeScale = 1f;
        
        // Load Level 1 or your starting scene
        SceneManager.LoadScene("Level 1");
    }
    
    void OnOptionsClicked()
    {
        // Load options menu scene
        // You can implement this based on your needs
        Debug.Log("Options clicked - implement your options menu here");
    }
    
    void OnExitClicked()
    {
        // Show the exit confirmation panel instead of immediately exiting
        if (exitConfirmationPanel != null)
        {
            exitConfirmationPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Exit confirmation panel not assigned! Going directly to main menu.");
            GoToMainMenu();
        }
    }
    
    void OnConfirmExitClicked()
    {
        // User confirmed they want to exit
        GoToMainMenu();
    }
    
    void OnCancelExitClicked()
    {
        // User canceled, hide the confirmation panel
        if (exitConfirmationPanel != null)
        {
            exitConfirmationPanel.SetActive(false);
        }
    }
    
    void GoToMainMenu()
    {
        // Resume time
        Time.timeScale = 1f;
        
        // Load main menu scene
        SceneManager.LoadScene("Main menu");
    }
}