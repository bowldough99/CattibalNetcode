using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystemArgs : EventArgs
{
    private float healthRatio;

    public HealthSystemArgs()
    {
        healthRatio = 0;
    }
    public HealthSystemArgs(float _healthRatio)
    {
        healthRatio = _healthRatio;
    }

    public void setHealth(float _healthRatio)
    {
        healthRatio = _healthRatio;
    }

    public float getHealth()
    {
        return healthRatio;
    }

}

public delegate void Test(object sender, HealthSystemArgs e);

//this is the logical script
public class HealthSystem : MonoBehaviour
{
    /*
    public enum HealthState
    {
        Health_100,
        Health_80,
        Health_60,
        Health_40,
        Health_20,
        Dead
    }
    public HealthState healthState;*/

    //private int health;

    public event EventHandler<HealthSystemArgs> OnDamaged;
    //public event EventHandler OnDamaged;
    public event EventHandler OnAttack;

    public int healthAmount;
    private int healthAmountMax;

    public HealthSystem()
    {
        healthAmount = 100;
        healthAmountMax = 100;
    }

    public HealthSystem(int _healthAmount)
    {
        healthAmountMax = _healthAmount;
        healthAmount = _healthAmount;
    }

    public int GetCurrentHealthState()
    {
        //function to return the current health
        //return healthState;
        return healthAmount;
    }

    /*
    public void SetHealthState(HealthState healthState)
    {
        //function to set the health
        this.healthState = healthState;
    }*/

    public void SetHealth(int _healthAmount)
    {
        //function to set the health
        healthAmount = _healthAmount;
    }

    public void SetMaxHealth(int _healthAmountMax)
    {
        //function to set the health
        healthAmountMax = _healthAmountMax;
    }


    //public void Hunger(int hungerAmount)
    //{
    //    hungerAmountMax = hungerAmount;
    //    this.hungerAmount = hungerAmount;
    //}

    public void Damage(int amount)
    {
        healthAmount -= amount;
        if (healthAmount < 0)
        {
            healthAmount = 0;
        }

        HealthSystemArgs newEvent = new HealthSystemArgs();
        newEvent.setHealth((float)healthAmount / healthAmountMax);

        OnDamaged?.Invoke(this, newEvent);
    }

    public void Ate(int amount)
    {
        healthAmount += amount;
        if (healthAmount > healthAmountMax)
        {
            healthAmount = healthAmountMax;
        }

        //hungerAmount -= amount;
        //if (hungerAmount < 0)
        //{
        //    hungerAmount = 0;
        //}
        OnAttack?.Invoke(this, EventArgs.Empty);
    }

    //public void Hungry(int amount)
    //{
    //    hungerAmount += amount;
    //    if (hungerAmount > hungerAmountMax)
    //    {
    //        hungerAmount = hungerAmountMax;
    //    }
    //}

    public float GetHealthNormalized()
    {
        return (float)healthAmount / healthAmountMax;
    }
}
