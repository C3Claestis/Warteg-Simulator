using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

public class BukuBesarManager : MonoBehaviour
{
    [SerializeField] GameObject panelBukuBesar;
    [SerializeField] Text jumlah;
    private PlayerInput inputActions;
    
    private void Awake()
    {
        inputActions = new PlayerInput();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
