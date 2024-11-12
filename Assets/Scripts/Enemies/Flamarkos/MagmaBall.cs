using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagmaBall : MonoBehaviour
{
    public GameObject flamarkos;
    public Vector2 playerPos;

    private float magmaSpeed = 5f;
    private Vector2 origin;
    private Vector2 direction;

    private void Start()
    {
        origin = transform.position;
        direction = (playerPos - origin).normalized;
    }

    private void FixedUpdate()
    {
        transform.Translate(magmaSpeed * Time.deltaTime * direction);

        if (Vector2.Distance(origin, transform.position) >= 5)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != flamarkos)
            Destroy(gameObject);
    }
}
