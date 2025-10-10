using UnityEngine;
using UnityEngine.SceneManagement;

public class backtomenu : MonoBehaviour
{
    [SerializeField] private string newGameLevel = "Main Menu";
  public void StartButton() {
    SceneManager.LoadScene(newGameLevel);
  } 
}
