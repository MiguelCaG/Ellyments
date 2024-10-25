using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
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
        Iddle,
        Moving,
        Jumping,
        Dashing
    }

    public State currentState = State.Iddle;

    private float runSpeed = 10;
    private float jumpSpeed = 5;
    private float dashSpeed = 50;

    Rigidbody2D rb;
    SpriteRenderer sr;
    BoxCollider2D bc;

    [SerializeField] private GameObject fireBall;
    private GameObject hitController;
    private GameObject checkCollision;

    private float fireCooldown = 3f;
    private float bubbleCooldown = 10f;
    private float lastSkillTime = -10f;

    private Vector2 bcSize = new Vector2(0.35f, 0.98f);
    //public static bool dashing = false;
    private float initPos;

    public static event Action Bubble;
    public static event Action OpenElements;
    public static event Action CloseElements;
    public static event Action RockPunch;

    // TEMPORAL
    public Text tiempo;
    public Text lastTime;
    private void Tempo()
    {
        tiempo.text = Time.time.ToString();
        lastTime.text = "CD: " + lastSkillTime.ToString();
    }
    ////////////////////////////////

    private void Start()
    {
        CheckCollision.Stamp += StopDashing;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        checkCollision = transform.GetChild(2).gameObject;
        hitController = transform.GetChild(3).gameObject;
    }

    private void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Horizontal");
        Debug.Log(currentState);
        switch (currentState)
        {
            case State.Iddle:
                HandleIddle(moveInput);
                break;
            case State.Moving:
                HandleMoving(moveInput);
                break;
            case State.Jumping:
                HandleJumping();
                break;
            case State.Dashing:
                HandleDashing();
                break;
        }
        if (Input.GetButton("Fire1"))
        {
            switch (currentElement)
            {
                case Element.Fire:
                    if (lastSkillTime <= Time.time - fireCooldown)
                    {
                        Instantiate(fireBall, gameObject.transform.position, Quaternion.identity);
                        lastSkillTime = Time.time;
                    }
                    break;
                case Element.Water:
                    if (lastSkillTime <= Time.time - bubbleCooldown)
                    {
                        Bubble?.Invoke();
                        transform.GetChild(1).gameObject.SetActive(true);
                        lastSkillTime = Time.time;
                    }
                    break;
                case Element.Earth:
                    if (lastSkillTime <= Time.time - fireCooldown)
                    {
                        hitController.GetComponent<SpriteRenderer>().enabled = true;
                        RockPunch?.Invoke();
                        lastSkillTime = Time.time;
                    }
                    break;
                case Element.Air:
                    if (lastSkillTime <= Time.time - fireCooldown)
                    {
                        checkCollision.SetActive(true);
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
        Tempo();
        // ________

        if (Input.GetKey(KeyCode.Tab))
        {
            OpenElements?.Invoke();
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            CloseElements?.Invoke();
        }
    }

    private void HandleIddle(float moveInput)
    {
        if (moveInput != 0f)
        {
            currentState = State.Moving;
        }

        if (Input.GetButton("Jump") && CheckGround.onGround)
        {
            currentState = State.Jumping;
        }
    }

    private void HandleMoving(float moveInput)
    {
        rb.velocity = new Vector2(moveInput * runSpeed, rb.velocity.y);

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

        if (Input.GetButton("Jump") && CheckGround.onGround)
        {
            currentState = State.Jumping;
            return;
        }

        if (rb.velocity.x == 0f)
        {
            currentState = State.Iddle;
        }
    }

    private void HandleJumping()
    {
        if (CheckGround.onGround && rb.velocity.y <= 0f)
            currentState = State.Iddle;

        if (CheckGround.onGround)
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
    }

    private void HandleDashing()
    {
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
            Debug.Log("Elemento cambiado a: " + currentElement);
        }
    }

    private void StopDashing()
    {
        checkCollision.SetActive(false);
        bc.size = bcSize;
        rb.gravityScale = 1;
        rb.velocity = new Vector2(0, 0);
        currentState = State.Iddle;
    }
}
