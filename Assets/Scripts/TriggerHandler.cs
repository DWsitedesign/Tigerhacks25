using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private GameObject player;
    [Header("Message Settings")]
    [SerializeField] public string messageOnEnter = "Entered Trigger";
    [Header("Choose the type of interaction")]
    [SerializeField] private InteractionType type;
    private bool playerInside = false;

    [Header("Door Settings")]
    [SerializeField] private Transform teleportLocation;
    [SerializeField] private ScreenFader fader;
    [SerializeField] private TextMeshProUGUI messageBox;
    private bool isTeleporting = false;

    void Awake()
    {
        messageBox = GameObject.FindWithTag("Messagebox").GetComponent<TextMeshProUGUI>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = true;
            Debug.Log("Player entered trigger: " + gameObject.name);
            messageBox.text = messageOnEnter;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = false;
            Debug.Log("Player exited trigger: " + gameObject.name);
            messageBox.text = "";

        }
    }

    // Optional: You can have a method to perform the action
    public void Interact()
    {
        if (!playerInside || isTeleporting) return;
        switch (type)
        {
            case InteractionType.Door:
                Debug.Log("Interacting with Door");
                StartCoroutine(TeleportPlayer());
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

    private IEnumerator TeleportPlayer()
    {
        isTeleporting = true;
        fader.FadeOut();
        yield return new WaitForSeconds(1f);

        // Move player safely
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            Debug.Log("Teleporting player to: " + teleportLocation.position);
            controller.enabled = false;
            player.transform.position = teleportLocation.position;
            controller.enabled = true;
        }
        else
        {
            player.transform.position = teleportLocation.position;
        }
        GameObject.FindWithTag("MainCamera").GetComponent<CameraFollow>().CheckWall();
        messageBox.text = "";
        playerInside = false;
        player.GetComponent<SideScrollerController>().currentTrigger = null;
        yield return new WaitForSeconds(0.5f);
        fader.FadeIn();
        isTeleporting = false;
    }
}
