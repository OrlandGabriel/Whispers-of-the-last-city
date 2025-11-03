using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [Header("Dialogue Box Reference")]
    public GameObject dialogueBox;

    // This closes the dialogue box
    public void CloseDialogue()
    {
        dialogueBox.SetActive(false);
        Time.timeScale = 1f; // optional: unpause the game if you paused during dialogue
    }
}