using UnityEngine;
using UnityEngine.Events;

    public class PlayerInventory : MonoBehaviour
    {
        
        public int NumberofPages { get; private set; }

    public UnityEvent<PlayerInventory> OnPagesCollected;
        
        public void PagesCollected()
        {
        NumberofPages++;
        OnPagesCollected.Invoke(this);
        }
    }
