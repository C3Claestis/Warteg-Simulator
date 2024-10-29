using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NPCManager : MonoBehaviour
{
    [SerializeField] GameObject panelPesan;
    private NavMeshAgent agent;
    private Animator animator;
    private GameObject[] ArraypointPesanMenu;
    private GameObject[] ArraypointTempatMakan;
    private Transform pointPesanMenu;
    private Transform pointTempatMakan;
    private bool isSudahPesan;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        ArraypointPesanMenu = GameObject.FindGameObjectsWithTag("PointPesan");
        ArraypointTempatMakan = GameObject.FindGameObjectsWithTag("PointDudukWarteg");

        // Mengambil GameObject acak dari array dan mendapatkan komponen Transform-nya
        GameObject randomPointPesan = ArraypointPesanMenu[Random.Range(0, ArraypointPesanMenu.Length)];
        pointPesanMenu = randomPointPesan.transform;

        // Mengambil GameObject acak dari array dan mendapatkan komponen Transform-nya
        GameObject randomPointDuduk = ArraypointTempatMakan[Random.Range(0, ArraypointTempatMakan.Length)];
        pointTempatMakan = randomPointDuduk.transform;

        //Mengubah tag untuk tidak ikut terambil
        pointPesanMenu.gameObject.tag = "Untagged";

        //Move to target
        agent.SetDestination(pointPesanMenu.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSudahPesan)
        {
            // Mengecek apakah AI sudah mencapai tujuan dan berhenti
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && agent.velocity.sqrMagnitude == 0f)
            {
                animator.SetBool("IsPesan", true);  // Memulai animasi baru ketika tujuan sudah tercapai

                // Mengatur rotasi AI agar menghadap sesuai sumbu depan (z) dari point tujuan
                Vector3 targetDirection = pointPesanMenu.transform.forward;  // Mendapatkan sumbu depan dari target
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Smooth rotation
                panelPesan.SetActive(true);
            }
        }
    }
    public void SetSudahPesan(bool SudahPesan)
    {
        if (SudahPesan && !isSudahPesan)
        {
            isSudahPesan = true;
            animator.SetBool("IsPesan", false);
            agent.SetDestination(pointTempatMakan.transform.position);
            panelPesan.SetActive(false);

            //Mengubah tag untuk tidak ikut terambil
            pointTempatMakan.gameObject.tag = "Untagged";
            pointPesanMenu.gameObject.tag = "PointPesan";

            // Mulai coroutine untuk pengecekan titik makan
            StartCoroutine(CheckArrivalAtPointTempatMakan());
        }
    }
    private IEnumerator CheckArrivalAtPointTempatMakan()
    {
        // Loop hingga mencapai pointTempatMakan
        while (true)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && agent.velocity.sqrMagnitude == 0f)
            {
                animator.SetBool("IsSit", true); // Mulai animasi duduk makan
                animator.SetBool("IsWalk", false); // Menghentikan animasi berjalan

                // Mulai rotasi bertahap ke arah titik makan
                yield return StartCoroutine(RotateTowards(pointTempatMakan.forward));

                // Keluar dari coroutine setelah mencapai tujuan
                yield break;
            }

            // Tunggu satu frame sebelum pengecekan ulang
            yield return null;
        }
    }

    private IEnumerator RotateTowards(Vector3 targetDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            // Lakukan rotasi bertahap menuju target
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            yield return null;  // Tunggu satu frame sebelum melanjutkan rotasi
        }

        // Pastikan rotasi benar-benar ke target
        transform.rotation = targetRotation;
    }
}
