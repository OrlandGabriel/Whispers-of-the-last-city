using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlendermanGameManager : MonoBehaviour
{
    public Text pageProgressText; // Assign in inspector
    public int totalPages = 5;

    private List<Page> collectedPages = new List<Page>();
    private bool canCollectPages = false;

    void Start()
    {
        pageProgressText.text = "Pages: 0/" + totalPages;
        StartCoroutine(StartDialogueThenEnableCollection());
    }

    IEnumerator StartDialogueThenEnableCollection()
    {
        // Simulate dialogue with a simple wait or replace with your dialogue system
        yield return StartCoroutine(DialogueCoroutine());

        // Enable page collection after dialogue
        canCollectPages = true;
    }

    IEnumerator DialogueCoroutine()
    {
        // Example dialogue simulation
        Debug.Log("Dialogue started...");
        yield return new WaitForSeconds(5f); // Simulate 5 seconds of dialogue
        Debug.Log("Dialogue ended.");
    }

    public bool CanCollectPages()
    {
        return canCollectPages;
    }

    public void CollectPage(Page page)
    {
        if (!collectedPages.Contains(page))
        {
            collectedPages.Add(page);
            pageProgressText.text = "Pages: " + collectedPages.Count + "/" + totalPages;

            if (collectedPages.Count >= totalPages)
            {
                Debug.Log("All pages collected! You win!");
                // You can add win logic here
            }
        }
    }
}
