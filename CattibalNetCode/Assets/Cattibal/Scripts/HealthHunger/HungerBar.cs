using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;

//this is a visual script
public class HungerBar : MonoBehaviour
{
    private const float HUNGER_INCREASE_TIMER_MAX = 1f;
    [SerializeField] private Image hungerBarImage;
    private float hungerIncreaseTimer;

    private void Awake()
    {
        //hungerBarImage = transform.Find("HungerBarFill").GetComponent<Image>();
        hungerBarImage.fillAmount = 0;
    }

    private void Start()
    {
    }
    private void Update()
    {
    }
    
    public void updateHunger(float normalizedHunger)
    {
        //hungerBarImage = GameObject.Find("HungerBarFill").GetComponent<Image>();
        hungerBarImage.fillAmount = normalizedHunger;
    }
}
