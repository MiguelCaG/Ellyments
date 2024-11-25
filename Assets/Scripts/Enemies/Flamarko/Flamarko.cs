using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamarko : Enemy
{
    [SerializeField] private GameObject magmaBallPrefab;
    
    private float shootCooldown = -3f;
    private float reloadTime = 0f;
    private Vector2 escapeStartPos;
    private int ammo = 3;
    const int maxAmmo = 3;

    public float magmaBallDamage = -5f;

    private GameObject smoke;

    public FlamarkoFSM flamarkoFSM;

    private new void Start()
    {
        maxLife = 10f;
        base.Start();
        smoke = transform.GetChild(4).gameObject;
        flamarkoFSM = GetComponent<FlamarkoFSM>();
    }

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

    public void Shoot()
    {
        if (!playerInRange)
        {
            iddleTime = Time.time;
            flamarkoFSM.fsm1State = FlamarkoFSM.FSM1State.PATROL;
            return;
        }

        if (ammo > 0 && shootCooldown <= Time.time - 3f)
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
