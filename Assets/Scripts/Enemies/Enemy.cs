using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public event Action<Hit> Damage;
    public event Action Damaged;

    protected Rigidbody2D rb;

    private GameObject hitController;
    protected float attackRange = 1f;
    protected float attackDamage = -1f;
    private Hit attackHit;

    private PlayerSeeker playerSeeker;
    protected Vector2 playerPos;
    protected bool playerInRange;
    protected int lookAtPlayer;

    protected float speed = 1f;
    protected int direction = 1;
    [HideInInspector] public bool changeDirection;

    protected float iddleTime = 0f;
    protected float prepareAttack = 1f;
    protected float restAttack = 0f;

    protected float maxLife = 10f;
    protected float currentLife;

    protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentLife = maxLife;
        playerPos = Vector2.zero;
        playerSeeker = transform.GetChild(2).gameObject.GetComponent<PlayerSeeker>();
        hitController = transform.GetChild(3).gameObject;
        attackHit = new Hit(hitController.transform.position, attackRange / 2f, attackDamage);
    }

    private void FixedUpdate()
    {
        playerPos = playerSeeker.playerPos;
        playerInRange = playerSeeker.playerInRange;

        lookAtPlayer = playerPos.x - transform.position.x >= 0 ? 1 : -1;
    }

    protected void Patrol()
    {
        if (changeDirection)
        {
            direction = direction == 1 ? -1 : 1;
        }
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);
        transform.localScale = new Vector2(direction, transform.localScale.y);
    }

    protected void Attack()
    {
        transform.localScale = new Vector2(lookAtPlayer, transform.localScale.y);
        // TEMPORAL
        hitController.GetComponent<SpriteRenderer>().enabled = true;
        hitController.GetComponent<SpriteRenderer>().color += new Color(0.005f, 0f, 0f);
        /////////////
        if (prepareAttack <= Time.time)
        {
            attackHit.SetHitOrigin(hitController.transform.position);
            HurtPlayer(attackHit);
            hitController.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f);
            restAttack = Time.time + 5f;
            prepareAttack = restAttack + 2f;
        }
    }

    protected void Chase()
    {
        transform.localScale = new Vector2(lookAtPlayer, transform.localScale.y);
        rb.velocity = new Vector2(speed * 2f * lookAtPlayer, rb.velocity.y);
    }

    public void UpdateLife(float life)
    {
        currentLife += life;
        Debug.Log($"{currentLife}/{maxLife}");
        if (currentLife <= 0f)
        {
            Destroy(this.gameObject);
        }

        if (life < 0)
        {
            Damaged?.Invoke();
        }
    }

    protected void HurtPlayer(Hit hit)
    {
        Damage?.Invoke(hit);
    }
}
