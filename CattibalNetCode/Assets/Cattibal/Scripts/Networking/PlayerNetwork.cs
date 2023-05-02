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
using DissolveExample;

[RequireComponent(typeof(NetworkTransform))]
public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private Transform spawnedObjectPrefab; //GameObject spawnedObjectPrefab also works but limitation
    private Transform spawnedObjectTransform;
    bool moveToSpawn = false;

    [Header("Player Settings")]
    public HealthBar healthBar;
    public HungerBar hungerBar;
    public LivesHunger livesHungerBar;

    private const int defaultHp = 90;
    private const int defaultHunger = 100;
    private const int defaultLives = 9;

    private NetworkVariable<int> hp = new NetworkVariable<int>(defaultHp);
    private NetworkVariable<int> hunger = new NetworkVariable<int>(defaultHunger);
    private NetworkVariable<int> lives = new NetworkVariable<int>(defaultLives);
    private NetworkVariable<int> newHunger = new NetworkVariable<int>(9);

    private const float HUNGER_INCREASE_TIMER_MAX = 0.5f;
    private const float HEALTH_DECREASE_TIMER_MAX = 5f;
    private float hungerIncreaseTimer;
    private float healthDecreaseTimer;

    private float timerToHungry = 5f;
    private float timerToStarve = 5f;


    [SerializeField] private Transform playerHand;
    [SerializeField] private float minimumAttackDistance = 1.0f;

    [Header("Game Settings")]
    [SerializeField] TextMeshPro endGameText;
    private bool _isAlive = true;
    public bool IsAlive => lives.Value > 0;

    private int skinNumberToSpawn;

    [SerializeField] private Material[] catSkins;
    [SerializeField] private Material skinMaterialToUse;
    [SerializeField] private GameObject spawnedPlayerPrefab;

    private PlayerMovement playerMovement;
    private CattibalGameManager gameManager;
    [SerializeField] private DissolveChilds dissolve;
    float tParam = 0;
    float valToBeLerped = 0;
    float speed = 0.5f;

    private NetworkVariable<MyCustomData> playerData = new NetworkVariable<MyCustomData>(
        new MyCustomData {
            _int = 0,
            _bool = false,
            _float = 0.2f,
            _health = 100,
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

        if (healthBar == null)
        {
            healthBar = GameObject.FindObjectOfType<HealthBar>(true);
        }
        if (hungerBar == null)
        {
            hungerBar = GameObject.FindObjectOfType<HungerBar>(true);
        }
        if (livesHungerBar == null)
        {
            livesHungerBar = GameObject.FindObjectOfType<LivesHunger>(true);
        }
        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }
        if (gameManager == null)
        {
            gameManager = GameObject.FindObjectOfType<CattibalGameManager>(true);
        }

        SkinnedMeshRenderer meshRenderer = spawnedPlayerPrefab.GetComponent<SkinnedMeshRenderer>();
        meshRenderer.material = catSkins[Random.Range(0, catSkins.Length)];

        if (IsOwner)
        {
            GameObject.FindObjectOfType<TutorialUI>(true).player = this;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsClient)
        {
            //lives.OnValueChanged -= OnLivesChanged;
        }
    }


    private void Update()
    {
        if(lives.Value <= 0)
        {
            DissolveClientRpc();
            CattibalGameManager.Instance.KillPlayer(OwnerClientId);
        }

        if (!moveToSpawn)
        {
            if(IsOwner)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    Debug.Log(string.Format("move {0}", OwnerClientId));
                    transform.position = gameManager.GetComponent<CattibalGameManager>().getSpawnPoint((int)OwnerClientId);
                }
                else
                {
                    SetSpawnPointServerRpc();
                }
            }
            moveToSpawn = true;
        }

        if (!_isAlive)
        {
        }
        if (CattibalGameManager.Instance.HasGameTimerEned())
        {
            lives.Value = 0;
        }
        if (!IsOwner) return;



        if (CattibalGameManager.Instance.IsWinner(OwnerClientId))
        {
            if (!GameOverUI.instance.gameObject.activeSelf)
            {
                GameOverUI.instance.gameObject.SetActive(true);
                GameOverUI.instance.ActivateVictory();
            }
        }

        if (!CattibalGameManager.Instance.IsGamePlaying()) return;

        if(lives.Value <= 0)
        {
            //_isAlive = false;
            if (!GameOverUI.instance.gameObject.activeSelf)
            {
                GameOverUI.instance.gameObject.SetActive(true);
                GameOverUI.instance.ActivateDefeat();
            }
        }

        if (newHunger.Value > 0)
        {
            livesHungerBar.VeryHungry = false;
            timerToHungry -= Time.deltaTime;
            if (timerToHungry > 0)
            {
                //this timer blinks the next hunger icon to lose
                livesHungerBar.Hungry = true;
            }
            if (timerToHungry <= 0)
            {
                //this resets the blinking of next hunger icon
                HungerServerRPC("losing hunger by", 1, this.OwnerClientId);
                timerToHungry = 10f;
            }
        }
        else
        {
            livesHungerBar.VeryHungry = true;
        }

        if (livesHungerBar.VeryHungry)
        {
            timerToStarve -= Time.deltaTime;
            if(timerToStarve > 0)
            {

            }
            if(timerToStarve <= 0)
            {
                HealthServerRPC("dying by", 1, this.OwnerClientId);
                timerToStarve = 5f;
            }
        }
        else
        {
            timerToStarve = 5f;
        }

        //healthBar.updateHP(hp.Value / 100.0f);
        //hungerBar.updateHunger(hunger.Value / 100.0f);
        //livesHungerBar.UpdateLivesHunger(lives.Value, newHunger.Value);
        Debug.Log(newHunger.Value);

        if (playerMovement.IsAttacking == true)
        {
            CheckPunch(playerHand.transform);
        }

    }

    private void CheckPunch(Transform hand)
    {
        // new attack code
        PlayerNetwork target = playerMovement.ClawBox.GetHitTarget();
        if(target != null)
        {
            Debug.Log("NEW ATTACK CODE HIIIT");
            UpdateHealthServerRPC(1, target.OwnerClientId);
            playerMovement.OnHitScratched();
            //target.HealthSourceServerRpc(-20, (int)OwnerClientId);
        }

        return;
        /*
        Debug.Log("DETROIT SSSSMAAAASSHH");
        RaycastHit hit;

        int layerMask = LayerMask.GetMask("Character");

        if (Physics.Raycast(hand.position, hand.transform.forward, out hit, minimumAttackDistance, layerMask))
        {
            Debug.DrawRay(hand.position, hand.transform.forward * minimumAttackDistance, Color.yellow);
            var playerHit = hit.transform.parent.GetComponent<NetworkObject>();
            if (playerHit != null)
            {
                playerMovement.OnHitScratched();
                UpdateHealthServerRPC(10, playerHit.OwnerClientId); // QQ i think this is being called twice? should this be removed?
                Debug.Log(playerHit.OwnerClientId);
                healthBar.HealedOverlay();
            }
            Debug.Log("raycast hitting");
        }
        else
        {
            Debug.DrawRay(hand.position, hand.transform.forward * minimumAttackDistance, Color.red);
            Debug.Log("raycast NOT hitting");
        }
        */
    }

    [ServerRpc(RequireOwnership = false)]
    public void HealthSourceServerRpc(int healthChange, int id)
    {
        hp.Value += healthChange;
        if (hp.Value > 100)
        {
            hp.Value = 100;
        }

        if (hp.Value <= 0 || lives.Value <= 0)
        {
            _isAlive = false;
            hp.Value = 0;
            lives.Value = 0;
            CattibalGameManager.Instance.KillPlayer(OwnerClientId);

            if (id < 0)
            {
                //UIKillMessages.instance.AddStarveMessage(OwnerClientId.ToString());
                NotifyStarveClientRpc(OwnerClientId.ToString());
            }
            else
            {
                //UIKillMessages.instance.AddKillMessage(id.ToString(), OwnerClientId.ToString());
                NotifyKillClientRpc(id.ToString(), OwnerClientId.ToString());
            }
        }
        healthBar.DamagedOverlay();
    }

    [ClientRpc]
    public void NotifyKillClientRpc(string killer, string killee)
    {
        UIKillMessages.instance.AddKillMessage(killer, killee);
    }

    [ClientRpc]
    public void NotifyStarveClientRpc(string victim)
    {
        UIKillMessages.instance.AddStarveMessage(victim);
    }

    [ServerRpc]
    private void UpdateHealthServerRPC(int healthDamaged, ulong clientId)
    {
        var clientDamaged = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerNetwork>();
        if (clientDamaged != null && clientDamaged.lives.Value > 0 && clientDamaged.lives.Value <= 9)
        {
            //clientDamaged.hp.Value -= healthDamaged;
            //clientDamaged.NotifyDamageClientRpc(-healthDamaged, (int)OwnerClientId);
            clientDamaged.DamageClient(-healthDamaged, (int)OwnerClientId);
            //clientDamaged.lives.Value -= 1;

            if (clientDamaged.IsOwner)
            {
                clientDamaged.healthBar.DamagedOverlay(); //QQ is this supposed to be where the client who got hit get the damaged overlay??
            }
            else
            {
                clientDamaged.DamagedOverlayClientRpc();
            }
            if (clientDamaged.lives.Value <= 0)
            {
                clientDamaged._isAlive = false; //QQ if i want to let the game know this particular client died, is it here? coz this seems to be causing the problem in line 360
            }
        }
        else
        {
            Debug.Log(string.Format("which lost child is this. {0}", clientId));
        }

        clientDamaged.NotifyHealthChangedClientRpc(healthDamaged, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        });
        clientDamaged.livesHungerBar.Loselife(lives.Value);

        Debug.Log("health changing");
        //healthBar.HealedOverlay(); //QQ i think this one is correctly showing? but the one on top shouldnt be showing on the person who attack.
        //HealOverlayClientRpc();
    }

    [ClientRpc]
    public void DamagedOverlayClientRpc()
    {
        if(IsOwner)
        {
            healthBar.DamagedOverlay();
        }
    }

    [ClientRpc]
    public void DissolveClientRpc()
    {
        //QQ idgi, why is this only happening if its the client killing a host?? but when host kill client, the client dont dissolve
        dissolve.SetValue(tParam);
        if (tParam < 1)
        {
            tParam += Time.deltaTime * speed;
            valToBeLerped = Mathf.Lerp(0, 1, tParam);
        }
    }

    public void DamageClient(int damage, int source)
    {
        if(NetworkManager.Singleton.IsServer)
        {
            NotifyDamageClientRpc(damage, source);

            lives.Value += damage;
            if (lives.Value > 9)
            {
                lives.Value = 9;
            }

            if (lives.Value <= 0)
            {
                lives.Value = 0;
                _isAlive = false;
                if (source < 0)
                {
                    //UIKillMessages.instance.AddStarveMessage(OwnerClientId.ToString());
                    NotifyStarveClientRpc(OwnerClientId.ToString());
                }
                else
                {
                    //UIKillMessages.instance.AddKillMessage(id.ToString(), OwnerClientId.ToString());
                    NotifyKillClientRpc(source.ToString(), OwnerClientId.ToString());
                }
            }
        }
    }

    [ClientRpc]
    public void NotifyDamageClientRpc(int damage, int source)
    {
        //HealthSourceServerRpc(damage, source);

        //hp.Value += damage; // no need to set hp for client
        if (hp.Value > 100)
        {
            //hp.Value = 100;
        }

        if (hp.Value <= 0)
        {
            //hp.Value = 0;
            if(IsOwner)
            {
            }
        }
    }

    [ClientRpc]
    public void NotifyHealthChangedClientRpc(int healthDamaged, ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        livesHungerBar.LoseHunger(newHunger.Value);
        livesHungerBar.Loselife(lives.Value);
        //Debug.Log("Got punched" + healthDamaged);
    }


    //////////////////////////////////////////////////////GRACE COPY TO TRY//////////////////////////////////////////////////
    
    [ServerRpc]
    public void HealthServerRPC(string message, int healthChange, ulong clientId)
    {
        var clientDamaged = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerNetwork>();
        if (clientDamaged != null && clientDamaged.lives.Value > 0 && clientDamaged.lives.Value <= 9)
        {
            //clientDamaged.lives.Value -= healthChange;
            clientDamaged.GraceDamageClient(-healthChange);
            clientDamaged.livesHungerBar.Loselife(lives.Value);
            if (clientDamaged.IsOwner)
            {
                clientDamaged.healthBar.DamagedOverlay(); //QQ is this supposed to be where the client who got hit get the damaged overlay??
            }
            else
            {
                clientDamaged.DamagedOverlayClientRpc();
            }
        }
        else
        {
            Debug.Log(string.Format("which lost child is this. {0}", clientId));
        }

        NotifyHealthChangedClientRpc(healthChange, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        });
    }

    [ServerRpc]
    public void HungerServerRPC(string message, int amount, ulong clientId)
    {
        var clientDamaged = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerNetwork>();
        if (clientDamaged != null && clientDamaged.newHunger.Value > 0 && clientDamaged.newHunger.Value <= 9)
        {
            //clientDamaged.newHunger.Value -= amount;
            clientDamaged.GraceHungerClient(-amount);
            livesHungerBar.LoseHunger(newHunger.Value);
            if (clientDamaged.IsOwner)
            {
                //clientDamaged.healthBar.HealedOverlay(); //QQ is this supposed to be where the client who got hit get the damaged overlay??
            }
            else
            {
                //clientDamaged.HealOverlayClientRpc();
            }
        }
        else
        {
            Debug.Log(string.Format("which lost child is this. {0}", clientId));
        }

        NotifyHealthChangedClientRpc(amount, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        });
    }
    [ServerRpc]
    public void RegainHungerServerRPC(string message)
    {
        Debug.Log("HungerServerRPC" + OwnerClientId + "; " + message);
        newHunger.Value = 9;
        livesHungerBar.RegainFullness();
    }
    public void GraceDamageClient(int damage)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            lives.Value += damage;
            if (lives.Value > 9)
            {
                lives.Value = 9;
            }

            if (lives.Value <= 0)
            {
                lives.Value = 0;
                NotifyStarveClientRpc(OwnerClientId.ToString());
            }
        }
    }
    public void GraceHungerClient(int damage)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            newHunger.Value += damage; //or is it coz of this?
            if (newHunger.Value > 9)
            {
                newHunger.Value = 9;
            }

            if (newHunger.Value <= 0)
            {
                newHunger.Value = 0;
            }
        }
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

    [ClientRpc]
    public void HealOverlayClientRpc()
    {
        healthBar.HealedOverlay();
    }

    [ServerRpc]
    public void SetSpawnPointServerRpc()
    {
        SetSpawnPointClientRpc(gameManager.GetComponent<CattibalGameManager>().getSpawnPoint((int)OwnerClientId));
    }

    [ClientRpc]
    public void SetSpawnPointClientRpc(Vector3 position)
    {
        transform.position = position;
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

