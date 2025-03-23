using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamarko : Enemy
{
    // MAGMA BALL
    [SerializeField] private GameObject magmaBallPrefab;
    
    // TIMERS
    private float shootCooldown = 0f;
    private float reloadTime = 0f;

    // RELOAD PROPERTIES
    private Vector2 escapeStartPos;

    private int ammo = 4;
    const int maxAmmo = 4;

    private GameObject smoke;

    // AUDIO
    [SerializeField] private AudioClip explosion;

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
            ChangeAnim("EnemyIdle", "Idle");
            return;
        }
        else
        {
            base.Patrol();
        }

        if (playerInRange)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            shootCooldown = Time.time - 1.5f;
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

        if (ammo > 0 && shootCooldown <= Time.time - 1.5f)
        {
            transform.localScale = new Vector2(lookAtPlayer, transform.localScale.y);

            ChangeAnim("EnemyAttack", "Shoot");

            if (shootCooldown <= Time.time - 2f)
            {
                Vector2 magmaBallPos = gameObject.transform.position + new Vector3(0f, 0.5f, 0f);
                GameObject magmaBall = Instantiate(magmaBallPrefab, magmaBallPos, Quaternion.identity);
                MagmaBall mB = magmaBall.GetComponent<MagmaBall>();
                mB.Initialize(gameObject, playerPos + new Vector2(0f, 0.5f));

                shootCooldown = Time.time;
                ammo--;

                SoundFXManager.instance.PlaySoundFXClip(explosion, transform, 1f);
            }
        }
        else if (ammo <= 0)
        {
            escapeStartPos = transform.position;
            changeDirection = false;
            flamarkoFSM.fsm1State = FlamarkoFSM.FSM1State.RELOAD;
            sr.color = new Color(0.3f, 0f, 0f);
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
                shootCooldown = Time.time - 1.5f;
                flamarkoFSM.fsm1State = FlamarkoFSM.FSM1State.SHOOT;
            }
            return;
        }

        if (!changeDirection && (Mathf.Abs(escapeStartPos.x - transform.position.x) <= 3f))
        {
            ChangeAnim("EnemyRun", "Run");

            rb.velocity = new Vector2(-lookAtPlayer * speed * 2f, rb.velocity.y);
            transform.localScale = new Vector2(-lookAtPlayer, transform.localScale.y);
            reloadTime = Time.time + 5f;
        }
        else
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            smoke.SetActive(true);
            sr.color += new Color(0.15f * Time.fixedDeltaTime, 0f, 0f);

            ChangeAnim("EnemyExtra", "Reload");

            if (reloadTime <= Time.time)
            {
                ammo = maxAmmo;
                sr.color = new Color(1f, 0f, 0f);
            }
        }
    }
}
