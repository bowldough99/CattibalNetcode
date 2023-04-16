using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public int itemNumber = 0;
    public Sprite[] weaponToShow;
    public GameObject item;

    public static WeaponUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        item.GetComponent<Image>().sprite = weaponToShow[itemNumber];
    }

    private void ShowWeaponUI(int weaponNumber)
    {
        item.GetComponent<Image>().sprite = weaponToShow[weaponNumber];
    }
    


}
