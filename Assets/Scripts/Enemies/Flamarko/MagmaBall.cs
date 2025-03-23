using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagmaBall : MonoBehaviour
{
    private GameObject flamarko;
    private Vector2 playerPos;

    private float magmaSpeed = 5f;
    private int magmaDamage = -1;
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

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void FixedUpdate()
    {
        transform.Translate(magmaSpeed * Time.deltaTime * Vector3.right);

        if (Vector2.Distance(origin, transform.position) >= 5)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //HealthManager hm = collision.gameObject.GetComponent<HealthManager>();
            //Debug.Log("Objeto colisionado: " + collision.gameObject.name + " ¿Tiene HealthManager?: " + (hm != null));
            collision.gameObject.GetComponent<HealthManager>().UpdateLife(magmaDamage);
        }

        if (collision.gameObject != flamarko)
            Destroy(gameObject);
    }
}
