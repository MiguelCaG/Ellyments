using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamarkos : Enemy
{
    [SerializeField] private GameObject magmaBallPrefab;
    
    private float shootCooldown = -3;
    private float reloadTime = 0;
    private int ammo = 3;
    const int maxAmmo = 3;

    private GameObject smoke;

    public FlamarkosFSM flamarkosFSM;

    private new void Start()
    {
        maxLife = 10f;
        base.Start();
        smoke = transform.GetChild(3).gameObject;
        flamarkosFSM = GetComponent<FlamarkosFSM>();
    }

    public new void Patrol()
    {
        base.Patrol();

        if (playerInRange)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            flamarkosFSM.fsm1State = FlamarkosFSM.FSM1State.SHOOT;
        }
    }

    public void Shoot()
    {
        if (playerInRange)
        {
            if (ammo > 0)
            {
                if (shootCooldown <= Time.time - 3)
                {
                    transform.localScale = new Vector2(lookAtPlayer, transform.localScale.y);
                    GameObject magmaBall = Instantiate(magmaBallPrefab, gameObject.transform.position, Quaternion.identity);
                    MagmaBall mB = magmaBall.GetComponent<MagmaBall>();
                    mB.flamarkos = gameObject;
                    mB.playerPos = playerPos;

                    shootCooldown = Time.time;
                    ammo--;
                }
            }
            else
            {
                flamarkosFSM.fsm1State = FlamarkosFSM.FSM1State.RELOAD;
            }
        }
        else
        {
            flamarkosFSM.fsm1State = FlamarkosFSM.FSM1State.PATROL;
        }
    }

    public void Reload()
    {
        if (ammo != maxAmmo)
        {
            if (!changeDirection)
            {
                rb.velocity = new Vector2(-lookAtPlayer * 2, rb.velocity.y);
                transform.localScale = new Vector2(-lookAtPlayer, transform.localScale.y);
                reloadTime = Time.time + 5;
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                smoke.SetActive(true);
                if (reloadTime <= Time.time)
                    ammo = maxAmmo;
            }
        }
        else
        {
            smoke.SetActive(false);
            flamarkosFSM.fsm1State = FlamarkosFSM.FSM1State.SHOOT;
        }
    }
}
