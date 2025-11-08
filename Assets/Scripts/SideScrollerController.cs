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

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerStates = GetComponent<PlayerStates>();
    }
    private void OnTriggerEnter(Collider other)
    {
        var trigger = other.GetComponent<TriggerHandler>();
        if (trigger != null)
        {
            currentTrigger = trigger;
        }
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
        if( moveInput.magnitude !=0)
        {
        lastMoveInput = new Vector2(moveInput.x, moveInput.y);
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
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Attack Triggered "+ lastMoveInput);
            if (Physics.SphereCast(transform.position, .5f, new Vector3(lastMoveInput.x, 0, lastMoveInput.y).normalized, out RaycastHit hit, 5, enemyLayer))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Hit enemy: " + hit.collider.name);
                // Example: deal damage
                hit.collider.GetComponent<EnemyController>().takeDamage(10);
            }
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
        Vector3 move = new Vector3(moveInput.x * walkSpeed, velocity.y, moveInput.y * walkSpeed);

        // Move once per frame
        controller.Move(move * Time.deltaTime);
    }
}
