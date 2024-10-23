using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 7;
    [SerializeField] private GameObject playerBody;
    [SerializeField] private Camera playerCamera;

    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float interactDistance = 4f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Color defaultCrosshairColor = Color.white;
    [SerializeField] private Color hoveringOnInteractableCrosshairColor = Color.green;
    [SerializeField] private Light flashlight;
    [SerializeField] private AudioClip flashlightToggleSfx;

    [SerializeField] private PlayerHUD hudPrefab;

    private CharacterController characterController;
    private PlayerInputActions playerActions;
    private PlayerHUD playerHUD;

    public Inventory Inventory { get { return playerInventory; } }

    private Inventory playerInventory;

    private AudioSource playerAudioSource;

    private float xRot;

    // Head bobbing settings 
    [SerializeField] private float bobAmount = 0.1f; 
    [SerializeField] private float bobSpeed = 5f; 
    [SerializeField] private float returnSpeed = 5f; // Speed of smooth transition back to original position
    private Vector3 cameraInitialPosition;
    private Vector3 cameraTargetPosition;
    private float bobTimer = 0f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerActions = new PlayerInputActions();
        playerAudioSource = GetComponent<AudioSource>();
        playerInventory = GetComponent<Inventory>();

        //Spawn the HUD
        Canvas canvas = FindObjectOfType<Canvas>();
        playerHUD = Instantiate(hudPrefab.gameObject, canvas.transform).GetComponent<PlayerHUD>();

        xRot = 0;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set initial camera position for head bobbing
        cameraInitialPosition = playerCamera.transform.localPosition;
        cameraTargetPosition = cameraInitialPosition;
    }

    private void OnEnable()
    {
        playerActions.Enable();
        ConnectInputEvents();
    }

    private void OnDisable()
    {
        playerActions.Disable();
        DisconnectInputEvents();
    }

    private void ConnectInputEvents()
    {
       playerActions.PlayerMovement.Interact.performed += OnInteractPressed;
       playerActions.PlayerMovement.ToggleFlashlight.performed += OnFlashlightToggle;
       playerActions.UI.PauseGame.performed += PauseGame;
    }

    private void DisconnectInputEvents()
    {
        playerActions.PlayerMovement.Interact.performed -= OnInteractPressed;
        playerActions.PlayerMovement.ToggleFlashlight.performed += OnFlashlightToggle;
        playerActions.UI.PauseGame.performed -= PauseGame;
    }

    private void Update()
    {
        if (PauseManager.Instance.IsPaused)
        {
            return;
        }

        HandleMovement();
        HandleLooking();

        Color crosshairColor = IsLookingAtInteractable() ? hoveringOnInteractableCrosshairColor : defaultCrosshairColor;
        crosshairColor.a = 1f;

        playerHUD.Crosshair.color = crosshairColor;
    }

    private void OnInteractPressed(InputAction.CallbackContext context) 
    {
        Debug.DrawLine(playerCamera.transform.position, playerCamera.transform.position + (playerCamera.transform.forward * interactDistance), Color.red, 3.0f);

        if (PerformInteractionRaycast(out RaycastHit hitData))
        {
            if(hitData.transform.TryGetComponent(out IInteractable interactable))
            {
                InteractData interactData;
                interactData.interactingPlayer = this;

                interactable.Interact(interactData);
                return;
            }

            Debug.Log("Object is not interactable: " + hitData.transform.name);
        }
    }


    private void OnFlashlightToggle(InputAction.CallbackContext context)
    {
        flashlight.enabled = !flashlight.enabled;
        playerAudioSource.PlayOneShot(flashlightToggleSfx);
    }

    private void PauseGame(InputAction.CallbackContext context)
    {
        PauseManager.Instance.TogglePause();
    }

    private bool PerformInteractionRaycast(out RaycastHit hit)
    {
        return Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactDistance, interactLayer);
    }

    private bool IsLookingAtInteractable()
    {
        if (PerformInteractionRaycast(out RaycastHit hitData))
        {
            if (hitData.transform.TryGetComponent(out IInteractable interactable))
            {
                return interactable.CanBeInteractedWith();
            }
        }

        return false;
    } 

    private void HandleMovement()
    {
        Vector2 movementInput = playerActions.PlayerMovement.Movement.ReadValue<Vector2>();

        float gravityDisplacement = Physics.gravity.y;

        Vector3 movement = new Vector3(movementInput.x, gravityDisplacement, movementInput.y) * movementSpeed * Time.deltaTime;

        movement = transform.TransformVector(movement);

        characterController.Move(movement);

        // Head bobbing logic 
        if (movementInput.x != 0 || movementInput.y != 0) // If moving
        {
            bobTimer += Time.deltaTime * bobSpeed;
            float bobOffset = Mathf.Sin(bobTimer) * bobAmount;
            cameraTargetPosition = cameraInitialPosition + new Vector3(0, bobOffset, 0);
        }
        else
        {
            bobTimer = 0f; // Reset the timer when not moving
            cameraTargetPosition = cameraInitialPosition; // Smoothly transition to the original position
        }

        // Smoothly move the camera to the target position
        playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, cameraTargetPosition, Time.deltaTime * returnSpeed);
    }

    private void HandleLooking()
    {
        Vector2 lookInput = playerActions.PlayerMovement.Look.ReadValue<Vector2>();

        Vector2 look = lookInput * sensitivity * 0.1f;
        xRot -= look.y;
        xRot = Mathf.Clamp(xRot, -90.0f, 90.0f);

        transform.Rotate(Vector3.up * look.x);

        playerCamera.transform.localRotation = Quaternion.Euler(xRot, 0.0f, 0.0f);
    }
}
