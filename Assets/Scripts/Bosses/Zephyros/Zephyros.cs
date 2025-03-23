using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zephyros : Boss
{
    // BOSS PROPERTIES
    private bool hitsUpdated = false;

    // FIGHT ELEMENTS
    //// TORNADO
    private float maxSpinTime = 6f;
    private float spinTime = -1f;
    private int spinDirection = 0;
    private Vector3 spinStartPos = Vector3.zero;

    //// GALE
    //private float galeDirection = 0f;
    //private bool push;
    private float galeXOffset = 11f;
    private Vector2 galeEffectArea = new Vector2(20f, 4f);
    private float galeForce = 10f;

    ////// GALE FORCE
    private GameObject galeForcePS;
    private int flyingObjects = 0;
    private int flyingObjectsEnded = 0;
    private int totalFlyingObjects = 5;
    private float generatingTime = -1f;
    protected float generateTime = 1.5f;
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private GameObject woodPrefab;

    ////// DEVOURING GALE
    private GameObject devouringGalePS;
    private float devouringTime = -1f;
    protected float devourTime = 5f;
    private bool devoured = false;
    private float throwingTime = -1f;

    //// AERIAL RUSH
    private int maxRushes = 3;
    private int rushes = -1;
    private float rushDirection = 0f;
    private Vector3 destination = Vector3.zero;
    private bool attackDodged = false;
    private int nextWay = 0;
    [SerializeField] private Transform[] waypoints;
    private int indexWaypoint = 0;
    private object dodgeLock = new object();

    //// CHANGE PHASE
    public static event Action Swap;

    // PLAYER PROPERTIES
    [SerializeField] PlayerActionsData pAD;

    // SCENE PROPERTIES
    private Vector3 leftFixedPos = new Vector3(-9.9f, -0.8f, 0f);
    private Vector3 rightFixedPos = new Vector3(9.9f, -0.8f, 0f);

    //AUDIOS
    [SerializeField] private AudioClip dash;
    [SerializeField] private AudioClip wind;
    private AudioSource windAudio;
    private float audioTime = 0f;

    private new void Start()
    {
        restTime = 4f;
        //maxLife = 20f;
        maxLife = 200f;
        changePhasePercentage = 0.5f;
        base.Start();
        //life = maxLife * changePhasePercentage + 5f;
        galeForcePS = transform.GetChild(0).gameObject;
        FlyingObject.LimitReached += () => flyingObjectsEnded++;

        devouringGalePS = transform.GetChild(1).gameObject;

        FlyingObject.PlayerHit += UpdatePlayerHits;

        StartCoroutine(HandleSpawn());
    }

    // SPAWN
    private IEnumerator HandleSpawn()
    {
        rb.velocity = new Vector2(0f, 0f);
        rb.gravityScale = 0f;
        transform.position = rightFixedPos;
        Debug.LogWarning("ZephyrosPos: " + transform.position);

        yield return new WaitForSeconds(1.5f);

        rb.gravityScale = 1f;
        alive = true;
    }

    private void Update()
    {
        restTime = Mathf.Clamp(4f - pAD.aggressiveness, 1f, 6f);
    }

    // TORNADO ATTACK
    public bool TornadoAttack()
    {
        bossAnim.SetBool("Tornado", true);
        if (audioTime <= Time.time)
        {
            SoundFXManager.instance.PlaySoundFXClip(dash, transform, 1f);
            audioTime = Time.time + dash.length;
        }
        if (spinTime < 0f)
        {
            spinTime = Time.time;
            spinDirection = lookAtPlayer;
            spinStartPos = transform.position;
            speed = !secondPhase ? 5f : 7f;
        }

        if (Time.time - spinTime >= maxSpinTime || changingPhase)
        {
            bossAnim.SetBool("Tornado", false);

            spinTime = -1f;
            spinDirection = 0;
            spinStartPos = Vector3.zero;
            speed = 1f;
            if (!changingPhase) UpdatePlayerHits(-0.01f);
            audioTime = 0f;
            return false;
        }

        if (rightFixedPos.x <= transform.position.x || spinStartPos.x + 10f <= transform.position.x) { spinDirection = -1; spinStartPos = transform.position; }
        else if (leftFixedPos.x >= transform.position.x || spinStartPos.x - 10f >= transform.position.x) { spinDirection = 1; spinStartPos = transform.position; }

        rb.velocity = new Vector2(speed * spinDirection, rb.velocity.y);

        return true;
    }

    // GALE FORCE ATTACK
    public bool Onrush()
    {
        if (audioTime == 0f)
        {
            SoundFXManager.instance.PlaySoundFXClip(dash, transform, 1f);
            audioTime = Time.time + dash.length;
        }

        int direction = (Vector2.Distance(transform.position, leftFixedPos) < Vector2.Distance(transform.position, rightFixedPos)) ? -1 : 1;
        speed = !secondPhase ? 6f : 8f;

        if (changingPhase)
        {
            if (!bossAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) bossAnim.SetTrigger("Idle");
            rb.velocity = new Vector2(0f, rb.velocity.y);
            return false;
        }

        if (transform.position.x <= leftFixedPos.x || transform.position.x >= rightFixedPos.x)
        {
            if (!bossAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) bossAnim.SetTrigger("Idle");
            rb.velocity = new Vector2(0f, rb.velocity.y);
            transform.position = direction == -1 ? leftFixedPos : rightFixedPos;
            transform.localScale = new Vector2(-direction, transform.localScale.y);
            audioTime = 0f;
            return false;
        }
        else
        {
            if (!bossAnim.GetCurrentAnimatorStateInfo(0).IsName("Move")) bossAnim.SetTrigger("Move");
            transform.localScale = new Vector2(direction, transform.localScale.y);
            rb.velocity = new Vector2(speed * direction, rb.velocity.y);
        }
        return true;
    }

    public bool GaleForceAttack()
    {
        if (audioTime <= Time.time)
        {
            windAudio = SoundFXManager.instance.PlaySoundFXClip(wind, transform, 0.75f);
            audioTime = Time.time + wind.length;
        }

        float pushDirection = transform.localScale.x;
        galeForcePS.SetActive(true);
        Collider2D[] galeCollider = Physics2D.OverlapBoxAll(transform.position + new Vector3(galeXOffset * transform.localScale.x, 0f, 0f), galeEffectArea, 0f);
        foreach (var hit in galeCollider)
        {
            if (hit.CompareTag("Player") && hit.gameObject.layer == 8)
            {
                Transform playerTransform = hit.transform;
                Vector3 targetPosition = playerTransform.position + new Vector3(pushDirection * galeForce * Time.deltaTime, 0f, 0f);
                playerTransform.position = Vector3.Lerp(playerTransform.position, targetPosition, 0.1f);
                //player.GetComponent<Rigidbody2D>().AddForce(new Vector2(galeDirection * galeForce, 0f));
            }
        }

        if (flyingObjectsEnded == totalFlyingObjects || changingPhase)
        {
            galeForcePS.SetActive(false);
            flyingObjectsEnded = 0;
            flyingObjects = 0;
            if (!changingPhase) UpdatePlayerHits(-0.01f);
            if (windAudio != null) Destroy(windAudio.gameObject);
            audioTime = 0f;
            return false;
        }
        else if (flyingObjects < totalFlyingObjects && Time.time - generatingTime >= generateTime)
        {
            generatingTime = Time.time;
            Vector3 origin = new Vector3(transform.position.x, UnityEngine.Random.Range(-2f, 0f), 0f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));

            GameObject flyingObject = Instantiate(UnityEngine.Random.value < 0.5f ? rockPrefab : woodPrefab, origin, rotation);
            FlyingObject fo = flyingObject.GetComponent<FlyingObject>();
            fo.Initialize(pushDirection, 13f * pushDirection);
            flyingObjects++;

            SoundFXManager.instance.PlaySoundFXClip(dash, transform, 1f);
        }

        return true;
    }

    // DEVOURING GALE ATTACK
    public int DevouringGaleAttack()
    {
        if (audioTime <= Time.time)
        {
            windAudio = SoundFXManager.instance.PlaySoundFXClip(wind, transform, 0.75f);
            audioTime = Time.time + wind.length;
        }

        float pullDirection = -transform.localScale.x;
        devouringGalePS.SetActive(true);
        Collider2D[] galeCollider = Physics2D.OverlapBoxAll(transform.position + new Vector3(galeXOffset * transform.localScale.x, 0f, 0f), galeEffectArea, 0f);
        foreach (var hit in galeCollider)
        {
            if (hit.CompareTag("Player") && hit.gameObject.layer == 8)
            {
                Transform playerTransform = hit.transform;
                Vector3 targetPosition = playerTransform.position + new Vector3(pullDirection * galeForce * 2f * Time.deltaTime, 0f, 0f);
                playerTransform.position = Vector3.Lerp(playerTransform.position, targetPosition, 0.1f);
                //player.GetComponent<Rigidbody2D>().AddForce(new Vector2(galeDirection * galeForce, 0f));
            }
        }

        if (devoured)
        {
            devouringGalePS.SetActive(false);
            devouringTime = -1f;
            if (windAudio != null) Destroy(windAudio.gameObject);
            audioTime = 0f;
            return 2;
        }
        if (devouringTime < 0f)
        {
            devouringTime = Time.time;
        }
        if (Time.time - devouringTime >= devourTime || changingPhase)
        {
            devouringGalePS.SetActive(false);
            devouringTime = -1f;
            if (!changingPhase) UpdatePlayerHits(-0.01f);
            if (windAudio != null) Destroy(windAudio.gameObject);
            audioTime = 0f;
            return 0;
        }

        return 1;
    }

    public bool ThrowPlayer()
    {
        if (devoured)
        {
            EventManager.InvokeStopMove();

            bossAnim.SetTrigger("Spit");

            Vector2 throwVelocity = new Vector2(transform.localScale.x * 10f, 10f);
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

            playerRb.velocity = throwVelocity;

            throwingTime = Time.time;

            SoundFXManager.instance.PlaySoundFXClip(dash, transform, 1f);

            devoured = false;
        }

        if (Time.time - throwingTime >= 2f)
        {
            EventManager.InvokeStopMove();
            return false;
        }

        return true;
    }

    // AERIAL RUSH ATTACK
    public bool AerialRushAttack()
    {
        if (Mathf.Abs(transform.position.x - player.transform.position.x) <= 0.1f && !attackDodged)
        {
            attackDodged = true;
            DodgeChecker();
        }
        speed = !secondPhase ? 8f : 10f;
        if (rushes == -1)
        {
            rushDirection = lookAtPlayer;
            destination = rushDirection == -1 ? leftFixedPos : rightFixedPos;
            rushes = 0;
        }
        else if (rushes == maxRushes || changingPhase)
        {
            rushes = -1;
            if (!changingPhase) UpdatePlayerHits(-0.01f);
            return false;
        }

        if (audioTime == 0f)
        {
            SoundFXManager.instance.PlaySoundFXClip(dash, transform, 1f);
            audioTime = Time.time + dash.length;
        }

        if (rushes < maxRushes)
        {
            if (Mathf.Abs(destination.x - transform.position.x) < 0.1f)
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
                transform.localScale = new Vector2(-rushDirection, transform.localScale.y);
                transform.position = destination;
                rushes++;
                rushDirection = -rushDirection;
                destination = rushDirection == -1 ? leftFixedPos : rightFixedPos;
                indexWaypoint = rushDirection == -1 ? 0 : waypoints.Length - 1;
                nextWay = DecideNextWay();
                attackDodged = false;
                audioTime = 0f;
            }
            else
            {
                switch (nextWay)
                {
                    case 1:
                        transform.localScale = new Vector2(rushDirection, transform.localScale.y);

                        float destinyDistance = Mathf.Abs(transform.position.x - destination.x);

                        Vector2 baseVelocity = new Vector2(speed * rushDirection, 0f);

                        float verticalSpeed = Mathf.Lerp(speed, -speed, Mathf.InverseLerp(16f, 6f, destinyDistance));

                        if (transform.position.y < 2.5f && destinyDistance > 6f)
                            baseVelocity.y = Mathf.Max(speed, verticalSpeed);
                        else if (destinyDistance <= 6f)
                            baseVelocity.y = Mathf.Min(-speed, verticalSpeed);

                        rb.velocity = baseVelocity;
                        break;
                    case 0:
                        transform.localScale = new Vector2(rushDirection, transform.localScale.y);

                        rb.velocity = new Vector2(speed * rushDirection, rb.velocity.y);
                        break;
                    case -1:
                        DownRush();
                        break;
                }
            }
        }
        return true;
    }

    private void DownRush()
    {
        if (indexWaypoint == waypoints.Length || indexWaypoint == -1)
        {
            transform.localScale = new Vector3(rushDirection, 1f);
            rb.velocity = new Vector2(speed * rushDirection, 0f);
            bc.isTrigger = false;
            rb.gravityScale = 1f;
            return;
        }

        rb.gravityScale = 0f;
        bc.isTrigger = true;

        Transform targetWaypoint = waypoints[indexWaypoint];

        if ((indexWaypoint % 2 == 0 && rushDirection == -1) || (indexWaypoint % 2 == 1 && rushDirection == 1))
        {
            rb.velocity = new Vector2(speed * rushDirection, 0f);
            if (Mathf.Abs(transform.position.x - targetWaypoint.position.x) <= 0.1f)
            {
                rb.velocity = Vector2.zero;
                transform.position = targetWaypoint.position;
                indexWaypoint -= (int)rushDirection;
            }
        }
        else
        {
            if (indexWaypoint == 1 || indexWaypoint == 2) transform.localScale = new Vector3(rushDirection * 0.5f, 0.5f);
            if (transform.position.y > -4f && Mathf.Abs(transform.position.x - targetWaypoint.position.x) > 0.1f)
                rb.velocity = new Vector2(0f, -speed);
            else if (Mathf.Abs(transform.position.x - targetWaypoint.position.x) > 0.1f)
                rb.velocity = new Vector2(speed * rushDirection, 0f);
            else
            {
                rb.velocity = new Vector2(0f, speed);
                if (Mathf.Abs(transform.position.y - targetWaypoint.position.y) <= 0.1f)
                {
                    rb.velocity = Vector2.zero;
                    transform.position = targetWaypoint.position;
                    indexWaypoint -= (int)rushDirection;
                }
            }
        }
    }

    private void DodgeChecker()
    {
        Vector3 directions;

        if (player.transform.position.y <= -3f)
            directions = new Vector3(-0.05f, -0.05f, 0.05f); // DOWNWARDS
        else if (player.transform.position.y >= 1.75f)
            directions = new Vector3(0.05f, -0.05f, -0.05f); // UPPERWARDS
        else
            directions = new Vector3(-0.05f, 0.05f, -0.05f); // MIDDLE

        lock (dodgeLock)
        {
            Vector3 dodge = new Vector3(
            Mathf.Round((pAD.dodgeZephyros.x + directions.x) * 100f) / 100f,
            Mathf.Round((pAD.dodgeZephyros.y + directions.y) * 100f) / 100f,
            Mathf.Round((pAD.dodgeZephyros.z + directions.z) * 100f) / 100f
            );

            pAD.dodgeZephyros = new Vector3(
                Mathf.Clamp(dodge.x, 0f, 1f),
                Mathf.Clamp(dodge.y, 0f, 1f),
                Mathf.Clamp(dodge.z, 0f, 1f)
            );

            pAD.SaveState();
        }
    }

    private int DecideNextWay()
    {
        float totalDodgeWeights = pAD.dodgeZephyros.x + pAD.dodgeZephyros.y + pAD.dodgeZephyros.z;
        float way = UnityEngine.Random.value;

        if (way <= pAD.dodgeZephyros.x / totalDodgeWeights)
            return 1;
        else if (way <= ((pAD.dodgeZephyros.x / totalDodgeWeights) + (pAD.dodgeZephyros.y / totalDodgeWeights)))
            return 0;
        else
            return -1;
    }

    private void AerialRushStruck()
    {
        lock (dodgeLock)
        {
            switch (nextWay)
            {
                case 1:
                    pAD.dodgeZephyros.x = Mathf.Clamp(pAD.dodgeZephyros.x + 0.05f, 0f, 1f);
                    break;
                case 0:
                    pAD.dodgeZephyros.y = Mathf.Clamp(pAD.dodgeZephyros.y + 0.05f, 0f, 1f);
                    break;
                case -1:
                    pAD.dodgeZephyros.z = Mathf.Clamp(pAD.dodgeZephyros.z + 0.05f, 0f, 1f);
                    break;
            }

            pAD.SaveState();
        }
    }

    // CHANGE PHASE
    public bool ChangeZephyros()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);

        if (bossAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            sr.color -= new Color(0.25f, 0.25f, 0.25f, 0f) * Time.deltaTime;

            if (sr.color.r <= 0.3f) sr.color = new Color(0.3f, 0.3f, 0.3f);
        }

        return sr.color != new Color(0.3f, 0.3f, 0.3f);
    }

    public void ChangeScenery()
    {
        Swap?.Invoke();
        secondPhase = true;
        changingPhase = false;
    }

    private void UpdatePlayerHits(float playerHit)
    {
        if (!hitsUpdated)
        {
            pAD.hitByZephyros = Mathf.Clamp(Mathf.Round((pAD.hitByZephyros + playerHit) * 100f) / 100f, -1f, 1f);

            pAD.SaveState();

            maxSpinTime = 6f - 2f * pAD.hitByZephyros;
            totalFlyingObjects = Mathf.RoundToInt(5 - 2 * pAD.hitByZephyros);
            devourTime = 5f - pAD.hitByZephyros;
            maxRushes = Mathf.RoundToInt(3 - pAD.hitByZephyros);

            // Debug.Log($"{(playerHit > 0f ? '+' : '-')}: TORNADO {maxSpinTime} // GALE {totalFlyingObjects} // ABSORB {devourTime} // RUSH {maxRushes}");
        }

        hitsUpdated = playerHit > 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!hitsUpdated) UpdatePlayerHits(0.1f);
            if (rushes >= 0) AerialRushStruck();
            if (Time.time - devouringTime < devourTime) devoured = true;
        }
    }

    protected override void Restore()
    {
        if (windAudio != null) Destroy(windAudio.gameObject);
        transform.position = new Vector3(transform.position.x, -0.8f, 0f);
        bc.isTrigger = true;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        sr.color = new Color(1f, 1f, 1f, 1f);
    }

    private void OnDestroy()
    {
        FlyingObject.LimitReached -= () => flyingObjectsEnded++;
    }
}
