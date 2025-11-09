using System;
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
        Locked,

        StoryItem,
        Loot,
        Box,
        Keypad
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

    [Header("Keypad Settings")]
    [SerializeField] private string correctCode = "1234";
    [SerializeField] private TriggerHandler unlockCollider;
    [SerializeField] private GameObject codeInputDisplay;
    private string currentCodeInput = "";

    [Header("Story Settings")]
    [SerializeField] private string storyText = "This is a story item.";
    [SerializeField] private GameObject storyDisplay;

    void Awake()
    {
        messageBox = GameObject.FindWithTag("Messagebox").GetComponent<TextMeshProUGUI>();
        player = GameObject.FindWithTag("Player");
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
                storyDisplay.SetActive(true);
                storyDisplay.GetComponentInChildren<TextMeshProUGUI>().text = storyText;
                storyDisplay.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    storyDisplay.SetActive(false);
                    player.GetComponent<SideScrollerController>().EnablePlayer();
                });
                player.GetComponent<SideScrollerController>().EnableUI();
                messageBox.text = "";
                break;
            case InteractionType.Loot:
                Debug.Log("Interacting with Loot");
                // TODO: give loot
                break;
            case InteractionType.Box:
                Debug.Log("Interacting with Box");
                // TODO: give box
                break;
            case InteractionType.Keypad:
                OpenKeypad();
                break;
            case InteractionType.Locked:
                Debug.Log("This door is locked.");
                messageBox.text = "This door is locked.";
                break;

        }
    }

    private void OpenKeypad()
    {
        currentCodeInput = "";
        codeInputDisplay.SetActive(true);
        messageBox.text = "";
        player.GetComponent<SideScrollerController>().EnableUI();
        // Find all Button components in children
        Button[] buttons = codeInputDisplay.GetComponentsInChildren<Button>();

        foreach (Button btn in buttons)
        {
            btn.onClick.RemoveAllListeners();
            if (btn.name == "Enter")
            {
                // Add listener for Enter button
                btn.onClick.AddListener(() =>
                {
                    if (currentCodeInput == correctCode)
                    {
                        Debug.Log("Correct code entered!");
                        messageBox.text = "Code Correct! Door Unlocked.";
                        unlockCollider.GetComponent<TriggerHandler>().type = InteractionType.Door;
                    }
                    else
                    {
                        Debug.Log("Incorrect code.");
                        messageBox.text = "Incorrect Code. Try Again.";
                    }
                    // Close keypad
                    codeInputDisplay.SetActive(false);
                    player.GetComponent<SideScrollerController>().EnablePlayer();
                });
                continue;
            }
            else if (btn.name == "Cancel")
            {
                // Add listener for Clear button
                btn.onClick.AddListener(() =>
                {
                    currentCodeInput = "";
                    codeInputDisplay.SetActive(false);
                    player.GetComponent<SideScrollerController>().EnablePlayer();
                });
                continue;
            }
            else
            {
                btn.onClick.AddListener(() => InputCharacter(btn.name));
            }

        }
    }
    public void InputCharacter(String Letter)
    {
        currentCodeInput += Letter;
        Debug.Log("Current Code Input: " + currentCodeInput);
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
        GameObject.Find("Rooms").GetComponent<RoomManager>().CheckRooms();
        GameObject.FindWithTag("MainCamera").GetComponent<CameraFollow>().CheckWall();
        messageBox.text = "";
        playerInside = false;
        player.GetComponent<SideScrollerController>().currentTrigger = null;
        yield return new WaitForSeconds(0.5f);
        fader.FadeIn();
        isTeleporting = false;
    }
}
