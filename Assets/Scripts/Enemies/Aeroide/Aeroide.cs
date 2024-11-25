using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aeroide : Enemy
{
    private float lifePercenatage = 0.5f;
    private float escapingTime = 0f;

    public AeroideFSM aeroideFSM;
    private new void Start()
    {
        maxLife = 20f;
        base.Start();
        aeroideFSM = GetComponent<AeroideFSM>();
    }

    public new void Patrol()
    {
        if (currentLife <= maxLife * lifePercenatage)
        {
            aeroideFSM.fsm1State = AeroideFSM.FSM1State.ESCAPE;
            return;
        }

        if (iddleTime >= Time.time - 3f)
        {
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
            prepareAttack = Time.time + 2f;
            restAttack = Time.time;
            aeroideFSM.fsm1State = AeroideFSM.FSM1State.ATTACK;
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
            if (currentLife <= maxLife * lifePercenatage)
            {
                escapingTime = Time.time;
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

    public void Escape()
    {
        if (escapingTime <= Time.time - 1f && attackRange >= Mathf.Abs(playerPos.x - transform.position.x))
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            prepareAttack = Time.time + 2f;
            restAttack = Time.time;
            aeroideFSM.fsm1State = AeroideFSM.FSM1State.ATTACK;
            return;
        }
        else
        {
            transform.localScale = new Vector2(-lookAtPlayer, transform.localScale.y);
            rb.velocity = new Vector2(speed * 2f * -lookAtPlayer, rb.velocity.y);
        }
    }
}
