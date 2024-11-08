using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class NPCManager : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] GameObject panelPesan;
    [SerializeField] TakeOrder takeOrder;

    private NavMeshAgent agent;
    private Animator animator;
    private GameObject[] ArraypointPesanMenu;
    private GameObject[] ArraypointTempatMakan;
    private GameObject[] ArraypointTempatBayar;
    private Transform pointPesanMenu;
    private Transform pointTempatMakan;
    private Transform pointTempatBayar;
    private bool isToSitPosition;
    private bool isSedangMakan;
    private bool isSudahMakan;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        pointPesanMenu = SearchPoint("PointPesan");
        pointTempatBayar = SearchPoint("PointBayar");

        if (pointPesanMenu != null)
        {
            // Move to target
            agent.SetDestination(pointPesanMenu.position);
        }
    }

    private Transform SearchPoint(string tag)
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag(tag);

        if (array.Length == 0)
        {
            Debug.LogWarning("No points found with tag: " + tag);
            return null;
        }

        GameObject randomPoint = array[Random.Range(0, array.Length)];
        randomPoint.tag = "Untagged"; // Mengubah tag untuk tidak terambil lagi
        return randomPoint.transform;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isToSitPosition)
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
        if (isSudahMakan)
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
        if (SudahPesan && !isToSitPosition)
        {
            ArraypointTempatMakan = GameObject.FindGameObjectsWithTag("PointDudukWarteg");

            // Mengambil GameObject acak dari array dan mendapatkan komponen Transform-nya
            GameObject randomPointDuduk = ArraypointTempatMakan[Random.Range(0, ArraypointTempatMakan.Length)];
            pointTempatMakan = randomPointDuduk.transform;

            isToSitPosition = true;
            animator.SetBool("IsPesan", false);
            agent.SetDestination(pointTempatMakan.transform.position);
            panelPesan.SetActive(false);

            //Mengubah tag untuk tidak ikut terambil
            pointTempatMakan.gameObject.tag = "Untagged";
            pointPesanMenu.gameObject.tag = "PointPesan";

            //Kirim value name and icon ke tempat duduk
            GameObject panelpointTempatMakan = pointTempatMakan.transform.GetChild(0).gameObject;
            panelpointTempatMakan.SetActive(true);

            Transform parentComponent = panelpointTempatMakan.transform.GetChild(0);

            Text namaMenu = parentComponent.transform.GetChild(1).GetComponent<Text>();
            Image iconMenu = parentComponent.transform.GetChild(2).GetComponent<Image>();
            Debug.Log("NAMA MENU : " + namaMenu.text);

            namaMenu.text = takeOrder.GetNameMenu();
            iconMenu.sprite = takeOrder.GetIconMenu();

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

                isSedangMakan = true; //Nyalakan kondisi sedang makan

                // Mulai rotasi bertahap ke arah titik makan
                yield return StartCoroutine(RotateTowards(pointTempatMakan.forward));

                StartCoroutine(RandomWaktuMakan());
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
    private IEnumerator RandomWaktuMakan()
    {
        // Cek kondisi isSedangMakan sebelum memulai waktu makan
        if (isSedangMakan)
        {
            float waktuMakan = Random.Range(5f, 10f); // Tentukan rentang waktu makan acak
            Debug.Log("Waktu makan dimulai selama: " + waktuMakan + " detik");

            yield return new WaitForSeconds(waktuMakan); // Tunggu hingga waktu makan selesai

            Debug.Log("Waktu makan selesai");
            isSedangMakan = false;

            // Panggil fungsi lain jika perlu setelah waktu makan selesai
            StartCoroutine(StandToExit());
        }
    }

    private IEnumerator StandToExit()
    {
        animator.SetBool("IsSit", false);
        yield return new WaitForSeconds(3f);
        animator.SetBool("IsWalk", true);
        //Move to target
        agent.SetDestination(pointTempatBayar.transform.position);
        yield return new WaitForSeconds(1f);
        isSudahMakan = true;
        // Logic keluar setelah makan bisa ditambahkan di sini
        yield return null;
    }
}
