using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AbilityManager;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] PlayerActionsData pAD;

    public enum Element
    {
        Fire,
        Water,
        Earth,
        Air,
        None
    }

    public Element currentElement = Element.None;

    public enum State
    {
        Idle,
        Moving,
        Jumping,
        Dashing,
        Diying,
        Immobile
    }

    public State currentState = State.Idle;

    private AbilityManager abilityM;
    private HealthManager healthM;

    private bool radialMenuOpen = false;

    private float runSpeed = 5;
    private float jumpSpeed = 5.5f;
    private float dashSpeed = 50;
    private bool jumpPressed = false;
    [HideInInspector] public bool onGround;
    private bool doubleJump = false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private BoxCollider2D bc;
    private Animator playerAnim;

    [SerializeField] private GameObject fireBall;
    private BoxCollider2D checkCollision;
    private GameObject hitController;

    private float normalDamage = -5f;
    private float rockPunchDamage = -15f;
    [HideInInspector] public float fireBallDamage = -10f;
    private Hit normalHit;
    private Hit rockPunchHit;

    private AuraManager auraManager;

    private float fireCooldown = 3f;
    private float bubbleCooldown = 5f;
    private float lastSkillTime = -5f;
    private float hitCooldown = 1f;
    private float lastHitTime = -1f;

    private Vector2 bcSize;
    private float initPos;

    public static event Action Bubble;
    public static event Action OpenElements;
    public static event Action CloseElements;
    public static event Action<Hit, bool> Damage;
    public static event Action<string> Restart;

    // TEMPORAL
    //public Text tiempo;
    //public Text lastTime;
    //private void Tempo()
    //{
    //    tiempo.text = Time.time.ToString();
    //    lastTime.text = "CD: " + lastSkillTime.ToString();
    //}
    ////////////////////////////////

    private void Start()
    {
        abilityM = GetComponent<AbilityManager>();
        healthM = GetComponent<HealthManager>();

        CheckCollision.Stamp += StopDashing;
        HealthManager.Die += Die;
        BubblePlatform.Bubble += () => healthM.UpdateLife(1);
        Zephyros.StopMove += StopMovePlayer;

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        playerAnim = GetComponent<Animator>();

        checkCollision = transform.GetChild(2).gameObject.GetComponent<BoxCollider2D>();
        hitController = transform.GetChild(3).gameObject;

        normalHit = new Hit(hitController.transform.position, 0.5f, normalDamage);
        rockPunchHit = new Hit(hitController.transform.position, 0.75f, rockPunchDamage);

        auraManager = GetComponent<AuraManager>();

        bcSize = bc.size;
    }

    private void FixedUpdate()
    {
        if (currentState == State.Diying || currentState == State.Immobile) return;

        float moveInput = Input.GetAxis("Horizontal");

        //Debug.Log(currentState);
        switch (currentState)
        {
            case State.Idle:
                HandleIdle(moveInput);
                break;
            case State.Moving:
                HandleMoving(moveInput);
                break;
            case State.Jumping:
                HandleJumping(moveInput);
                break;
            case State.Dashing:
                HandleDashing();
                break;
        }
        if (Input.GetButton("Fire1") && !radialMenuOpen && lastHitTime <= Time.time - hitCooldown)
        {
            hitController.GetComponent<SpriteRenderer>().enabled = true;
            hitController.GetComponent<SpriteRenderer>().color = Color.white;
            normalHit.SetHitOrigin(hitController.transform.position);
            Damage?.Invoke(normalHit, true);
            lastHitTime = Time.time;
        }
        if (Input.GetButton("Fire2") && !radialMenuOpen)
        {
            switch (currentElement)
            {
                case Element.Fire:
                    if (lastSkillTime <= Time.time - fireCooldown && auraManager.CanUseAura(5f))
                    {
                        auraManager.UpdateAura(-5f);
                        Instantiate(fireBall, gameObject.transform.position, Quaternion.identity);
                        lastSkillTime = Time.time;
                    }
                    break;
                case Element.Water:
                    if (lastSkillTime <= Time.time - bubbleCooldown && auraManager.CanUseAura(5f))
                    {
                        pAD.aggressiveness -= 0.1f;
                        auraManager.UpdateAura(-5f);
                        Bubble?.Invoke();
                        transform.GetChild(1).gameObject.SetActive(true);
                        lastSkillTime = Time.time;
                    }
                    break;
                case Element.Earth:
                    if (lastSkillTime <= Time.time - fireCooldown && auraManager.CanUseAura(5f))
                    {
                        auraManager.UpdateAura(-5f);
                        hitController.GetComponent<SpriteRenderer>().enabled = true;
                        hitController.GetComponent<SpriteRenderer>().color = Color.magenta;
                        rockPunchHit.SetHitOrigin(hitController.transform.position);
                        Damage?.Invoke(rockPunchHit, false);
                        lastSkillTime = Time.time;
                    }
                    break;
                case Element.Air:
                    if (lastSkillTime <= Time.time - fireCooldown && auraManager.CanUseAura(5f))
                    {
                        pAD.aggressiveness -= 0.1f;
                        auraManager.UpdateAura(-5f);
                        checkCollision.enabled = true;
                        initPos = transform.position.x;
                        currentState = State.Dashing;
                        lastSkillTime = Time.time;
                    }
                    break;
                default:
                    Debug.Log("No element selected");
                    break;
            }
        }
    }

    private void Update()
    {
        // TEMPORAL
        //Tempo();
        // ________

        if (Input.GetButtonDown("Jump") && (onGround || doubleJump))
        {
            jumpPressed = true;
        }

        if (Input.GetKey(KeyCode.Tab) && currentState != State.Diying)
        {
            radialMenuOpen = true;
            OpenElements?.Invoke();
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            radialMenuOpen = false;
            CloseElements?.Invoke();
        }

        if (Input.GetAxis("Vertical") < 0f)
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), true);
        }
        else if (Input.GetAxis("Vertical") > 0f)
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), false);
        }
    }

    private void HandleIdle(float moveInput)
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
        playerAnim.SetTrigger("Idle");

        if (moveInput != 0f)
        {
            currentState = State.Moving;
        }

        if (Input.GetButton("Jump") && onGround)
        {
            currentState = State.Jumping;
        }
    }

    private void HandleMoving(float moveInput)
    {
        rb.velocity = new Vector2(moveInput * runSpeed, rb.velocity.y);
        playerAnim.SetTrigger("Moving");

        if (rb.velocity.x < 0f)
        {
            sr.flipX = true;
            checkCollision.transform.Rotate(0, 180, 0);
            hitController.transform.localPosition = new Vector2(-0.5f, hitController.transform.localPosition.y);
        }
        else if (rb.velocity.x > 0f)
        {
            sr.flipX = false;
            checkCollision.transform.Rotate(0, 0, 0);
            hitController.transform.localPosition = new Vector2(0.5f, hitController.transform.localPosition.y);
        }

        if (Input.GetButton("Jump") && (onGround || doubleJump))
        {
            currentState = State.Jumping;
            return;
        }

        if (rb.velocity.x == 0f)
        {
            currentState = State.Idle;
        }
    }

    private void HandleJumping(float moveInput)
    {
        playerAnim.SetTrigger("Moving");

        if (jumpPressed && (onGround || (abilityM.IsAbilityUnlocked(Ability.DoubleJump) && doubleJump)))
        {
            rb.velocity = new Vector2(rb.velocity.x, doubleJump? jumpSpeed * 1.5f : jumpSpeed);
            doubleJump = abilityM.IsAbilityUnlocked(Ability.DoubleJump) && !doubleJump;
            jumpPressed = false;
        }
        if (onGround && rb.velocity.y <= 0f)
        {
            doubleJump = false;
            currentState = State.Idle;
        }
        if (moveInput != 0f)
            currentState = State.Moving;
    }

    private void HandleDashing()
    {
        playerAnim.SetTrigger("Moving");

        bc.size = new Vector2(bcSize.x, 0.2f);
        rb.gravityScale = 0;
        rb.velocity = new Vector2(sr.flipX ? -dashSpeed : dashSpeed, 0);
        if (Mathf.Abs(initPos - transform.position.x) >= 5f)
            StopDashing();
    }

    public void ChangeElement(string typeElement)
    {
        if (Enum.TryParse(typeElement, true, out Element newElement))
        {
            currentElement = newElement;
            //Debug.Log("Element changed to: " + currentElement);
        }
    }

    private void StopDashing()
    {
        checkCollision.enabled = false;
        bc.size = bcSize;
        rb.gravityScale = 1;
        rb.velocity = new Vector2(0, 0);
        currentState = State.Idle;
    }

    private void StopMovePlayer()
    {
        rb.velocity = Vector2.zero;
        currentState = (currentState != State.Immobile) ? State.Immobile : State.Idle;
    }

    private void Die(bool dead)
    {
        if (dead && currentState == State.Diying) return;

        CloseElements?.Invoke();
        if (currentState == State.Dashing)
        {
            StopDashing();
        }
        if (dead)
        {
            currentState = State.Diying;
            StartCoroutine(HandleDie());
        } else
        {
            playerAnim.SetTrigger("Hit");
        }
    }

    private IEnumerator HandleDie()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
        playerAnim.SetTrigger("Die");

        yield return new WaitUntil(() => playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Die"));

        yield return new WaitUntil(() => !playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Die"));

        Restart?.Invoke("DIED");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            healthM.UpdateLife(-1);
        }
    }

    private void OnDestroy()
    {
        CheckCollision.Stamp -= StopDashing;
        HealthManager.Die -= Die;
        BubblePlatform.Bubble -= () => healthM.UpdateLife(1);
        Zephyros.StopMove -= StopMovePlayer;
    }
}
