using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingObject : MonoBehaviour
{
    private float flyingSpeed = 6f;
    private float direction;
    private float limit;

    public static event Action LimitReached;
    public static event Action<float> PlayerHit;

    public void Initialize(float direction, float limit)
    {
        this.direction = direction;
        this.limit = limit;
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(transform.position.x - limit) > 0.1f)
        {
            transform.position += new Vector3(flyingSpeed * Time.deltaTime * direction, 0f, 0f);
        }
        else
        {
            LimitReached?.Invoke();
            Destroy(gameObject);
        }
            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<HealthManager>().UpdateLife(-1);
            PlayerHit?.Invoke(0.1f);
        }
    }
}
