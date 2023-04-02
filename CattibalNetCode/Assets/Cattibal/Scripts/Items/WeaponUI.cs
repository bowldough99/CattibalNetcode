using System.Collections;
using System.Collections.Generic;
//using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public int itemNumber = 0;
    public Sprite[] weaponToShow;
    public GameObject item;

    private void Update()
    {
        item.GetComponent<Image>().sprite = weaponToShow[itemNumber];
    }

}
