using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string jumpActionName = "Jump";
    [SerializeField] private string useActionName = "Use";
    [SerializeField] private string interactActionName = "Interact";
    [SerializeField] private string moveActionName = "Move";

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction useAction;
    private InputAction interactAction;

    public Vector2 MoveInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool UseTriggered { get; private set; }
    public bool InteractTriggered { get; private set; }

    public static PlayerInputHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        var actionMap = playerControls.FindActionMap(actionMapName);
        moveAction = actionMap.FindAction(moveActionName);
        jumpAction = actionMap.FindAction(jumpActionName);
        useAction = actionMap.FindAction(useActionName);
        interactAction = actionMap.FindAction(interactActionName);
        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        moveAction.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => MoveInput = Vector2.zero;

        jumpAction.performed += ctx => JumpTriggered = true;
        jumpAction.canceled += ctx => JumpTriggered = false;

        useAction.performed += ctx => UseTriggered=true;
        interactAction.performed += ctx => InteractTriggered = true;
        
        useAction.canceled += ctx => UseTriggered = false;
        interactAction.canceled += ctx => InteractTriggered = false;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        useAction.Enable();
        interactAction.Enable();
    }
    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        useAction.Disable();
        interactAction.Disable();
    }
}
