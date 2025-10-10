using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseBehaviour : MonoBehaviour
{
    [SerializeField] private string newPauseLevel = "Pause";
    public void Pausemenu()
    {
        SceneManager.LoadScene(newPauseLevel);
    }
}
