using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class InteractionManager
{
    private MenuWarteg menuWarteg;
    private GrabObject currentGrabObject;
    private Button buttonKonfirmasi;
    private GameObject pointPesanan;

    private Text textMesh;
    private Transform cam;
    private Transform grabDepan;
    private Transform grabKiri;
    private float direction;
    private bool canGrab;

    private PlayerInteract playerInteract;
    private PlayerMovement playerMovement;

    public InteractionManager(float direction, Transform cam, Text textMesh, PlayerInteract playerInteract,
    PlayerMovement playerMovement, Transform grabDepan, Transform grabKiri)
    {
        this.direction = direction;
        this.cam = cam;
        this.textMesh = textMesh;
        this.playerInteract = playerInteract;
        this.playerMovement = playerMovement;
        this.grabDepan = grabDepan;
        this.grabKiri = grabKiri;
    }

    public void PerformRaycast()
    {
        if (cam == null)
        {
            Debug.LogError("Referensi kamera tidak diatur.");
            return;
        }

        textMesh.text = string.Empty;
        canGrab = false;
        currentGrabObject = null;
        menuWarteg = null;

        // Membuat ray dari titik tengah layar menggunakan arah kamera
        Ray ray = new Ray(cam.position, cam.forward);
        RaycastHit hit;

        // Menggambar ray untuk tujuan debugging
        Debug.DrawRay(ray.origin, ray.direction * direction, Color.red);

        // Interact dengan objek yang memiliki tag "GrabObject" atau "door"
        if (Physics.Raycast(ray, out hit, direction))
        {
            if (hit.collider.CompareTag("PiringBersih"))
            {
                GrabObject grabObject = hit.collider.GetComponent<GrabObject>();

                if (grabObject != null)
                {
                    textMesh.text = hit.collider.name;
                    canGrab = true;
                    currentGrabObject = grabObject;
                    grabObject.SetIsCanGrab(true);

                    // Mengatur referensi grabObject dengan transform grab dari PlayerInteract
                    playerInteract.SetGrabReference(grabObject);
                }
            }

            //Take menu warteg
            else if (hit.collider.CompareTag("MenuWarteg"))
            {
                menuWarteg = hit.collider.GetComponent<MenuWarteg>(); // Menyimpan referensi MenuWarteg
                if (menuWarteg != null)
                {
                    textMesh.text = menuWarteg.GetNameMenu(); // Tampilkan nama menu
                    canGrab = true; // Tetapkan canGrab menjadi true untuk menu
                }
            }

            //Konfirmasi pesanan NPC
            else if (hit.collider.CompareTag("Konfirmasi"))
            {
                // Mencari Button di objek yang terkena
                buttonKonfirmasi = hit.collider.GetComponent<Button>();
                if (buttonKonfirmasi != null)
                {
                    textMesh.text = "Konfirmasi Pesanan!";
                    Debug.Log($"Button {buttonKonfirmasi.name} siap untuk diklik.");
                }
            }

            else if (hit.collider.CompareTag("PointMakan"))
            {
                if (pointPesanan != hit.collider.gameObject)
                {
                   
                }
            }
        }
    }

    public void HandleInteractObject()
    {
        // Interaksi dengan objek yang bisa di-grab
        if (canGrab && currentGrabObject != null)
        {
            // Jika tidak ada objek di grab, grab objek baru
            if (grabDepan.childCount == 0)
            {
                currentGrabObject.SetReference(grabDepan); // Mengatur referensi grab
                currentGrabObject.ToggleGrab(); // Mengaktifkan grab
            }
        }

        if (!canGrab && currentGrabObject == null && grabDepan.childCount != 0)
        {
            // Jika ada objek di grab, lepaskan objek tersebut
            GrabObject getObjekInGrab = grabDepan.GetChild(0).GetComponent<GrabObject>();
            getObjekInGrab.ToggleGrab(); // Melepas grab
            getObjekInGrab.SetReference(null); // Menghapus referensi grab

            // Jika objek yang sedang di-grab sama dengan currentGrabObject, reset currentGrabObject
            if (getObjekInGrab == currentGrabObject)
            {
                currentGrabObject = null;
            }
        }

        // Jika interaksi dapat dilakukan dengan MenuWarteg
        if (canGrab && menuWarteg != null)
        {
            menuWarteg.AmbilItem(5); // Panggil AmbilItem dan kurangi jumlahnya 10
        }

        // Jika buttonKonfirmasi terdeteksi dan ingin dieksekusi
        if (buttonKonfirmasi != null)
        {
            // Eksekusi klik pada buttonKonfirmasi
            buttonKonfirmasi.onClick.Invoke(); // Menjalankan metode onClick pada button
            Debug.Log($"Button {buttonKonfirmasi.name} telah diklik.");
            buttonKonfirmasi = null; // Reset buttonKonfirmasi setelah eksekusi
        }
    }

    public void SwitchGrabPosition()
    {
        // Jika ada objek di grabKiri, pindahkan ke grabDepan
        if (grabKiri.childCount != 0)
        {
            // Ambil objek yang di-grab dari grabKiri
            GrabObject getObjekInGrab = grabKiri.GetChild(0).GetComponent<GrabObject>();

            // Set referensi baru ke grabDepan tanpa melepaskan objek
            getObjekInGrab.SetReference(grabDepan);

            // Pindahkan objek dari grabKiri ke grabDepan
            getObjekInGrab.transform.SetParent(grabDepan);
        }
        // Jika tidak ada objek di grabKiri, periksa grabDepan dan pindahkan objeknya ke grabKiri
        else if (grabDepan.childCount != 0)
        {
            // Ambil objek yang di-grab dari grabDepan
            GrabObject getObjekInGrab = grabDepan.GetChild(0).GetComponent<GrabObject>();

            // Set referensi baru ke grabKiri tanpa melepaskan objek
            getObjekInGrab.SetReference(grabKiri);

            // Pindahkan objek dari grabDepan ke grabKiri
            getObjekInGrab.transform.SetParent(grabKiri);
        }
    }

    public void SetPlayerInteract(PlayerInteract playerInteract)
    {
        this.playerInteract = playerInteract;
    }
}
