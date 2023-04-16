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

    [SerializeField] private Image damagedOverlay;
    [SerializeField] private Image healededOverlay;
    private float duration = 0.8f;
    private float fadeSpeed = 1.5f;
    private float durationTimer;
    private HealthSystemArgs healthSystemArgument;

    private void Awake()
    {
    }

    private void Start()
    {
        //Debug.Log("visually, health is " + healthSystem.healthAmount);
        healthDamageBarImage.fillAmount = healthBarImage.fillAmount;
        damagedOverlay.color = new Color(damagedOverlay.color.r, damagedOverlay.color.g, damagedOverlay.color.b, 0);

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

        if(damagedOverlay.color.a > 0)
        {
            durationTimer += Time.deltaTime;
            if(durationTimer > duration)
            {
                float tempAlpha = damagedOverlay.color.a;
                tempAlpha = Time.deltaTime * fadeSpeed;
                damagedOverlay.color = new Color(damagedOverlay.color.r, damagedOverlay.color.g, damagedOverlay.color.b, tempAlpha);
            }
        }
        if (healededOverlay.color.a > 0)
        {
            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                float tempAlpha1 = healededOverlay.color.a;
                tempAlpha1 = Time.deltaTime * fadeSpeed;
                healededOverlay.color = new Color(healededOverlay.color.r, damagedOverlay.color.g, damagedOverlay.color.b, tempAlpha1);
            }
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

    public void DamagedOverlay()
    {
        durationTimer = 0;
        damagedOverlay.color = new Color(damagedOverlay.color.r, damagedOverlay.color.g, damagedOverlay.color.b, 1);
    }

    public void HealedOverlay()
    {
        durationTimer = 0;
        healededOverlay.color = new Color(healededOverlay.color.r, healededOverlay.color.g, healededOverlay.color.b, 1);
    }
}
