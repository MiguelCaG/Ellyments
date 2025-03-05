using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamarko : Enemy
{
    // MAGMA BALL
    [SerializeField] private GameObject magmaBallPrefab;
    
    // TIMERS
    private float shootCooldown = -2f;
    private float reloadTime = 0f;

    // RELOAD PROPERTIES
    private Vector2 escapeStartPos;

    private int ammo = 4;
    const int maxAmmo = 4;

    private GameObject smoke;

    // FSM
    public FlamarkoFSM flamarkoFSM;

    private new void Start()
    {
        maxLife = 20f;

        elemStrength = PlayerBehaviour.Element.Air;
        elemWeakness = PlayerBehaviour.Element.Water;

        base.Start();

        smoke = transform.GetChild(5).gameObject;

        flamarkoFSM = GetComponent<FlamarkoFSM>();
    }

    // PATROL STATE
    public new void Patrol()
    {
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
            rb.velocity = new Vector2(0f, rb.velocity.y);
            flamarkoFSM.fsm1State = FlamarkoFSM.FSM1State.SHOOT;
        }
    }

    // SHOOT STATE
    public void Shoot()
    {
        if (!playerInRange)
        {
            iddleTime = Time.time;
            flamarkoFSM.fsm1State = FlamarkoFSM.FSM1State.PATROL;
            return;
        }

        if (ammo > 0 && shootCooldown <= Time.time - 2f)
        {
            transform.localScale = new Vector2(lookAtPlayer, transform.localScale.y);
            GameObject magmaBall = Instantiate(magmaBallPrefab, gameObject.transform.position, Quaternion.identity);
            MagmaBall mB = magmaBall.GetComponent<MagmaBall>();
            mB.Initialize(gameObject, playerPos);

            shootCooldown = Time.time;
            ammo--;
        }
        else if (ammo <= 0)
        {
            escapeStartPos = transform.position;
            changeDirection = false;
            flamarkoFSM.fsm1State = FlamarkoFSM.FSM1State.RELOAD;
        }
    }

    // RELOAD STATE
    public void Reload()
    {
        if (ammo == maxAmmo)
        {
            smoke.SetActive(false);
            if (!playerInRange)
            {
                iddleTime = Time.time;
                flamarkoFSM.fsm1State = FlamarkoFSM.FSM1State.PATROL;
            }
            else
            {
                flamarkoFSM.fsm1State = FlamarkoFSM.FSM1State.SHOOT;
            }
            return;
        }

        if (!changeDirection && (Mathf.Abs(escapeStartPos.x - transform.position.x) <= 3f))
        {
            rb.velocity = new Vector2(-lookAtPlayer * 2f, rb.velocity.y);
            transform.localScale = new Vector2(-lookAtPlayer, transform.localScale.y);
            reloadTime = Time.time + 5f;
        }
        else
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            smoke.SetActive(true);
            if (reloadTime <= Time.time)
                ammo = maxAmmo;
            }
    }
}
