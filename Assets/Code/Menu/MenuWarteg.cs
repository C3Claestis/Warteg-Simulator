using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MenuWarteg : MonoBehaviour
{
    [SerializeField] private int indexMenu; // Indeks menu
    [SerializeField] private TMPro.TextMeshProUGUI textMesh;
    [SerializeField] private Transform grabPlayerSajian;
    [SerializeField] private Transform piringBersih;
    [SerializeField] private Transform nasiContainer;
    private string nameMenu; // Nama menu
    private int persentaseMenu = 100; // Persentase sisa menu
    private List<GameObject> tampilanSetiapRatio; // Daftar tampilan berdasarkan rasio menu
    // Start is called before the first frame update
    void Start()
    {
        // Mengambil nama GameObject sebagai nama menu
        nameMenu = gameObject.name;
        textMesh.text = nameMenu.Substring(5);
        grabPlayerSajian = GameObject.Find("GrabSajian").GetComponent<Transform>();

        // Mengambil semua child sebagai tampilanSetiapRatio
        tampilanSetiapRatio = new List<GameObject>();
        foreach (Transform child in transform)
        {
            tampilanSetiapRatio.Add(child.gameObject);
        }
        // Memperbarui tampilan pada awal
        UpdateTampilanAktif();
    }

    // Method untuk mengambil item dari menu dan mengurangi persentase
    public void AmbilItem(int jumlah)
    {
        if (grabPlayerSajian.GetChild(0).gameObject.tag == "PiringBersih")
        {
            piringBersih = grabPlayerSajian.GetChild(0).GetComponent<Transform>(); // Ambil child pertama

            if (piringBersih != null) // Pastikan ada child di grabPlayerSajian
            {
                nasiContainer = piringBersih.GetChild(0).GetComponent<Transform>(); // Ambil child pertama dari "PiringBersih"

                if (nasiContainer != null) // Pastikan nasiContainer memiliki child
                {
                    Transform nasi = nasiContainer.GetChild(0); // Ambil child pertama dari nasiContainer

                    //Aktifkan nasi
                    if (gameObject.name == "Menu-Nasi")
                    {
                        Debug.Log("Nama GameObject: " + gameObject.name);
                        if (!nasi.gameObject.activeSelf)
                        {
                            nasi.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        if (nasi.gameObject.activeSelf) // Cek apakah `nasi` aktif
                        {
                            if (jumlah <= 0)
                            {
                                Debug.LogWarning("Jumlah harus lebih besar dari 0!");
                                return;
                            }

                            persentaseMenu -= jumlah;

                            if (persentaseMenu < 0)
                            {
                                persentaseMenu = 0; // Pastikan tidak negatif
                            }

                            // Pastikan indexMenu valid sebelum mengakses child
                            if (indexMenu >= 0 && indexMenu < nasiContainer.childCount)
                            {
                                Transform childNasi = nasiContainer.GetChild(indexMenu); // Ambil child sesuai indexMenu
                                Debug.Log($"Mengakses childNasi dengan index {indexMenu}: {childNasi.name}"); // Log nama childNasi

                                if (!childNasi.gameObject.activeSelf) // Jika child belum aktif
                                {
                                    childNasi.gameObject.SetActive(true); // Aktifkan child sesuai dengan indexMenu
                                    Debug.Log($"Aktifkan childNasi: {childNasi.name}"); // Log jika diaktifkan
                                }
                            }
                        }
                    }
                    UpdateTampilanAktif();
                }
            }
        }
    }

    // Metode untuk memperbarui tampilan berdasarkan persentase menu
    private void UpdateTampilanAktif()
    {
        // Nonaktifkan semua tampilan terlebih dahulu
        foreach (GameObject tampilan in tampilanSetiapRatio)
        {
            tampilan.SetActive(false);
        }

        // Kemudian aktifkan tampilan berdasarkan persentase
        if (persentaseMenu > 70 && persentaseMenu <= 100)
        {
            tampilanSetiapRatio[0].SetActive(true);
        }
        else if (persentaseMenu > 50 && persentaseMenu <= 70)
        {
            tampilanSetiapRatio[1].SetActive(true);
        }
        else if (persentaseMenu > 20 && persentaseMenu <= 50)
        {
            tampilanSetiapRatio[2].SetActive(true);
        }
        else if (persentaseMenu > 0 && persentaseMenu <= 20)
        {
            tampilanSetiapRatio[3].SetActive(true);
        }
    }

    // Method untuk mengembalikan nama menu
    public string GetNameMenu()
    {
        return nameMenu;
    }

    // Method untuk mendapatkan persentase menu saat ini
    public int GetPersentaseMenu()
    {
        return persentaseMenu;
    }
}
