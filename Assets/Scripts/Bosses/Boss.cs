using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [HideInInspector] public float maxLife = 100f;
    [HideInInspector] public float life;

    [HideInInspector] public bool changingPhase = false;

    protected GameObject player;

    public static event Action<string> Restart;

    protected void Start()
    {
        life = maxLife;

        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void UpdateLife(float damage)
    {
        if (changingPhase) return;

        life += damage;
        if (life <= 0f)
        {
            player.GetComponent<HealthManager>().AddHeart();
            player.GetComponent<AuraManager>().AddAura();
            Restart?.Invoke("WIN");
            Destroy(this.gameObject);
        }
    }
}
