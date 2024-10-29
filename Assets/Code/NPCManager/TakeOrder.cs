using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakeOrder : MonoBehaviour
{
    [SerializeField] List<Order> order = new List<Order>();
    [SerializeField] Text Menu;
    [SerializeField] Image iconMenu;

    public string  GetNameMenu() => Menu.text;
    public Sprite  GetIconMenu() => iconMenu.sprite;
    // Start is called before the first frame update
    void Start()
    {
        RandomMenu();
    }
    void RandomMenu()
    {
        int randomMenu = Random.Range(0, order.Count);
        
        Menu.text = order[randomMenu].nameMenu;
        iconMenu.sprite = order[randomMenu].icon;
    }
}

[System.Serializable]
public class Order
{
    public string nameMenu;
    public Sprite icon;
}
