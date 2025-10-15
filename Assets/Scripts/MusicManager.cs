using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    
    void Awake()
    {
        Debug.Log("MusicManager Awake called!");
        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("Music set to DontDestroyOnLoad!");
        }
        else if (instance != this)
        {
            Debug.Log("Duplicate music manager found, destroying...");
            Destroy(gameObject);
        }
    }
}