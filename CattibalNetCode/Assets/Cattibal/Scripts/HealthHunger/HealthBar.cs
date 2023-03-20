using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;
using System;

//this is a visual script
public class HealthBar : MonoBehaviour
{

    private const float HEALTH_DAMAGED_SHRINK_TIMER_MAX = 1f;

    [SerializeField] private Image healthBarImage;
    [SerializeField] private Image healthDamageBarImage;
    private float healthDamageShrinkTimer;

    private HealthSystemArgs healthSystemArgument;

    private void Awake()
    {
    }

    public void Setup(ulong ID)
    {
    }

    private void Start()
    {
        //Debug.Log("visually, health is " + healthSystem.healthAmount);
        healthDamageBarImage.fillAmount = healthBarImage.fillAmount;
    }
    private void Update()
    {
        healthDamageShrinkTimer -= Time.deltaTime;
        if (healthDamageShrinkTimer < 0)
        {
            if (healthBarImage.fillAmount < healthDamageBarImage.fillAmount)
            {
                float shrinkSpeed = 1f;
                healthDamageBarImage.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
        }

        if (healthDamageBarImage.fillAmount < healthBarImage.fillAmount)
        {
            healthDamageBarImage.fillAmount = healthBarImage.fillAmount;
        }
    }

    private void HealthSystem_OnDamaged(object sender, HealthSystemArgs e)
    {
        healthDamageShrinkTimer = HEALTH_DAMAGED_SHRINK_TIMER_MAX;
        SetHealth(e.getHealth());
        Debug.Log("player damaged");
    }
    private void HealthSystem_OnAttack(object sender, System.EventArgs e)
    {
        healthDamageBarImage.fillAmount = healthBarImage.fillAmount;
    }
    private void SetHealth(float healthNormalized)
    {
        healthBarImage.fillAmount = healthNormalized;
    }


    public void updateHP(float healthNormalized)
    {
        healthBarImage.fillAmount = healthNormalized;
    }
}
