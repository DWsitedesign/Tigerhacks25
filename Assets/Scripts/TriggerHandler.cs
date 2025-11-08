using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    public enum InteractionType
    {
        Door,
        StoryItem,
        Loot,
        Box
    }
    [Header("Trigger Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] public string messageOnEnter = "Entered Trigger";
    [Header("Choose the type of interaction")]
    [SerializeField] private InteractionType type;
    private bool playerInside = false;
   private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = true;
            Debug.Log("Player entered trigger: " + gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = false;
            Debug.Log("Player exited trigger: " + gameObject.name);
        }
    }

    // Optional: You can have a method to perform the action
    public void Interact()
    {
        switch (type)
        {
            case InteractionType.Door:
                Debug.Log("Interacting with Door");
                // TODO: open door logic
                break;
            case InteractionType.StoryItem:
                Debug.Log("Interacting with Story");
                // TODO: trigger story event
                break;
            case InteractionType.Loot:
                Debug.Log("Interacting with Loot");
                // TODO: give loot
                break;
            case InteractionType.Box:
                Debug.Log("Interacting with Box");
                // TODO: give box
                break;

        }
    }
}
