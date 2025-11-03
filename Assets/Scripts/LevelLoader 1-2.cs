using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader1_2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void OnTriggerEnter (Collider other)
    {
        if (other.tag == "LevelExit")    
        {
            SceneManager.LoadScene("Level 2");
        }
    }    
}   