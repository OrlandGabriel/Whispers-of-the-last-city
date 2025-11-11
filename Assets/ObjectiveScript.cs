using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectiveScript : MonoBehaviour 
{
    public Text ObjectiveParentText;
    public Text ChildText1;
    public Text ChildText2;
    public Text ChildText3;
    
    bool changedonce = false;
    bool canPressF = true;
    bool sequenceComplete = false;
    bool isTextVisible = false;
    
    void Start()
    {
        // Text is visible at start (showing initial objective)
    }
    
    // Update is called once per frame
    void Update()
    {

        GameObject[] books = GameObject.FindGameObjectsWithTag("Book");

        if (books.Length == 0 )
        {
            ChildText1.text = "After collecting all pages find the entrance to the dungeon.";
            ObjectiveParentText.enabled = true;
            ChildText1.enabled = true;

            ChildText2.enabled = false;
            ChildText3.enabled = false;
            isTextVisible = true;
        }


        if (Input.GetKeyDown(KeyCode.F) && canPressF)
        {
            if (changedonce == false)
            {
                changedonce = true;

                // Start the hide/show sequence
                StartCoroutine(HideShowSequence());
            }
        }
        
        // After sequence is complete, Tab toggles text visibility
        if (Input.GetKeyDown(KeyCode.Tab) && sequenceComplete)
        {
            if (isTextVisible)
            {
                HideText();
                isTextVisible = false;
            }
            else
            {
                ShowText();
                isTextVisible = true;
            }
        }
    }
    
    IEnumerator HideShowSequence()
    {
        canPressF = false;
        
        // Hide the text immediately
        HideText();
        isTextVisible = false;
        
        // Wait 5 seconds
        yield return new WaitForSeconds(10f);
        
        // Show new objective text
        ShowText();
        ObjectiveParentText.text = "New Objective";
        ChildText1.text = "Collect all 5 Pages";
        ChildText2.text = "After collecting all pages find the entrance to the dungeon.";
        ChildText3.text = "Press Tab or Obj Bar to reveal the objectives";
        isTextVisible = true;
        
        // Wait 5 seconds (or however long you want it displayed)
        yield return new WaitForSeconds(5f);
        
        // Hide again
        HideText();
        isTextVisible = false;
        
        // Mark sequence as complete - now Tab can be used
        sequenceComplete = true;
        
        canPressF = true;
    }
    
    void ShowText()
    {
        ObjectiveParentText.enabled = true;
        ChildText1.enabled = true;
        ChildText2.enabled = true;
        ChildText3.enabled = true;
    }

    void HideText()
    {
        ObjectiveParentText.enabled = false;
        ChildText1.enabled = false;
        ChildText2.enabled = false;
        ChildText3.enabled = false;
    }

}