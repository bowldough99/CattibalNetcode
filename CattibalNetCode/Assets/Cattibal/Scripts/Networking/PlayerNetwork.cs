using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine;
using static CattibalLobbyManager;
using UnityEngine.TextCore.Text;
using Unity.Netcode.Components;
using UnityEngine.XR;
using StarterAssets;
using TMPro;

[RequireComponent(typeof(NetworkTransform))]
public class PlayerNetwork : NetworkBehaviour
{
    private enum Weapon
    {
        Catnip,
        CannedFood,
        YarnBall,
        Laser,
        None,
    }
    
    private Weapon weapon;
    [SerializeField] private GameObject weaponHeld;

    [SerializeField] private Transform spawnedObjectPrefab; //GameObject spawnedObjectPrefab also works but limitation
    private Transform spawnedObjectTransform;
    bool moveToSpawn = false;
    [SerializeField]private HealthSystem healthSystem;
    public HealthBar healthBar;
    public HungerBar hungerBar;

    private const int defaultHp = 100; 
    private const int defaultHunger = 0; 

    private NetworkVariable<int> hp = new NetworkVariable<int>(defaultHp);
    private NetworkVariable<int> hunger = new NetworkVariable<int>(defaultHunger);

    private const float HUNGER_INCREASE_TIMER_MAX = 0.5f;
    private const float HEALTH_DECREASE_TIMER_MAX = 0.5f;
    private float hungerIncreaseTimer;
    private float healthDecreaseTimer;

    [SerializeField] TextMeshPro endGameText;

    private int skinNumberToSpawn;

    [SerializeField] private Material[] catSkins;
    [SerializeField] private Material skinMaterialToUse;
    [SerializeField] private GameObject spawnedPlayerPrefab;

    [SerializeField] private Transform playerHand;
    [SerializeField] private float minimumAttackDistance = 1.0f;

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CattibalGameManager gameManager;

    private NetworkVariable<MyCustomData> playerData = new NetworkVariable<MyCustomData>(
        new MyCustomData {
            _int = 0,
            _bool = false,
            _float = 0.2f,
            _health = 100,
            //hunger = 0,
            //isAttacked = false,
            //isAttacking = false,
            //isHungry = false,
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    //impt: Network Variable must be initialized if not got error 

    //impt: NetworkVariable can only be value type (int, float, enum, bool, struct), not reference types (class, object, array, string)
    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public float _float;
        public FixedString128Bytes message; //preallocate some memory very specific to the string character length choose the right one
        public int _health;
        public int _maxHealth;

        // impt: add a serializer and the : INetworkSerializable to make this work if not got error
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter 
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref _float);
            serializer.SerializeValue(ref message);
            serializer.SerializeValue(ref _health);
        }
    }

    //impt: anything related to network should not use Start or Awake but use OnNetworkSpawn instead
    public override void OnNetworkSpawn()
    {
        CameraManager.Instance.ShowFirstPersonView();

        //playerData.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
        //{
        //    Debug.Log(OwnerClientId + "; randomNumber:" + newValue._int + ";" + newValue._bool + ";" + newValue._float + ";" + newValue.message);
        //};

        if (healthBar == null)
        {
            healthBar = GameObject.FindObjectOfType<HealthBar>(true);
        }
        if (hungerBar == null)
        {
            hungerBar = GameObject.FindObjectOfType<HungerBar>(true);
        }
        if(playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }
        if (gameManager == null)
        {
            gameManager = GameObject.FindObjectOfType<CattibalGameManager>(true);
        }

        //hp.Value = 100;
        //hunger.Value = 0;
        weaponHeld = null;

        SkinnedMeshRenderer meshRenderer = spawnedPlayerPrefab.GetComponent<SkinnedMeshRenderer>();
        meshRenderer.material = catSkins[Random.Range(0, catSkins.Length)];
        //meshRenderer.material = skinMaterialToUse;
        //Debug.Log(CattibalLobbyManager.KEY_PLAYER_SKIN.ToString());

        if (IsOwner)
        {
            GameObject.FindObjectOfType<TutorialUI>().player = this;
        }
    }

    


    private void Update()
    {

        if (!moveToSpawn)
        {
            //gameManager.GetComponent<CattibalGameManager>().registerPlayer();
            transform.position = gameManager.GetComponent<CattibalGameManager>().getSpawnPoint();
            
            moveToSpawn = true;
        }
        if (!IsOwner) return; //anything before only works when IsOwner

        if (Input.GetKeyDown(KeyCode.T))
        {
            HealthServerRPC("ATTACK!", -10); //But the client can still press T which shows the Debug in the server logs
            //TestServerRPC(new ServerRpcParams()); //But the client can still press T which shows the Debug in the server logs
            //TestClientRPC(new ClientRpcParams { Send = new ClientRpcSendParams {TargetClientIds = new List<ulong> { 1 } } }); //But the client can still press T which shows the Debug in the server logs
            //spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
            //spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true); //if i want to allow client to spawn this, it needs to be in the ServerRPC

            //playerData.Value = new MyCustomData
            //{
            //    _int = 10,
            //    _bool = true,
            //    _float = 0.5f,
            //    message = "Attacked",
            //    _health = 10
            //};
        }


        if (Input.GetKeyDown(KeyCode.V))
        {
            HealthServerRPC("HEAL!", 5);
            HungerServerRPC("EAT!", 100);
            //hp.Value += 10;

            //spawnedObjectTransform.GetComponent<NetworkObject>().Despawn(true);
            //usefull to use the despawn method if i want to remove it from the network but keep the gameobject alive
            //Destroy(spawnedObjectTransform.gameObject);
        }

        hungerIncreaseTimer += Time.deltaTime;

        if (hungerIncreaseTimer >= HUNGER_INCREASE_TIMER_MAX)
        {
            HungerServerRPC("hungry...", -5);
            hungerIncreaseTimer = 0;
        }

        if (hunger.Value == 100)
        {
            healthDecreaseTimer += Time.deltaTime;

            if (healthDecreaseTimer >= HEALTH_DECREASE_TIMER_MAX)
            {
                HealthServerRPC("lose hp", -1);
                healthDecreaseTimer = 0;
            }
            
        }

        healthBar.updateHP(hp.Value / 100.0f);
        hungerBar.updateHunger(hunger.Value / 100.0f);

    }

    private void FixedUpdate()
    {
        if(IsClient && IsOwner)
        {

            if (playerMovement.IsAttacking == true)
            {
                CheckPunch(playerHand.transform);
            }
        }
    }

    private void CheckPunch(Transform hand)
    {
        RaycastHit hit;

        int layerMask = LayerMask.GetMask("Character");
        int layerMask2 = LayerMask.GetMask("Box");

        if (Physics.Raycast(hand.position, hand.transform.forward, out hit, minimumAttackDistance, layerMask))
        {
            Debug.DrawRay(hand.position, hand.transform.forward * minimumAttackDistance, Color.yellow);
            var playerHit = hit.transform.GetComponent<NetworkObject>();
            if(playerHit != null)
            {
                UpdateHealthServerRPC(15, playerHit.OwnerClientId);
                Debug.Log(playerHit.OwnerClientId);
            }
            Debug.Log("raycast hitting");
        }
        else if(Physics.Raycast(hand.position, hand.transform.forward, out hit, minimumAttackDistance, layerMask2))
        {
            Debug.Log("Box hit");
        }
        else
        {
            Debug.DrawRay(hand.position, hand.transform.forward * minimumAttackDistance, Color.red);
            Debug.Log("raycast NOT hitting");
        }
    }

    private void CheckWeapon()
    {
        if (weaponHeld == null)
        {
            return;
        }
    }

    [ServerRpc]
    //impt: the name of function must end with ServerRPC
    private void HealthServerRPC(string message, int healthChange)
    //private void TestServerRpc(ServerRpcParams serverRpcParams)
    {
        Debug.Log("HealthServerRPC" + OwnerClientId + "; " + message);
        hp.Value += healthChange;
        if (hp.Value > 100)
        {
            hp.Value = 100;
        }

        if (hp.Value <= 0)
        {
            Debug.Log("player is dead");
        }
        //Everything that is put here will only run in the server/host but not in client 
        //Debug.Log("TestServerRPC" + OwnerClientId + ";" + message);
        //Debug.Log("TestServerRPC" + OwnerClientId + ";" + serverRpcParams.Receive.SenderClientId);
        //this is a good place to send strings as compare to using the FixedString method
        //this is a good function to use to get messages from clients if i want the client to not have ownership
    }
    [ServerRpc]
    private void HungerServerRPC(string message, int hungerChange)
    {
        Debug.Log("HungerServerRPC" + OwnerClientId + "; " + message);
        hunger.Value -= hungerChange;
        if (hunger.Value < 0)
        {
            hunger.Value = 0;
        }
        if (hunger.Value > 100)
        {
            hunger.Value = 100;
        }
    }

    [ServerRpc]
    private void UpdateHealthServerRPC(int healthDamaged, ulong clientId)
    {
        var clientDamaged = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerNetwork>();
        if(clientDamaged != null && clientDamaged.hp.Value > 0 && clientDamaged.hp.Value < 100)
        {
            clientDamaged.hp.Value -= healthDamaged;
        }

        NotifyHealthChangedClientRpc(healthDamaged, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] {clientId}
            }
        });

        Debug.Log("health changing");
    }

    [ClientRpc]
    public void NotifyHealthChangedClientRpc(int healthDamaged, ClientRpcParams clientRpcParams = default)
    {
        if (IsOwner) return;
        Debug.Log("Got punched" + healthDamaged);
    }

    [ClientRpc]
    //impt: the name of function must end with ServerRPC
    //private void TestServerRPC(string message)
    private void TestClientRpc(ClientRpcParams clientRpcParams)
    {
        //Everything that is put here will only run in the server/host but not in client
        //The difference is that in ClientRPC, even if the player tries to press T, this would not do anything
        //This place is purely to send msg from Server to Client (maybe i can do pause?)
        //Special: this function params allow server to send data to one or more particular ClientId

        //targetClientId
    }

    [ServerRpc(RequireOwnership = false)]
    public void NotifyReadyServerRpc()
    {
        CattibalGameManager.Instance.registerPlayer();

        if(CattibalGameManager.Instance.numOfPlayers == CattibalGameManager.Instance.totalPlayers)
        {
            NotifyClientsReadyClientRpc();
        }
    }

    [ClientRpc]
    public void NotifyClientsReadyClientRpc()
    {
        CattibalGameManager.Instance.numOfPlayers = CattibalGameManager.Instance.totalPlayers;
    }

    private Material GetCatSkins(CattibalLobbyManager.PlayerSkin playerSkin)
    {
        switch (playerSkin)
        {
            default:
            case CattibalLobbyManager.PlayerSkin.Tuxedo: return catSkins[0];
            case CattibalLobbyManager.PlayerSkin.Blue: return catSkins[1];
            case CattibalLobbyManager.PlayerSkin.Calico: return catSkins[2];
            case CattibalLobbyManager.PlayerSkin.Siamese: return catSkins[3];
        }
    }
}

