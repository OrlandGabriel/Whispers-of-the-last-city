using UnityEngine;

public class Book : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Book initialized: " + gameObject.name);
        
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Debug.Log("Book has collider. IsTrigger: " + col.isTrigger);
        }
        else
        {
            Debug.LogError("Book is missing a Collider!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something triggered the book: " + other.gameObject.name);
        
        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();

        if (playerInventory != null)
        {
            Debug.Log("Player collected the book!");
            playerInventory.PagesCollected();
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Object that triggered doesn't have PlayerInventory component");
        }
    }
}