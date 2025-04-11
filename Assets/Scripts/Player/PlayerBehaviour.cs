using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AbilityManager;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] AbilityManager aM;
    [SerializeField] PlayerActionsData pAD;
    [SerializeField] PlayerData pD;

    public enum Element
    {
        Fire,
        Water,
        Earth,
        Air,
        None
    }

    public enum State
    {
        Idle,
        Moving,
        Jumping,
        Dashing,
        Swimming,
        Diying,
        Immobile
    }

    public State currentState = State.Idle;

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
    private BoxCollider2D playerCollider;

    private float normalDamage = -5f;
    private float rockPunchDamage = -15f;
    [HideInInspector] public float fireBallDamage = -10f;
    private Hit normalHit;
    private Hit rockPunchHit;

    private AuraManager auraManager;

    private float fireCooldown = 3f;
    private float bubbleCooldown = 5f;
    private float lastSkillTime = -5f;
    private float hitCooldown = 0.5f;
    private float lastHitTime = -1f;

    private Vector2 bcSize;
    private float initPos;

    public static event Action Bubble;
    public static event Action OpenElements;
    public static event Action CloseElements;
    public static event Action<Element> ElemChanged;
    public static event Action<Hit, bool> Damage;
    public static event Action<string> Restart;

    public static event Action<PolygonCollider2D> OnPlayerRespawnOrTravel;
    private PolygonCollider2D currentConfiner;

    [SerializeField] private AudioClip hit;
    [SerializeField] private AudioClip explosion;
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip dash;

    private void Start()
    {
        if (pD.lastCheckpoint.respawn)
        {
            transform.position = pD.lastCheckpoint.position;
            pD.lastCheckpoint.respawn = false;

            Collider2D confinerCollider = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("CameraBounds"));
            if (confinerCollider != null)
            {
                currentConfiner = confinerCollider.GetComponent<PolygonCollider2D>();
                OnPlayerRespawnOrTravel?.Invoke(currentConfiner);
            }
        }
        else if (pD.newTravelZone.travel)
        {
            transform.position = pD.newTravelZone.newPosition;
            transform.localScale = new Vector2(pD.newTravelZone.newDirection, transform.localScale.y);
            pD.newTravelZone.travel = false;

            Collider2D confinerCollider = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("CameraBounds"));
            if (confinerCollider != null)
            {
                currentConfiner = confinerCollider.GetComponent<PolygonCollider2D>();
                OnPlayerRespawnOrTravel?.Invoke(currentConfiner);
            }
        }

        healthM = GetComponent<HealthManager>();

        CheckCollision.Stamp += StopDashing;
        HealthManager.Die += Die;
        BubbleShield.Bubble += BubbleFinished;
        EventManager.StopMove += StopMovePlayer;

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        playerAnim = GetComponent<Animator>();

        checkCollision = transform.GetChild(2).gameObject.GetComponent<BoxCollider2D>();
        hitController = transform.GetChild(3).gameObject;
        playerCollider = transform.GetChild(4).gameObject.GetComponent<BoxCollider2D>();

        normalHit = new Hit(hitController.transform.position, 0.5f, normalDamage, Element.None);
        rockPunchHit = new Hit(hitController.transform.position, 0.75f, rockPunchDamage, Element.Earth);

        auraManager = GetComponent<AuraManager>();

        bcSize = bc.size;

        ElemChanged?.Invoke(pD.currentElement);
    }

    private void FixedUpdate()
    {
        if (currentState == State.Diying)
        {
            rb.velocity = new Vector2(0f, onGround ? 0f : rb.velocity.y);
            return;
        }

        if (currentState == State.Immobile)
        {
            playerAnim.SetTrigger("Idle");
            return;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //Debug.Log(currentState);
        switch (currentState)
        {
            case State.Idle:
                HandleIdle(horizontalInput);
                break;
            case State.Moving:
                HandleMoving(horizontalInput);
                break;
            case State.Jumping:
                HandleJumping(horizontalInput);
                break;
            case State.Dashing:
                HandleDashing();
                break;
            case State.Swimming:
                HandleSwimming(horizontalInput, verticalInput);
                break;
        }
        if (Input.GetButton("Fire1") && !radialMenuOpen && lastHitTime <= Time.time - hitCooldown)
        {
            hitController.GetComponent<SpriteRenderer>().color = Color.white;
            hitController.transform.localScale = Vector3.one;
            playerAnim.SetTrigger("Attacking");

            normalHit.SetHitOrigin(hitController.transform.position);
            Damage?.Invoke(normalHit, true);
            lastHitTime = Time.time;

            SoundFXManager.instance.PlaySoundFXClip(hit, transform, 1f);
        }
        if (Input.GetButton("Fire2") && !radialMenuOpen)
        {
            switch (pD.currentElement)
            {
                case Element.Fire:
                    if (lastSkillTime <= Time.time - fireCooldown && auraManager.CanUseAura(5f))
                    {
                        auraManager.UpdateAura(-5f);
                        GameObject fireBallAttack = Instantiate(fireBall, gameObject.transform.position, Quaternion.identity);
                        FireBall fB = fireBallAttack.GetComponent<FireBall>();
                        fB.Initialize(gameObject);
                        lastSkillTime = Time.time;

                        SoundFXManager.instance.PlaySoundFXClip(explosion, transform, 1f);
                    }
                    break;
                case Element.Water:
                    if (lastSkillTime <= Time.time - bubbleCooldown && auraManager.CanUseAura(5f))
                    {
                        pAD.aggressiveness = Mathf.Round((pAD.aggressiveness - 0.1f) * 10f) / 10f;

                        pAD.SaveState();

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

                        hitController.GetComponent<SpriteRenderer>().color = new Color32(255, 75, 0, 255);
                        hitController.transform.localScale = Vector3.one * 2f;
                        playerAnim.SetTrigger("Attacking");

                        rockPunchHit.SetHitOrigin(hitController.transform.position);
                        Damage?.Invoke(rockPunchHit, false);
                        lastSkillTime = Time.time;

                        SoundFXManager.instance.PlaySoundFXClip(explosion, transform, 1f);
                    }
                    break;
                case Element.Air:
                    if (lastSkillTime <= Time.time - fireCooldown && auraManager.CanUseAura(5f))
                    {
                        pAD.aggressiveness = Mathf.Round((pAD.aggressiveness - 0.1f) * 10f) / 10f;

                        pAD.SaveState();

                        auraManager.UpdateAura(-5f);
                        checkCollision.enabled = true;
                        initPos = transform.position.x;
                        currentState = State.Dashing;
                        lastSkillTime = Time.time;

                        SoundFXManager.instance.PlaySoundFXClip(dash, transform, 1f);
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
        if (Input.GetButtonDown("Jump") && (onGround || doubleJump))
        {
            jumpPressed = true;
        }

        if (Input.GetKey(KeyCode.Tab) && currentState != State.Diying && !SceneController.stopTime)
        {
            radialMenuOpen = true;
            OpenElements?.Invoke();
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            radialMenuOpen = false;
            CloseElements?.Invoke();
        }

        if (currentState != State.Diying && currentState != State.Immobile && Input.GetAxis("Vertical") < 0f)
        {

            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), false);
        }
    }

    private void HandleIdle(float moveInput)
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
        if (onGround) { if (!playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) playerAnim.SetTrigger("Idle"); }
        else playerAnim.SetTrigger("Falling");

        if (onGround)
        {
            doubleJump = false;
        }

        if (moveInput != 0f)
        {
            currentState = State.Moving;
        }

        if (Input.GetButton("Jump") && (onGround || doubleJump))
        {
            currentState = State.Jumping;
        }
    }

    private void HandleMoving(float moveInput)
    {
        rb.velocity = new Vector2(moveInput * runSpeed, rb.velocity.y);

        if (onGround) playerAnim.SetTrigger("Moving");
        else if (!playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Jumping")) playerAnim.SetTrigger("Falling");

        transform.localScale = new Vector2(rb.velocity.x < 0f ? -1f : rb.velocity.x > 0f ? 1f : rb.transform.localScale.x, rb.transform.localScale.y);

        if (Input.GetButton("Jump") && (onGround || doubleJump))
        {
            currentState = State.Jumping;
            return;
        }
        if (onGround && rb.velocity.y <= 0f)
            doubleJump = false;

        if (rb.velocity.x == 0f)
        {
            currentState = State.Idle;
        }
    }

    private void HandleJumping(float moveInput)
    {
        if (jumpPressed && (onGround || (aM.IsAbilityUnlocked(Ability.DoubleJump) && doubleJump)))
        {
            playerAnim.SetTrigger("Jumping");

            rb.velocity = new Vector2(rb.velocity.x, doubleJump ? jumpSpeed * 1.5f : jumpSpeed);
            doubleJump = aM.IsAbilityUnlocked(Ability.DoubleJump) && !doubleJump;
            jumpPressed = false;

            SoundFXManager.instance.PlaySoundFXClip(jump, transform, 1f);
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
        playerCollider.size = new Vector2(bcSize.x, 0.2f);
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        if (Mathf.Abs(initPos - transform.position.x) >= 5f)
            StopDashing();
    }

    private void HandleSwimming(float xInput, float yInput)
    {
        playerAnim.SetTrigger("Moving");

        rb.velocity = new Vector2(xInput * runSpeed, yInput * runSpeed);

        transform.localScale = new Vector2(rb.velocity.x < 0f ? -1f : rb.velocity.x > 0f ? 1f : rb.transform.localScale.x, rb.transform.localScale.y);
    }

    public void ChangeElement(string typeElement)
    {
        Element previousElement = pD.currentElement;
        if (Enum.TryParse(typeElement, true, out Element newElement))
        {
            pD.currentElement = previousElement != newElement ? newElement : Element.None;
            ElemChanged?.Invoke(pD.currentElement);

            pD.SaveState();

            //Debug.Log("Element changed to: " + currentElement);
        }
    }

    private void BubbleFinished()
    {
        playerAnim.SetTrigger("Heal");

        healthM.UpdateLife(1);
        StopSwimming();
    }

    private void StopSwimming()
    {
        rb.gravityScale = 1f;
        if (currentState != State.Immobile && currentState != State.Diying) currentState = State.Moving;
    }

    private void StopDashing()
    {
        while (IsStuck())
        {
            transform.position += new Vector3(transform.localScale.x * 0.1f, 0, 0);
        }

        checkCollision.enabled = false;
        bc.size = bcSize;
        playerCollider.size = bcSize;
        rb.gravityScale = 1;
        rb.velocity = new Vector2(0, 0);
        if (currentState != State.Immobile && currentState != State.Diying) currentState = State.Idle;
    }

    private bool IsStuck()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(bcSize.x, 0.5f), 0f, Vector2.zero, 0f, LayerMask.GetMask("DashingHole"));

        return hit.collider != null;
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
            EventManager.InvokePlayerDead();
            currentState = State.Diying;
            StartCoroutine(HandleDie());
        }
        else
        {
            playerAnim.SetTrigger("Hit");
        }
    }

    private IEnumerator HandleDie()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
        //rb.bodyType = RigidbodyType2D.Static;
        playerAnim.SetTrigger("Die");

        yield return new WaitUntil(() => playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Die"));

        yield return new WaitUntil(() => !playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Die"));

        Restart?.Invoke("DIED");
        pD.Reset();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            healthM.UpdateLife(-1);
        }

        if (collision.CompareTag("Water") && currentState != State.Diying && currentState != State.Immobile)
        {
            rb.gravityScale = 5f;
            currentState = State.Swimming;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water") && currentState != State.Diying && currentState != State.Immobile)
        {
            StopSwimming();
        }
    }

    private void OnDestroy()
    {
        CheckCollision.Stamp -= StopDashing;
        HealthManager.Die -= Die;
        BubbleShield.Bubble -= BubbleFinished;
        EventManager.StopMove -= StopMovePlayer;
    }
}
