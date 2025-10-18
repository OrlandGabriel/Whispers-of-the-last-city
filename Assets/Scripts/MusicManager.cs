using UnityEngine;

public class MusicManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Debug.Log("=== MUSIC MANAGER: DontDestroyOnLoad called! ===");
    }
}