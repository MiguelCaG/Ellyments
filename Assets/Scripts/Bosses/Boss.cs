using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    // BOSS PROPERTIES
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected BoxCollider2D bc;
    protected Animator bossAnim;

    private float restingTime = -1f;
    protected float restTime = 0f;
    [HideInInspector] public bool alive = false;
    [HideInInspector] public float maxLife = 100f;
    [HideInInspector] public float life;
    protected float speed = 1f;
    private bool destroy = false;

    // CHANGE PHASE
    [HideInInspector] public float changePhasePercentage;
    [HideInInspector] public bool secondPhase = false;
    [HideInInspector] public bool changingPhase = false;

    // PLAYER PROPERTIES
    protected int lookAtPlayer;
    [SerializeField] protected GameObject player;

    public static event Action BossKilled;

    // AUDIOS
    [SerializeField] private AudioClip hurt;

    protected void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        bc = gameObject.GetComponent<BoxCollider2D>();
        bossAnim = GetComponent<Animator>();

        life = maxLife;

        //player = GameObject.FindGameObjectWithTag("Player");

        EventManager.Destroy += () => destroy = true;
    }

    // LOOK AT PLAYER
    protected void FixedUpdate()
    {
        lookAtPlayer = player.transform.position.x - transform.position.x >= 0 ? 1 : -1;
        if (!secondPhase && life <= maxLife * changePhasePercentage) changingPhase = true;
    }

    public void LookAtPlayer()
    {
        transform.localScale = new Vector2(lookAtPlayer, transform.localScale.y);
    }

    // REST
    public bool Rest()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);

        if (restingTime < 0f)
        {
            restingTime = Time.time;
        }

        if (Time.time - restingTime >= restTime || changingPhase)
        {
            if (sr.color.a < 1f)
            {
                sr.color += new Color(0f, 0f, 0f, 1f) * Time.deltaTime;
            }
            else
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
                restingTime = -1f;
                return false;
            }
        }
        else if (sr.color.a > 0.5)
        {
            sr.color -= new Color(0f, 0f, 0f, 1f) * Time.deltaTime;
        }

        return true;
    }

    protected virtual void Restore() {}

    public void UpdateLife(float damage)
    {
        if (changingPhase) return;

        life += damage;
        if (life <= 0f && alive)
        {
            alive = false;
            StartCoroutine(HandleKilled());
        }

        SoundFXManager.instance.PlaySoundFXClip(hurt, transform, 1f);
    }

    private IEnumerator HandleKilled()
    {
        Collider2D playerCollider = player.transform.GetChild(4).GetComponent<Collider2D>();
        Collider2D bossCollider = GetComponent<Collider2D>();
        if (playerCollider != null && bossCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, bossCollider, true);
        }

        Restore();

        bossAnim.SetTrigger("Kill");

        BossKilled?.Invoke();
        player.GetComponent<HealthManager>().AddHeart();
        player.GetComponent<AuraManager>().AddAura();

        yield return new WaitUntil(() => bossAnim.GetCurrentAnimatorStateInfo(0).IsName("Kill"));
        yield return new WaitUntil(() => !bossAnim.GetCurrentAnimatorStateInfo(0).IsName("Kill"));

        yield return new WaitUntil(() => destroy == true);

        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        EventManager.Destroy -= () => destroy = true;
    }
}
