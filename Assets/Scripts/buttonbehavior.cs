using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonbehavior : MonoBehaviour
{
    [SerializeField] private string newGameLevel = "level1";
  public void StartButton() {
    SceneManager.LoadScene(newGameLevel);
  } 
}
