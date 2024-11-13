using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMakan : MonoBehaviour
{
    void Update()
    {
        // Membuat raycast ke arah sumbu z objek
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1f))
        {
            if (hit.collider.CompareTag("Pelanggan"))
            {
                NPCManager nPCManager = hit.collider.GetComponent<NPCManager>();

                if (nPCManager != null)
                {
                    nPCManager.SetSedangMakan(true);
                    gameObject.SetActive(false);
                }
            }

        }

        // Menggambar garis untuk visualisasi raycast di Scene view
        Debug.DrawRay(transform.position, transform.forward * 1f, Color.red);
    }
}
