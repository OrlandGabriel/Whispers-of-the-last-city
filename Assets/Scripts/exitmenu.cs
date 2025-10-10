using UnityEngine;
public class ExitApp : MonoBehaviour
{
    
    public void ExitApplication()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
