using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    [Header("References")]
    public Button interactButton;
    public GameObject dialogueUI; // This should be your DialogueBox

    private bool playerInRange = false;

    private void Start()
    {
        interactButton.gameObject.SetActive(false);
        interactButton.onClick.AddListener(OnInteract);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactButton.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactButton.gameObject.SetActive(false);
        }
    }

    private void OnInteract()
    {
        if (playerInRange)
        {
            dialogueUI.SetActive(true); // show the dialogue box
            Time.timeScale = 0f; // optional: pause player movement
        }
    }
}