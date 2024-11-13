using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
public class BukuBesarManager : MonoBehaviour
{
    [SerializeField] GameObject panelBukuBesar;
    [SerializeField] TextMeshProUGUI currentPiring;
    [SerializeField] Transform MenuInGame;
    private PlayerInput inputActions;
    private bool isPanelActive = false;
    int activeChildCount;

    // Referensi ke script PlayerMovement
    [SerializeField] private PlayerMovement playerMovement;

    private void Awake()
    {
        inputActions = new PlayerInput();
    }

    private void Start()
    {
        // Menghitung jumlah child yang aktif di MenuInGame
        activeChildCount = CountActiveChildrenOfGameObject(MenuInGame.gameObject);
        currentPiring.text = activeChildCount.ToString();
    }
    private void OnEnable()
    {
        // Mengaktifkan input actions dan mengikat tindakan untuk tombol M
        inputActions.Player.Enable();
        inputActions.Player.OpenBook.performed += OnToggleBukuBesar;
    }

    private void OnDisable()
    {
        // Menonaktifkan input actions
        inputActions.Player.Disable();
        inputActions.Player.OpenBook.performed -= OnToggleBukuBesar;
    }

    // Method untuk menangani tombol M ditekan
    private void OnToggleBukuBesar(InputAction.CallbackContext context)
    {
        isPanelActive = !isPanelActive; // Toggle status panel

        // Aktifkan atau nonaktifkan panel Buku Besar
        panelBukuBesar.SetActive(isPanelActive);

        // Nonaktifkan atau aktifkan pergerakan pemain
        playerMovement.SetCanMove(!isPanelActive);
    }
    // Fungsi untuk menghitung jumlah child yang aktif
    private int CountActiveChildrenOfGameObject(GameObject parent)
    {
        int count = 0;

        // Iterasi melalui setiap child dan cek apakah child aktif
        foreach (Transform child in parent.transform)
        {
            if (child.gameObject.activeInHierarchy)  // Memeriksa jika child aktif
            {
                count++;
            }
        }

        return count;
    }
}
