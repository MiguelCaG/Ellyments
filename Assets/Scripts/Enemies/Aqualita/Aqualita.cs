using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aqualita : Enemy
{
    private float attackRange = 1f;
    private float lifePercenatage = 0.5f;
    private float healTime = 0f;
    private float healPercentage = 0.25f;

    private GameObject bubble;

    public AqualitaFSM aqualitaFSM;
    private new void Start()
    {
        maxLife = 20f;
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

        base.Patrol();

        if (playerInRange)
        {
            aqualitaFSM.fsm1State = AqualitaFSM.FSM1State.CHASE;
        }
    }

    public void Chase() // I THINK THE BASE METHOD SHOULD BE IN ENEMY AS IT IS USED IN TWO MORE TYPES OF ENEMIES.
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

        transform.localScale = new Vector2(lookAtPlayer, transform.localScale.y);
        rb.velocity = new Vector2(speed * 2f * lookAtPlayer, rb.velocity.y);

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

            if (attackRange < Mathf.Abs(playerPos.x - transform.position.x))
            {
                aqualitaFSM.fsm1State = AqualitaFSM.FSM1State.CHASE;
                return;
            }

            if (!playerInRange)
            {
                iddleTime = Time.time;
                aqualitaFSM.fsm1State = AqualitaFSM.FSM1State.PATROL;
                return;
            }
        }
    }

    public void Heal()
    {
        if (currentLife == maxLife)
        {
            bubble.SetActive(false);
            aqualitaFSM.fsm1State = AqualitaFSM.FSM1State.PATROL;
            return;
        }
        else if (currentLife > maxLife * lifePercenatage && attackRange >= Mathf.Abs(playerPos.x - transform.position.x))
        {
            bubble.SetActive(false);
            prepareAttack = Time.time + 2f;
            restAttack = Time.time;
            aqualitaFSM.fsm1State = AqualitaFSM.FSM1State.ATTACK;
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
