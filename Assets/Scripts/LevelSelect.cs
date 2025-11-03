using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private Button chapter1Button;
    [SerializeField] private Button chapter2Button;
    [SerializeField] private Button chapter3Button;

    void Start()
    {
        // Get the highest unlocked level (default = 1)
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // Always unlocked
        chapter1Button.interactable = true;

        // Unlock buttons depending on progress
        chapter2Button.interactable = (unlockedLevel >= 2);
        chapter3Button.interactable = (unlockedLevel >= 3);

        Debug.Log("Loaded level progress. Highest unlocked level: " + unlockedLevel);
    }

    // Scene loading methods
    public void LoadLevel1()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Cutscene_Level1");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level 2");
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene("Level 3");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main menu");
    }
}
