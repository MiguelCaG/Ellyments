using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // ENEMY EVENTS
    public event Action<Hit> Damage;
    public event Action Damaged;
    public event Action<GameObject> Killed;

    // ENEMY PROPERTIES
    protected Rigidbody2D rb;

    /// MOVING
    protected float speed = 1f;
    protected int direction = 1;
    [HideInInspector] public bool changeDirection;

    /// ATTACKING
    private GameObject hitController;
    protected float attackRange = 1f;
    protected float attackDamage = -1f;
    private Hit attackHit;

    /// TIMERS
    protected float iddleTime = 0f;
    protected float prepareAttack = 1f;
    protected float restAttack = 0f;

    /// HEALTH
    [HideInInspector] public float maxLife = 10f;
    [HideInInspector] public float currentLife;
    private GameObject healthBar;

    // PLAYER PROPERTIES
    private PlayerSeeker playerSeeker;
    protected Vector2 playerPos;
    protected bool playerInRange;
    protected int lookAtPlayer;

    // ELEMENTS PROPERTIES
    protected PlayerBehaviour.Element elemStrength = PlayerBehaviour.Element.None;
    protected PlayerBehaviour.Element elemWeakness = PlayerBehaviour.Element.None;

    protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        hitController = transform.GetChild(4).gameObject;
        attackHit = new Hit(hitController.transform.position, attackRange / 2f, attackDamage, PlayerBehaviour.Element.None);

        currentLife = maxLife;
        healthBar = transform.GetChild(0).gameObject;

        playerSeeker = transform.GetChild(3).gameObject.GetComponent<PlayerSeeker>();
        playerPos = Vector2.zero;
    }

    private void FixedUpdate()
    {
        playerPos = playerSeeker.playerPos;
        playerInRange = playerSeeker.playerInRange;
        lookAtPlayer = playerPos.x - transform.position.x >= 0 ? 1 : -1;
    }

    // PATROL STATE
    protected void Patrol()
    {
        if (changeDirection)
        {
            direction = direction == 1 ? -1 : 1;
        }
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);
        transform.localScale = new Vector2(direction, transform.localScale.y);
    }

    // ATTACK STATE
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
            hitController.GetComponent<SpriteRenderer>().enabled = false;
            hitController.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f);
            restAttack = Time.time + 5f;
            prepareAttack = restAttack + 2f;
        }
    }

    // CHASE STATE
    protected void Chase()
    {
        transform.localScale = new Vector2(lookAtPlayer, transform.localScale.y);
        rb.velocity = new Vector2(speed * 2f * lookAtPlayer, rb.velocity.y);
    }

    // DAMAGED
    public void UpdateLife(float life, PlayerBehaviour.Element elemAttack)
    {
        if (life < 0)
        {
            Damaged?.Invoke();
            if (!healthBar.activeSelf) healthBar.SetActive(true);
        }

        Color32 color = new Color32(188, 139, 0, 255);
        if (elemAttack == elemStrength)
        {
            life /= 2f;
            color = new Color32(186, 25, 0, 255);
        }
        else if (elemAttack == elemWeakness)
        {
            life *= 2f;
            color = new Color32(0, 185, 141, 255);
        }
        if (healthBar.activeSelf) healthBar.GetComponent<EnemyHealthBar>().fillEase.color = color;

        if (currentLife + life > maxLife)
            currentLife = maxLife;
        else
            currentLife += life;
        Debug.Log($"{currentLife}/{maxLife}");
        if (currentLife <= 0f)
        {
            Killed?.Invoke(gameObject);
            Destroy(gameObject);
        }
    }

    // PLAYER HIT
    protected void HurtPlayer(Hit hit)
    {
        Damage?.Invoke(hit);
    }
}