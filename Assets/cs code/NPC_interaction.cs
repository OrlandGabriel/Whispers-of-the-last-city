using UnityEngine;

public class NPC_interaction : MonoBehaviour
{
    bool mother_detection = false;

    void Update()
    {
        if (mother_detection && Input.GetKeyDown(KeyCode.F))
        {
            print("Hello my son, you are sus");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "PL")
        {
            mother_detection = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        mother_detection = false;
    }
}
