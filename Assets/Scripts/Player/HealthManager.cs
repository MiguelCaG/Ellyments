using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private PlayerData pD;

    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    private Animator anim;

    public static event Action<bool> Die;

    [SerializeField] private AudioClip hurt;
    [SerializeField] private AudioClip heal;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < pD.health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < pD.heartsCount)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    public int GetHealth() { return pD.health; }
    public int GetMaxHealth() { return pD.heartsCount; }

    public void AddHeart(int add = 1)
    {
        pD.heartsCount += add;
        pD.health = pD.heartsCount;

        pD.SaveState();
    }

    public void UpdateLife(int life)
    {
        if (life < 0 && anim.GetCurrentAnimatorStateInfo(1).IsName("Hit")) return;

        if (pD.health != 0)
        {
            pD.health = Mathf.Clamp(pD.health + life, 0, pD.heartsCount);
            pD.SaveState();
        }

        if (life < 0)
        {
            SoundFXManager.instance.PlaySoundFXClip(hurt, transform, 1f);
            Die?.Invoke(pD.health == 0);
        }
        else if (life > 0 && pD.health != pD.heartsCount)
        {
            SoundFXManager.instance.PlaySoundFXClip(heal, transform, 1f);
        }
    }
}
