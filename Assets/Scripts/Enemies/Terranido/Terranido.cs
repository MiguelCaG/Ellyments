using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terranido : Enemy
{
    // FLAGS
    private bool stopSleeping = false;
    private bool isExpanding = false;

    // EXPANSION ATTACK
    private float expansionRadius = 2f;
    private float expansionDamage = -2f;
    private Hit expansionHit;

    // ANIMATOR
    private Animator terranidoAnim;

    // SLEEPING COLLIDER
    private GameObject rockModeCollider;

    // FSM
    public TerranidoFSM terranidoFSM;

    private new void Start()
    {
        speed = 0.5f;

        maxLife = 40f;

        elemStrength = PlayerBehaviour.Element.Water;
        elemWeakness = PlayerBehaviour.Element.Air;

        base.Start();

        expansionHit = new Hit(transform.position, expansionRadius, expansionDamage, PlayerBehaviour.Element.Earth);

        terranidoAnim = GetComponent<Animator>();

        rockModeCollider = transform.GetChild(6).gameObject;

        terranidoFSM = GetComponent<TerranidoFSM>();

        Damaged += Awakened;
    }

    // SLEEP STATE
    public void Sleep()
    {
        rockModeCollider.SetActive(true);
        rb.bodyType = RigidbodyType2D.Static;

        if (stopSleeping)
        {
            terranidoFSM.fsm1State = TerranidoFSM.FSM1State.EXPAND;
            stopSleeping = false;
            return;
        }
    }

    private void Awakened()
    {
        if(terranidoFSM.fsm1State == TerranidoFSM.FSM1State.SLEEP)
            stopSleeping = true;
    }

    // EXPAND STATE
    public void Expand()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rockModeCollider.SetActive(false);

        if (isExpanding) return;

        isExpanding = true;
        StartCoroutine(Expanding());
    }

    private IEnumerator Expanding()
    {
        terranidoAnim.SetTrigger("Expand");

        yield return new WaitUntil(() => terranidoAnim.GetCurrentAnimatorStateInfo(0).IsName("ExpandTerranido"));

        yield return new WaitUntil(() => !terranidoAnim.GetCurrentAnimatorStateInfo(0).IsName("ExpandTerranido"));

        expansionHit.SetHitOrigin(transform.position);
        HurtPlayer(expansionHit);

        yield return new WaitUntil(() => terranidoAnim.GetCurrentAnimatorStateInfo(0).IsName("RestoreTerranido"));

        yield return new WaitUntil(() => !terranidoAnim.GetCurrentAnimatorStateInfo(0).IsName("RestoreTerranido"));

        if (playerInRange)
            terranidoFSM.fsm1State = TerranidoFSM.FSM1State.CHASE;
        else
            terranidoFSM.fsm1State = TerranidoFSM.FSM1State.SLEEP;

        isExpanding = false;
    }

    // CHASE STATE
    public new void Chase()
    {
        if (!playerInRange)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            terranidoFSM.fsm1State = TerranidoFSM.FSM1State.SLEEP;
            return;
        }

        base.Chase();

        if (attackRange >= Mathf.Abs(playerPos.x - transform.position.x))
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            prepareAttack = Time.time + 2f;
            restAttack = Time.time;
            terranidoFSM.fsm1State = TerranidoFSM.FSM1State.ATTACK;
        }
    }

    // ATTACK STATE
    public new void Attack()
    {
        if (restAttack <= Time.time)
        {
            base.Attack();
        }
        else
        {
            Debug.Log("RESTING");

            if (!playerInRange)
            {
                terranidoFSM.fsm1State = TerranidoFSM.FSM1State.SLEEP;
                return;
            }

            if (attackRange < Mathf.Abs(playerPos.x - transform.position.x))
            {
                terranidoFSM.fsm1State = TerranidoFSM.FSM1State.CHASE;
                return;
            }
        }
    }

    private void OnDestroy()
    {
        Damaged -= Awakened;
    }
}
