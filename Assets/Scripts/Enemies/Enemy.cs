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
    protected SpriteRenderer sr;
    protected Animator enemyAnim;

    /// MOVING
    protected float speed = 1f;
    protected int direction = 1;
    [HideInInspector] public bool changeDirection;

    /// ATTACKING
    private GameObject hitController;
    protected float attackRange = 1.2f;
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

    [SerializeField] private AudioClip hit;
    [SerializeField] private AudioClip hurt;

    protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        enemyAnim = GetComponent<Animator>();

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
        ChangeAnim("EnemyPatrol", "Patrol");

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

        ChangeAnim("EnemyPreparedAttack", "Attack");

        if (prepareAttack <= Time.time)
        {
            attackHit.SetHitOrigin(hitController.transform.position);
            HurtPlayer(attackHit);
            restAttack = Time.time + 2f;
            prepareAttack = restAttack + 1f;

            SoundFXManager.instance.PlaySoundFXClip(hit, transform, 1f);
        }
    }

    // CHASE STATE
    protected void Chase()
    {
        ChangeAnim("EnemyRun", "Chase");

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

            SoundFXManager.instance.PlaySoundFXClip(hurt, transform, 1f);
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

    protected void ChangeAnim(string animation, string trigger)
    {
        if (!enemyAnim.GetCurrentAnimatorStateInfo(0).IsName(animation)) enemyAnim.SetTrigger(trigger);
    }
}