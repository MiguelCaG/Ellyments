using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aeroide : Enemy
{
    // LIFE PERCENTAGE
    private float lifePercenatage = 0.5f;

    // ESCAPING TIMER
    private float escapingTime = 0f;

    // FSM
    public AeroideFSM aeroideFSM;

    private new void Start()
    {
        maxLife = 30f;

        elemStrength = PlayerBehaviour.Element.Earth;
        elemWeakness = PlayerBehaviour.Element.Fire;

        base.Start();

        aeroideFSM = GetComponent<AeroideFSM>();
    }

    // PATROL STATE
    public new void Patrol()
    {
        if (currentLife <= maxLife * lifePercenatage)
        {
            aeroideFSM.fsm1State = AeroideFSM.FSM1State.ESCAPE;
            return;
        }

        if (iddleTime >= Time.time - 3f)
        {
            ChangeAnim("EnemyIdle", "Idle");
            return;
        }
        else
        {
            base.Patrol();
        }

        if (playerInRange)
        {
            aeroideFSM.fsm1State = AeroideFSM.FSM1State.CHASE;
        }
    }

    // CHASE STATE
    public new void Chase()
    {
        if (currentLife <= maxLife * lifePercenatage)
        {
            aeroideFSM.fsm1State = AeroideFSM.FSM1State.ESCAPE;
            return;
        }

        if (!playerInRange)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            iddleTime = Time.time;
            aeroideFSM.fsm1State = AeroideFSM.FSM1State.PATROL;
            return;
        }

        base.Chase();

        if (attackRange >= Mathf.Abs(playerPos.x - transform.position.x))
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            prepareAttack = Time.time + 1f;
            restAttack = Time.time;
            aeroideFSM.fsm1State = AeroideFSM.FSM1State.ATTACK;
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
            if (currentLife <= maxLife * lifePercenatage)
            {
                escapingTime = Time.time;
                restAttack = Time.time + 0.5f;
                aeroideFSM.fsm1State = AeroideFSM.FSM1State.ESCAPE;
                return;
            }

            if (!playerInRange)
            {
                iddleTime = Time.time;
                aeroideFSM.fsm1State = AeroideFSM.FSM1State.PATROL;
                return;
            }

            if (attackRange < Mathf.Abs(playerPos.x - transform.position.x))
            {
                aeroideFSM.fsm1State = AeroideFSM.FSM1State.CHASE;
                return;
            }
        }
    }

    // ESCAPE STATE
    public void Escape()
    {
        if(restAttack > Time.time)
        {
            return;
        }

        if (escapingTime <= Time.time - 1f && attackRange >= Mathf.Abs(playerPos.x - transform.position.x))
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            prepareAttack = Time.time + 1f;
            restAttack = Time.time;
            aeroideFSM.fsm1State = AeroideFSM.FSM1State.ATTACK;
            return;
        }
        else
        {
            ChangeAnim("EnemyRun", "Escape");

            transform.localScale = new Vector2(-lookAtPlayer, transform.localScale.y);
            rb.velocity = new Vector2(speed * 2f * -lookAtPlayer, rb.velocity.y);
        }
    }
}
