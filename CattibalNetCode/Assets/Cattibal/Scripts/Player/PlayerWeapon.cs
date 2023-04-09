using CodeMonkey;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static PlayerWeapon;

public class PlayerWeapon : NetworkBehaviour
{

    public enum Weapon
    {
        Catnip,
        CannedFood,
        YarnBall,
        CardboardBox,
        None,
    }
    private Weapon weapon;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private LayerMask pickUpLayerMask;

    [SerializeField] private PlayerNetwork playerNetwork;
    [SerializeField] private WeaponUI weaponUI;
    public string itemHeld = "None";

    public override void OnNetworkSpawn()
    {
        if (weaponUI == null)
        {
            weaponUI = GameObject.FindObjectOfType<WeaponUI>(true);
        }
        weaponUI.itemNumber = 0;
    }

    private void Update()
    {
        if(itemHeld == "None")
        {
            weaponUI.itemNumber = 0;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (itemHeld == "None")
            {
                float pickUpDistance = 8f;
                if (Physics.Raycast(playerTransform.position, playerTransform.forward, out RaycastHit raycastHit, pickUpDistance, pickUpLayerMask))
                {
                    Debug.Log(raycastHit.transform);
                    if (raycastHit.transform.TryGetComponent(out WeaponScript weaponScript))
                    {
                        Debug.Log(weaponScript);
                        if (raycastHit.transform.gameObject.tag == "Catnip")
                        {
                            raycastHit.transform.gameObject.GetComponent<NetworkObject>().Despawn();
                            itemHeld = "Catnip";
                            weaponUI.itemNumber = 1;
                            //Debug.Log("i collected" + itemHeld);
                        }
                        else if (raycastHit.transform.gameObject.tag == "CannedFood")
                        {
                            raycastHit.transform.gameObject.GetComponent<NetworkObject>().Despawn();
                            itemHeld = "CannedFood";
                            weaponUI.itemNumber = 2;
                            //Debug.Log("i collected a" + itemHeld);
                        }
                        else if (raycastHit.transform.gameObject.tag == "Yarnball")
                        {
                            raycastHit.transform.gameObject.GetComponent<NetworkObject>().Despawn();
                            itemHeld = "Yarnball";
                            weaponUI.itemNumber = 3;
                            //Debug.Log("i collected a" + itemHeld);
                        }
                        else if (raycastHit.transform.gameObject.tag == "Box")
                        {
                            raycastHit.transform.gameObject.GetComponent<NetworkObject>().Despawn();
                            itemHeld = "Box";
                            weaponUI.itemNumber = 4;
                            //Debug.Log("i collected a" + itemHeld);
                        }
                        else if (raycastHit.transform.gameObject.tag == "FishBone")
                        {
                            raycastHit.transform.gameObject.GetComponent<NetworkObject>().Despawn();
                            itemHeld = "FishBone";
                            weaponUI.itemNumber = 5;
                            //Debug.Log("i collected a" + itemHeld);
                        }
                        else if (raycastHit.transform.gameObject.tag == "Milk")
                        {
                            raycastHit.transform.gameObject.GetComponent<NetworkObject>().Despawn();
                            itemHeld = "Milk";
                            weaponUI.itemNumber = 6;
                            //Debug.Log("i collected a" + itemHeld);
                        }

                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (itemHeld == "None")
            {

            }
            else if (itemHeld == "Catnip")
            {
                playerNetwork.HealthServerRPC("HEAL!", 10);
                itemHeld = "None";
            }
            else if (itemHeld == "CannedFood")
            {
                playerNetwork.HealthServerRPC("HEAL!", 10);
                itemHeld = "None";
            }
            else if (itemHeld == "Yarnball")
            {
                playerNetwork.HealthServerRPC("HEAL!", 10);
                itemHeld = "None";
            }
            else if (itemHeld == "Box")
            {
                playerNetwork.HealthServerRPC("HEAL!", 10);
                itemHeld = "None";
            }
            else if (itemHeld == "FishBone")
            {
                playerNetwork.HealthServerRPC("HEAL!", 10);
                itemHeld = "None";
            }
            else if (itemHeld == "Milk")
            {
                playerNetwork.HealthServerRPC("HEAL!", 10);
                itemHeld = "None";
            }

        }
    }

    public void UpdatePlayerWeapon(Weapon weapon)
    {

    }
}
