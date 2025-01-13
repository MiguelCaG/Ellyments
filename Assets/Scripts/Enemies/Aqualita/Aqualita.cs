using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aqualita : Enemy
{
    private float lifePercenatage = 0.5f;
    private float healTime = 0f;
    private float healPercentage = 0.25f;

    private GameObject bubble;

    public AqualitaFSM aqualitaFSM;
    private new void Start()
    {
        maxLife = 30f;
        base.Start();
        bubble = transform.GetChild(4).gameObject;
        aqualitaFSM = GetComponent<AqualitaFSM>();
    }

    public new void Patrol()
    {
        if (currentLife <= maxLife * lifePercenatage)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            healTime = Time.time + 4f;
            aqualitaFSM.fsm1State = AqualitaFSM.FSM1State.HEAL;
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
            aqualitaFSM.fsm1State = AqualitaFSM.FSM1State.CHASE;
        }
    }

    public new void Chase()
    {
        if (currentLife <= maxLife * lifePercenatage)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            healTime = Time.time + 4f;
            aqualitaFSM.fsm1State = AqualitaFSM.FSM1State.HEAL;
            return;
        }

        if (!playerInRange)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            iddleTime = Time.time;
            aqualitaFSM.fsm1State = AqualitaFSM.FSM1State.PATROL;
            return;
        }

        base.Chase();

        if (attackRange >= Mathf.Abs(playerPos.x - transform.position.x))
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            prepareAttack = Time.time + 2f;
            restAttack = Time.time;
            aqualitaFSM.fsm1State = AqualitaFSM.FSM1State.ATTACK;
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
                healTime = Time.time + 4f;
                aqualitaFSM.fsm1State = AqualitaFSM.FSM1State.HEAL;
                return;
            }

            if (!playerInRange)
            {
                iddleTime = Time.time;
                aqualitaFSM.fsm1State = AqualitaFSM.FSM1State.PATROL;
                return;
            }

            if (attackRange < Mathf.Abs(playerPos.x - transform.position.x))
            {
                aqualitaFSM.fsm1State = AqualitaFSM.FSM1State.CHASE;
                return;
            }
        }
    }

    public void Heal()
    {
        if (currentLife > maxLife * lifePercenatage && attackRange >= Mathf.Abs(playerPos.x - transform.position.x))
        {
            bubble.SetActive(false);
            prepareAttack = Time.time + 2f;
            restAttack = Time.time;
            aqualitaFSM.fsm1State = AqualitaFSM.FSM1State.ATTACK;
            return;
        }
        else if (currentLife == maxLife)
        {
            bubble.SetActive(false);
            if (!playerInRange)
            {
                iddleTime = Time.time;
                aqualitaFSM.fsm1State = AqualitaFSM.FSM1State.PATROL;
            }
            else
            {
                aqualitaFSM.fsm1State = AqualitaFSM.FSM1State.CHASE;
            }
            return;
        }
        else
        {
            bubble.SetActive(true);
            if (healTime <= Time.time)
            {
                UpdateLife(maxLife * healPercentage);
                healTime = Time.time + 4f;
            }
        }
    }
}
