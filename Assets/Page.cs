using UnityEngine;

public class Page : MonoBehaviour
{
    private bool isCollected = false;
    private SlendermanGameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<SlendermanGameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in scene.");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player") && gameManager.CanCollectPages())
        {
            // Check for player input to collect page, e.g., "E" key
            if (Input.GetKeyDown(KeyCode.E))
            {
                Collect();
            }
        }
    }

    void Collect()
    {
        isCollected = true;
        gameManager.CollectPage(this);
        gameObject.SetActive(false); // Hide the page after collection
    }
}

