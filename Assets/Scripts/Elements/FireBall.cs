using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    private bool playerDir; // [True] Left : [False] Right
    private float initPos;

    private void Awake()
    {
        playerDir = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>().flipX;
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
        if (collision.gameObject.tag == "Flammable")
        {
            collision.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            // TO DO
            Debug.Log("Enemy Hit");
            Destroy(this.gameObject);
        }
    }
}
