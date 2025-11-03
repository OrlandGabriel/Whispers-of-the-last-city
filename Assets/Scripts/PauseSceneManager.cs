using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseSceneManager : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button exitButton;
    
    [SerializeField] private GameObject exitConfirmationPanel;
    [SerializeField] private Button confirmExitButton;
    [SerializeField] private Button cancelExitButton;

    void Start()
    {
        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(false);

        // You can keep these listeners in code if you want
        // OR remove them and hook up manually in Inspector
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);

        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGameClicked);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);

        if (confirmExitButton != null)
            confirmExitButton.onClick.AddListener(OnConfirmExitClicked);

        if (cancelExitButton != null)
            cancelExitButton.onClick.AddListener(OnCancelExitButton);
    }

    // Make these PUBLIC so they appear in Inspector
    public void OnContinueClicked()
    {
        Debug.Log("Continue clicked - unpausing game");
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync("Pause");
    }

    public void OnNewGameClicked()
    {
        Debug.Log("New Game clicked - loading Level 1");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level 1");
    }

    public void OnExitClicked()
    {
        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(true);
        else
            GoToMainMenu();
    }

    public void OnConfirmExitClicked()
    {
        GoToMainMenu();
    }

    public void OnCancelExitButton()
    {
        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(false);
    }

    void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main menu");
    }
}