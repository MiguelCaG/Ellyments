using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    private int health;
    private int heartsCount;

    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    private Animator anim;

    public static event Action<bool> Die;

    private void Start()
    {
        health = 5;
        heartsCount = health;

        anim = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < heartsCount)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    public int GetHealth() { return health; }

    public void AddHeart(int add = 1)
    {
        heartsCount += add;
        health = heartsCount;
    }

    public void UpdateLife(int life)
    {
        if (life < 0 && anim.GetCurrentAnimatorStateInfo(1).IsName("Hit")) return;
        
        if (health != 0)
            health = Mathf.Clamp(health + life, 0, heartsCount);

        if (life < 0)
            Die?.Invoke(health == 0);
    }
}
