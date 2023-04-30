using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesHunger : MonoBehaviour
{
    public Image[] lives;
    public int livesRemaining;
    public Image[] hunger;
    public int hungerRemaining;

    public void Loselife()
    {
        livesRemaining--;
        lives[livesRemaining].enabled = false; 
    }

    public void LoseHunger()
    {
        hungerRemaining--;
        hunger[hungerRemaining].enabled = false;
    }

    private void Update()
    {
        
    }
}
