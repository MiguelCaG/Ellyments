using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private PlayerActionsData pAD;

    private GameObject player;
    private bool playerDir; // [True] Left : [False] Right
    private float initPos;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerDir = player.GetComponent<SpriteRenderer>().flipX;
        GetComponent<SpriteRenderer>().flipX = playerDir;
        initPos = transform.position.x;
    }

    private void Update()
    {
        if (playerDir)
            transform.position -= new Vector3(10, 0) * Time.deltaTime;
        else
            transform.position += new Vector3(10, 0) * Time.deltaTime;

        if (Mathf.Abs(initPos - transform.position.x) >= 20f)
            Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Flammable"))
        {
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss"))
        {
            pAD.attackingDistance++;
            pAD.aggressiveness += 0.1f;
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.GetComponent<Enemy>().UpdateLife(player.GetComponent<PlayerBehaviour>().fireBallDamage);
            }
            else if (collision.gameObject.CompareTag("Boss"))
            {
                collision.GetComponent<Boss>().UpdateLife(player.GetComponent<PlayerBehaviour>().fireBallDamage);
            }
        }

        if (collision.gameObject != player)
        {
            Destroy(this.gameObject);
        }
    }
}
