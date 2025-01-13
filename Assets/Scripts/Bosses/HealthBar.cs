using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    private GameObject boss;
    private Boss bossComponent;
    [HideInInspector] public float maxHealth;
    [HideInInspector] public float health;
    private float lerpSpeed = 0.05f;
    private Color originalColor;

    private void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");
        bossComponent = boss.GetComponent<Boss>();
        maxHealth = bossComponent.maxLife;
        healthSlider.maxValue = maxHealth;
        easeHealthSlider.maxValue = maxHealth;
        health = maxHealth;
        originalColor = healthSlider.GetComponentInChildren<Image>().color;
    }

    private void Update()
    {
        health = boss != null ? bossComponent.life : 0f;
        if (bossComponent.changingPhase) healthSlider.GetComponentInChildren<Image>().color = Color.gray;
        else healthSlider.GetComponentInChildren<Image>().color = originalColor;

        if (healthSlider.value != health)
        {
            healthSlider.value = health;
        }

        if (healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, lerpSpeed);
        }
    }
}
