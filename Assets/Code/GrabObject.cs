using UnityEngine;

public class GrabObject : MonoBehaviour
{
    public bool isCanGrab;
    public bool isGrab;
    private Transform reference;
    private Rigidbody rb;

    [SerializeField] Vector3 rotasiObjek;
    public void SetIsCanGrab(bool isCanGrab)
    {
        this.isCanGrab = isCanGrab;
    }

    public void SetReference(Transform reference)
    {
        this.reference = reference;
    }

    public void ToggleGrab()
    {
        isGrab = !isGrab;
        HandleRigidbody();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isCanGrab && isGrab)
        {
            transform.parent = reference;
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 10); // Menggunakan Lerp
            transform.localRotation = Quaternion.Euler(rotasiObjek.x, rotasiObjek.y, rotasiObjek.z);
        }

        else if (!isGrab)
        {
            isCanGrab = false;
            transform.parent = null;
        }
    }

    private void HandleRigidbody()
    {
        if (isGrab)
        {
            // Menonaktifkan Rigidbody
            rb.isKinematic = true;
            rb.useGravity = false; // Ini bisa dihilangkan jika tidak diperlukan
            rb.detectCollisions = false; // Jika Anda ingin mencegah benturan
        }
        else
        {
            // Mengaktifkan kembali Rigidbody
            rb.isKinematic = false;
            rb.useGravity = true; // Mengaktifkan kembali gravitasi
            rb.detectCollisions = true; // Mengaktifkan kembali deteksi benturan
        }
    }
}
