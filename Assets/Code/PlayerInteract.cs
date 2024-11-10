using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    [Header("Jangkauan Raycast")][Range(0.1f, 5.0f)][SerializeField] private float direction = 2.5f;
    [Header("Camera Referensi")][SerializeField] private Transform cam; // Referensi ke Kamera
    [Header("Text Object Raycast")][SerializeField] private Text textMesh;
    [Header("Referensi Grab Raycast")][SerializeField] Transform grabDepan;
    [Header("Referensi Grab Raycast")][SerializeField] Transform grabKiri;

    private PlayerInput inputActions;
    private InteractionManager interactionManager;

    void Awake()
    {
        // Inisialisasi input actions
        inputActions = new PlayerInput();
        interactionManager = new InteractionManager(direction, cam, textMesh, this, grabDepan, grabKiri);
        SetInteractionManager(interactionManager);
        interactionManager.SetPlayerInteract(this);
    }

    void OnEnable()
    {
        // Mengaktifkan input actions dan mengikat tindakan Interact
        inputActions.Player.Enable();
        inputActions.Player.Object.performed += OnInteractObject;
        inputActions.Player.SwitchPlate.performed += OnSwitchGrabPosition;
        
    }

    void OnDisable()
    {
        // Menonaktifkan input actions
        inputActions.Player.Disable();
        inputActions.Player.Object.performed -= OnInteractObject;
        inputActions.Player.SwitchPlate.performed -= OnSwitchGrabPosition;
    }

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main.transform;
        }
    }

    void Update()
    {
        interactionManager.PerformRaycast();
    }

    //Interaksi untuk object grab saja dengan LEFT_MOUSE
    private void OnInteractObject(InputAction.CallbackContext context)
    {
        // Handle additional interaction
        interactionManager.HandleInteractObject();
    }
    // Method untuk menangani perpindahan object grab dari grabKiri ke grabDepan
    private void OnSwitchGrabPosition(InputAction.CallbackContext context)
    {
        interactionManager.SwitchGrabPosition();
    }
    
    public void SetGrabReference(GrabObject grabObject)
    {
        grabObject.SetReference(grabDepan);
    }
    public void SetInteractionManager(InteractionManager interactionManager)
    {
        this.interactionManager = interactionManager;
    }
}
