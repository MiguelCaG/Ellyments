using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    private GameObject boss;
    private Boss bossComponent;
    private string bossName;
    [HideInInspector] public float maxHealth;
    [HideInInspector] public float health;
    private float lerpSpeed = 0.05f;
    private Color originalColor;
    private TextMeshProUGUI bossNameText;

    private void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");
        bossComponent = boss.GetComponent<Boss>();
        bossName = boss.name;
        maxHealth = bossComponent.maxLife;
        healthSlider.maxValue = maxHealth;
        easeHealthSlider.maxValue = maxHealth;
        health = maxHealth;
        originalColor = healthSlider.GetComponentInChildren<Image>().color;

        bossNameText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        switch (bossName)
        {
            case "Ignarion":
                bossNameText.text = "Ignarion, Señor de las Llamas Eternas";
                bossNameText.color = new Color(0.5f, 0f, 0f);
                break;
            case "Zephyros":
                bossNameText.text = "Zephyros, Guardián de los Cielos";
                bossNameText.color = new Color(0f, 0.5f, 0.5f);
                break;
            case "Aqualis":
                bossNameText.text = "Aqualis, Emperatriz de las Profundidades";
                bossNameText.color = new Color(0f, 0f, 0.5f);
                break;
            case "Terrak":
                bossNameText.text = "Terrak, Coloso Durmiente del Abismo";
                bossNameText.color = new Color(0f, 0.5f, 0f);
                break;
        }
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
