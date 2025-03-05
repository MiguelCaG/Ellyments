using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    [HideInInspector] public float maxHealth;
    [HideInInspector] public float health;
    private float lerpSpeed = 0.05f;
    private bool heal = false;
    public Image fillEase;

    private void Start()
    {
        maxHealth = transform.GetComponentInParent<Enemy>().maxLife;
        healthSlider.maxValue = maxHealth;
        easeHealthSlider.maxValue = maxHealth;
        health = maxHealth;
    }

    private void Update()
    {
        health = transform.GetComponentInParent<Enemy>().currentLife;

        if (healthSlider.value > health)
        {
            healthSlider.value = health;
            heal = false;
        }
        else if (health > healthSlider.value)
        {
            easeHealthSlider.value = health;
            heal = true;
        }

        if (easeHealthSlider.value > healthSlider.value)
        {
            if (!heal) easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, lerpSpeed);
            else healthSlider.value = Mathf.Lerp(healthSlider.value, easeHealthSlider.value, lerpSpeed);
        }
    }
}
