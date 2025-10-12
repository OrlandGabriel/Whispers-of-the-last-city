using UnityEngine;
using TMPro;
public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI BookText;
    void Start()
    {
        BookText = GetComponent<TextMeshProUGUI>();
    }
    
    public void UpdateBookText(PlayerInventory PlayerInventory)
    {
        BookText.text = PlayerInventory.NumberofPages.ToString();
    }
}
