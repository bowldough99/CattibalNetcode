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
    public bool Hungry = false;
    public bool VeryHungry = false;
    public void Loselife(int amt)
    {
        if(livesRemaining == 0) return;
        StartCoroutine(FadeImage(lives[amt].gameObject));
        if(livesRemaining == 0)
        {
            //DEAD
        }
    }

    public void LoseHunger(int amt)
    {
        StartCoroutine(FadeImage(hunger[amt].gameObject));
        if (hungerRemaining == 0 && livesRemaining != 0)
        {
            VeryHungry = true;
        }
    }

    public void RegainFullness()
    {
        VeryHungry = false;
        for (int i = 0; i < 9; i++)
        {
            hunger[i].color = new Color(1f, 1f, 1f, 1f);
            hunger[i].enabled = true;
        }
    }
    public void UpdateLivesHunger(int life, int hungry)
    {
        livesRemaining = life;
        hungerRemaining = hungry;
    }

    IEnumerator FadeImage(GameObject obj)
    {
        //yield return new WaitForSeconds(delay);
        Image image = obj.GetComponent<Image>();
        while (image.color.a > 0)
        {
            image.color -= new Color(0, 0, 0, 1) * Time.deltaTime;
            yield return null;
        }
        image.enabled = false;
    }

    private void Update()
    {
        if (VeryHungry && livesRemaining > 0)
        {
            lives[livesRemaining - 1].color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.time * 0.8f, 1));
        }
        else if (VeryHungry = false && livesRemaining > 0)
        {
            lives[livesRemaining - 1].color = new Color(1f, 1f, 1f, 1f);
        }
        if (Hungry && hungerRemaining > 0)
        {
            hunger[hungerRemaining - 1].color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.time * 0.8f, 1));
        }
    }
}
