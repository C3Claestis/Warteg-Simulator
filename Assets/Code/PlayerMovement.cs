using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform cam;  // Kamera
    [SerializeField] Transform body; // Badan Pemain
    [SerializeField] CinemachineVirtualCamera virtualCamera; // Cinemachine virtual camera
    [Range(0f, 10.0f)][SerializeField] float speed = 6.0f; // Speed move
    [Range(0f, 3.0f)][SerializeField] float mouseSensitivity = 2.0f; // Mouse sens
    private float jumpHeight = 1.0f; // Tinggi lompatan
    private float gravity = -9.81f;
    private CharacterController characterController;
    private Vector3 velocity;
    private PlayerInput inputActions;
    private Vector2 moveInput;
    private bool isJumping = false;
    private float xRotation = 0f;
    private CinemachinePOV povComponent;
    private bool isCanMove = true;
    public void SetCanMove(bool canMove) => this.isCanMove = canMove;
    public bool GetCanMove() => this.isCanMove;
    void Awake()
    {
        characterController = GetComponent<CharacterController>();

        // Initialize input actions
        inputActions = new PlayerInput();
    }

    void Start()
    {
        // Get the POV component from the Cinemachine virtual camera
        povComponent = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
    }

    void OnEnable()
    {
        // Enable the input actions
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.Jump.performed += OnJumpPerformed;
    }

    void OnDisable()
    {
        // Disable the input actions
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.Player.Disable();
    }

    void Update()
    {
        if (isCanMove)
        {
            // Calculate movement direction
            Vector3 move = CalculateMovementDirection(moveInput.x, moveInput.y);

            // Apply gravity
            if (characterController.isGrounded)
            {
                if (velocity.y < 0)
                {
                    velocity.y = -2f; // Reset velocity saat pemain menyentuh tanah
                }

                // Apply jump
                if (isJumping)
                {
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    isJumping = false; // Reset status lompat setelah lompat
                }
            }

            // Apply gravity
            velocity.y += gravity * Time.deltaTime;

            // Apply movement and gravity
            characterController.Move((move * speed + velocity) * Time.deltaTime);
        }
  
        // Handle camera and body rotation
        HandleCameraRotation();
        HandleCursor(isCanMove);
    }

    public bool HandleCursor(bool value)
    {
        if (value)
        {
            // Lock the cursor to the center of the screen and hide it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // Lock the cursor to the center of the screen and hide it
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        return isCanMove;
    }
    Vector3 CalculateMovementDirection(float moveHorizontal, float moveVertical)
    {
        // Calculate camera's forward and right directions
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        // Flatten the camera's forward vector (ignore the y component)
        camForward.y = 0f;
        camForward.Normalize();
        camRight.y = 0f;
        camRight.Normalize();

        // Calculate the desired move direction relative to the camera
        Vector3 move = camForward * moveVertical + camRight * moveHorizontal;

        return move.normalized;
    }

    void HandleCameraRotation()
    {
        // Get mouse input for camera rotation
        float mouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity * Time.deltaTime;

        // Update vertical camera rotation (pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Get the horizontal axis value from the Cinemachine POV component
        float yRotation = povComponent.m_HorizontalAxis.Value;
        body.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (characterController.isGrounded)
        {
            isJumping = true; // Set flag lompat saat pemain di tanah
        }
    }
}
