using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terranido : Enemy
{
    private bool stopSleeping = false;

    private float expansionRadius = 2.5f;
    private float expansionDamage = -2f;
    private Hit expansionHit;

    public TerranidoFSM terranidoFSM;
    private new void Start()
    {
        speed = 0.5f;
        maxLife = 40f;
        base.Start();
        Damaged += Awakened;
        terranidoFSM = GetComponent<TerranidoFSM>();
        expansionHit = new Hit(transform.position, expansionRadius, expansionDamage);
    }

    public void Sleep()
    {
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

    public void Expand()
    {
        if (transform.localScale.x <= 1.5f)
        {
            transform.localScale += new Vector3(0.01f, 0.01f, 0f);
        }
        else
        {
            // TEMPORAL
            transform.GetChild(4).gameObject.SetActive(true);
            transform.GetChild(4).gameObject.transform.localScale = new Vector3(3f, 3f, 1f);
            /////////////
            expansionHit.SetHitOrigin(transform.position);
            HurtPlayer(expansionHit);
            transform.localScale = new Vector3(1f, 1f, 1f);

            //
            StartCoroutine(RestoreExplosion(transform.GetChild(4)));
            //

            if (playerInRange)
                terranidoFSM.fsm1State = TerranidoFSM.FSM1State.CHASE;
            else
                terranidoFSM.fsm1State = TerranidoFSM.FSM1State.SLEEP;
        }
    }

    private IEnumerator RestoreExplosion(Transform explosion)
    {
        yield return new WaitForSeconds(1f);

        explosion.localScale = new Vector3(1f, 1f, 1f);
        explosion.gameObject.SetActive(false);
    }

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
