using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;


public class SideScrollerController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 2f;
    [SerializeField] private float gravityScale = -9.81f;

    private CharacterController controller;
    private Vector3 moveInput;
    private Vector3 velocity;
    public TriggerHandler currentTrigger;

    private PlayerStates playerStates;
    [SerializeField] private LayerMask enemyLayer;
    private Vector2 lastMoveInput;
    [SerializeField] private SelectedItem type;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject pauseMenu;
    private Transform healthItem;
    private Transform gun;
    private Transform meleeWeapon;

    public enum SelectedItem
    {
        healthPotion,
        gun,
        meleeWeapon
    }
    private float lastAttack = 2f;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        playerStates = GetComponent<PlayerStates>();
        healthItem = transform.Find("prescription_pill_bottle");
        gun = transform.Find("gun_m4a1");
        meleeWeapon = transform.Find("Wrenchhandler");
        healthItem.gameObject.SetActive(type == SelectedItem.healthPotion);
        gun.gameObject.SetActive(type == SelectedItem.gun);
        meleeWeapon.gameObject.SetActive(type == SelectedItem.meleeWeapon);
        EnablePlayer();
    }

    public void EnableUI()
    {
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput is null!");
            return;
        }
        Debug.Log("EnableUI called. Current action map: " + playerInput.currentActionMap.name);
        playerInput.SwitchCurrentActionMap("UI");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Switched to action map: " + playerInput.currentActionMap.name);
    }

    public void EnablePlayer()
    {
        if (playerInput == null)
        {
            Debug.LogError("EnablePlayer: PlayerInput is null!");
            return;
        }
        Debug.Log("EnablePlayer called. Current action map: " + playerInput.currentActionMap.name);
        playerInput.SwitchCurrentActionMap("Gameplay");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Switched to action map: " + playerInput.currentActionMap.name);
    }
    private void OnTriggerEnter(Collider other)
    {
        var trigger = other.GetComponent<TriggerHandler>();
        if (trigger != null)
        {
            currentTrigger = trigger;
        }
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Collided with enemy: " + other.name);
            // Example: take damage
            playerStates.TakeDamage(10);
        }
        if (other.CompareTag("HealthPickup"))
        {
            playerStates.Heal(5);
            Destroy(other.gameObject);
        }
        if (other.CompareTag("AmmoPickup"))
        {
            playerStates.PickUpAmmo(10);
            Destroy(other.gameObject);
        }
    }

    public void OnPause(InputAction.CallbackContext context){
        if (!context.performed) return;
        EnableUI();
        pauseMenu.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        var trigger = other.GetComponent<TriggerHandler>();
        if (trigger != null && currentTrigger == trigger)
        {
            currentTrigger = null;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (moveInput.magnitude != 0)
        {
            lastMoveInput = new Vector2(moveInput.x, moveInput.y);
        GameObject.Find("PlayerCharacter").GetComponent<Animator>().SetBool("IsRunning", true);
        } else{
            GameObject.Find("PlayerCharacter").GetComponent<Animator>().SetBool("IsRunning", false);
        }
        // Debug.Log("Move Input: " + moveInput);
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump" + context.performed + " isGrounded: " + controller.isGrounded);
        if (context.performed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravityScale);
            Debug.Log("Jump Triggered");
        }
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && currentTrigger != null)
        {
            currentTrigger.Interact();
        }
    }
    public void SelectShortcut(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        string item = context.control.name;
        Debug.Log("Selecting shortcut: " + item);
    
        switch (item)
        {
            case "1":
                type = SelectedItem.healthPotion;
                Debug.Log("Selected Health Potion");
                break;
            case "3":
                type = SelectedItem.gun;
                Debug.Log("Selected Gun");
                break;
            case "2":
                type = SelectedItem.meleeWeapon;
                Debug.Log("Selected Melee Weapon");
                break;
            default:
                Debug.LogWarning("Unknown item selected: " + item);
                break;
        }
        healthItem.gameObject.SetActive(type == SelectedItem.healthPotion);
        gun.gameObject.SetActive(type == SelectedItem.gun);
        meleeWeapon.gameObject.SetActive(type == SelectedItem.meleeWeapon);
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector3 direction = new Vector3(lastMoveInput.x, 0, lastMoveInput.y).normalized;
            Debug.DrawRay(transform.position, direction * (5), Color.blue, 2f);
            switch (type)
            {
                case SelectedItem.healthPotion:
                    Debug.Log("Use Health Potion");
                    playerStates.UseHealthPotion(1);
                    break;
                case SelectedItem.gun:
                    if (playerStates.UseAmmo(1))
                    {
                        Debug.Log("Fired Gun");
                        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 50, enemyLayer))
                        {
                            if (hit.collider.CompareTag("Enemy"))
                            {
                                Debug.Log("Hit enemy: " + hit.collider.name);
                                // Example: deal damage
                                hit.collider.GetComponent<EnemyController>().takeDamage(20);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("No ammo to fire!");
                    }
                    break;
                case SelectedItem.meleeWeapon:
                    if (Time.time - lastAttack >= 1f)
                    {
                        
                        GameObject.Find("Wrenchhandler").GetComponentInChildren<Animation>().Play("pipewrench");
                        if (Physics.SphereCast(transform.position, .5f, direction, out RaycastHit hit, 5, enemyLayer))
                        {
                            if (hit.collider.CompareTag("Enemy"))
                            {
                                Debug.Log("Hit enemy: " + hit.collider.name);
                                // Example: deal damage
                                hit.collider.GetComponent<EnemyController>().takeDamage(10);
                            }
                        }
                        lastAttack = Time.time;
                    }
                    break;
            }
        }
    }

    void Update()
    {

        // Gravity
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
        else
            velocity.y += gravityScale * Time.deltaTime;

        // Horizontal move only (side-scroller)
        if (lastMoveInput.x >= 0)
        {
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        Vector3 move = new Vector3(moveInput.x * walkSpeed, velocity.y, moveInput.y * walkSpeed);

        // Move once per frame
        controller.Move(move * Time.deltaTime);
    }
}
