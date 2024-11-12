using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Rigidbody2D rb;

    private PlayerSeeker playerSeeker;
    protected Vector2 playerPos;
    protected bool playerInRange;
    protected int lookAtPlayer;

    protected float speed = 1f;
    protected int direction = 1;
    public bool changeDirection;

    protected float maxLife;
    protected float currentLife;

    protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentLife = maxLife;
        playerPos = Vector2.zero;
        playerSeeker = transform.GetChild(2).gameObject.GetComponent<PlayerSeeker>();
    }

    private void FixedUpdate()
    {
        playerPos = playerSeeker.playerPos;
        playerInRange = playerSeeker.playerInRange;

        lookAtPlayer = playerPos.x - transform.position.x >= 0 ? 1 : -1;
    }

    public void Patrol()
    {
        if (changeDirection)
        {
            direction = direction == 1 ? -1 : 1;
        }
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);
        transform.localScale = new Vector2(direction, transform.localScale.y);
    }

    public void UpdateLife(float life)
    {
        currentLife += life;
        Debug.Log($"{currentLife}/{maxLife}");
        if (currentLife <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
