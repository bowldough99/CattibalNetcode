using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        if (!IsOwner) return;
        if (itemHeld == "None")
        {
            weaponUI.itemNumber = 0;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (itemHeld == "None")
            {

            }
            else if (itemHeld == "Catnip")
            {
                playerNetwork.RegainHungerServerRPC("I am full again!");
                itemHeld = "None";
            }
            else if (itemHeld == "CannedFood")
            {
                playerNetwork.RegainHungerServerRPC("I am full again!");
                itemHeld = "None";
            }
            else if (itemHeld == "Yarnball")
            {
                playerNetwork.RegainHungerServerRPC("I am full again!");
                itemHeld = "None";
            }
            else if (itemHeld == "Box")
            {
                playerNetwork.RegainHungerServerRPC("I am full again!");
                itemHeld = "None";
            }
            else if (itemHeld == "FishBone")
            {
                playerNetwork.RegainHungerServerRPC("I am full again!");
                itemHeld = "None";
            }
            else if (itemHeld == "Milk")
            {
                playerNetwork.RegainHungerServerRPC("I am full again!");
                itemHeld = "None";
            }

        }
    }
    private void OnTriggerStay(Collider collision)
    {
        if (!IsOwner) return; 

        if (collision.gameObject.layer == 10)
        {
            GameUI.Instance.AddCanPickUpMessage("PRESS E TO PICK UP");
            //NotifyCanPickUpClientRpc("PRESS E TO PICK UP");
            Debug.Log(OwnerClientId +"touched" + collision.gameObject.tag);
            if (Input.GetKeyDown(KeyCode.E))
            {
                CollectItem(collision.gameObject);
            }
        }

    }
    [ServerRpc]
    public void NotifyWeaponServerRpc(int weaponNumber, string weaponName, ulong clientId)
    {
        ApplyWeapon(weaponNumber, weaponName, clientId);
        NotifyWeaponClientRpc(weaponNumber, weaponName, clientId);
        return;

        var clientCollected = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerWeapon>();
        if (clientCollected != null)
        {
            if (!IsOwner) return;
            clientCollected.itemHeld = weaponName;
            clientCollected.weaponUI.itemNumber = weaponNumber;
            Debug.Log(OwnerClientId + "collected" + itemHeld); //QQ why this one only written in debug if only 1 player exists in the game? then when got 2 players suddenly doesnt write anymore;-;
            // but yeah it seems like only the host can interact with the weapons but not the clients..
        }
        else
        {
            Debug.Log(string.Format("which lost child is this. {0}", clientId));
        }

    }

    [ClientRpc]
    public void NotifyWeaponClientRpc(int weaponNumber, string weaponName, ulong clientId)
    {
        ApplyWeapon(weaponNumber, weaponName, clientId);
    }

    public void ApplyWeapon(int weaponNumber, string weaponName, ulong clientId)
    {
        itemHeld = weaponName;
        if(IsOwner)
            weaponUI.itemNumber = weaponNumber;
        return;
        var clientCollected = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerWeapon>();
        if (clientCollected != null)
        {
            if (!IsOwner) return;
            clientCollected.itemHeld = weaponName;
            clientCollected.weaponUI.itemNumber = weaponNumber;
            Debug.Log(OwnerClientId + "collected" + itemHeld); //QQ why this one only written in debug if only 1 player exists in the game? then when got 2 players suddenly doesnt write anymore;-;
            // but yeah it seems like only the host can interact with the weapons but not the clients..
        }
        else
        {
            Debug.Log(string.Format("which lost child is this. {0}", clientId));
        }
    }

    [ClientRpc]
    private void NotifyCanPickUpClientRpc(string message)
    {
        if (!IsOwner) return;
        GameUI.Instance.AddCanPickUpMessage(message);
    }

    public void CollectItem(GameObject other)
    {
        // qiang: IF SERVER: apply weapon changes for self, send notification to client to apply weapon changes
        // IF CLIENT: send server rpc to tell server to do  the above ^
        if (itemHeld == "None")
        {
            if (other.gameObject.tag == "Catnip")
            {
                if(NetworkManager.Singleton.IsServer)
                {
                    ApplyWeapon(1, "Catnip", OwnerClientId);
                    NotifyWeaponClientRpc(1, "Catnip", OwnerClientId);
                }
                else
                {
                    NotifyWeaponServerRpc(1, "Catnip", OwnerClientId);
                }
                DestroyWeaponCollected(other.gameObject);
            }
            else if (other.gameObject.tag == "CannedFood")
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    ApplyWeapon(2, "CannedFood", OwnerClientId);
                    NotifyWeaponClientRpc(2, "CannedFood", OwnerClientId);
                }
                else
                {
                    NotifyWeaponServerRpc(2, "CannedFood", OwnerClientId);
                }
                DestroyWeaponCollected(other.gameObject);
            }
            else if (other.gameObject.tag == "Yarnball")
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    ApplyWeapon(3, "Yarnball", OwnerClientId);
                    NotifyWeaponClientRpc(3, "Yarnball", OwnerClientId);
                }
                else
                {
                    NotifyWeaponServerRpc(3, "Yarnball", OwnerClientId);
                }
                DestroyWeaponCollected(other.gameObject);
            }
            else if (other.gameObject.tag == "Box")
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    ApplyWeapon(4, "Box", OwnerClientId);
                    NotifyWeaponClientRpc(4, "Box", OwnerClientId);
                }
                else
                {
                    NotifyWeaponServerRpc(4, "Box", OwnerClientId);
                }
                DestroyWeaponCollected(other.gameObject);
            }
            else if (other.gameObject.tag == "FishBone")
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    ApplyWeapon(5, "FishBone", OwnerClientId);
                    NotifyWeaponClientRpc(5, "FishBone", OwnerClientId);
                }
                else
                {
                    NotifyWeaponServerRpc(5, "FishBone", OwnerClientId);
                }
                DestroyWeaponCollected(other.gameObject);
            }
            else if (other.gameObject.tag == "Milk")
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    ApplyWeapon(6, "Milk", OwnerClientId);
                    NotifyWeaponClientRpc(6, "Milk", OwnerClientId);
                }
                else
                {
                    NotifyWeaponServerRpc(6, "Milk", OwnerClientId);
                }
                DestroyWeaponCollected(other.gameObject);
            }
        }
    }

    public void DestroyWeaponCollected(GameObject weaponObject)
    {
        DestroyWeaponServerRpc(weaponObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyWeaponServerRpc(NetworkObjectReference weaponNetworkObjectReference)
    {
        GameObject objectToDestroy = weaponNetworkObjectReference;
        objectToDestroy.GetComponent<NetworkObject>().Despawn();
    }

}

