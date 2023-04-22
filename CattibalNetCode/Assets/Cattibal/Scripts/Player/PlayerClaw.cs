using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClaw : MonoBehaviour
{
    public PlayerNetwork owner;
    PlayerNetwork attackedTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public PlayerNetwork GetHitTarget()
    {
        PlayerNetwork temp = attackedTarget;
        attackedTarget = null;
        return temp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player")
        {
            return;
        }

        if(other.transform.parent == null)
        {
            return;
        }

        attackedTarget = other.transform.parent.GetComponent<PlayerNetwork>();
        
        if(attackedTarget.OwnerClientId == owner.OwnerClientId)
        {
            attackedTarget = null;
            return;
        }
        
        if(attackedTarget != null)
        {
            enabled = false;
        }
    }
}
