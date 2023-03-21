using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using UnityEngine;

public class NetworkMessenger : MonoBehaviour
{
    public static NetworkMessenger Instance;

    NetworkDriver driver;

    private void Awake()
    {
        Instance = this;
    }

    public void InitializeMessenger(RelayServerData relaydata)
    {
        NetworkSettings settings = new NetworkSettings();
        settings.WithRelayParameters(ref relaydata);
        driver = NetworkDriver.Create(settings);

        if (driver.Bind(NetworkEndPoint.AnyIpv4) == 0)
        {
            Debug.Log("Can send DMs already");
        }
        else
        {
            Debug.Log("Rip, cannot send DMs");
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!driver.IsCreated || !driver.Bound)
        {
            return;
        }

    }
}
