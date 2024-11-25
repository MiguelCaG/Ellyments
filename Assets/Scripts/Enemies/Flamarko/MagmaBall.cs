using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagmaBall : MonoBehaviour
{
    private GameObject flamarko;
    private Vector2 playerPos;

    private float magmaSpeed = 5f;
    private Vector2 origin;
    private Vector2 direction;

    public void Initialize(GameObject flamarko, Vector2 playerPos)
    {
        this.flamarko = flamarko;
        this.playerPos = playerPos;
    }

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
        if (collision.gameObject.CompareTag("Player"))
            collision.gameObject.GetComponent<PlayerBehaviour>().UpdateLife(flamarko.GetComponent<Flamarko>().magmaBallDamage);

        if (collision.gameObject != flamarko)
            Destroy(gameObject);
    }
}
