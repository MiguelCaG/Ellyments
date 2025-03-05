using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Ignarion : Boss
{
    // BOSS PROPERTIES
    private Animator ignarionAnim;

    private float approachTime = -1f;
    private float attackRange = 4f;

    // FIGHT ELEMENTS
    //// EMERGIN FIRE
    private GameObject[] emerginFires;
    private int emerging = -1; // -1 => Not Emerging, 0 => Emerging, x => X Fires Emerged.

    //// VOLCANIC ROCKS
    private GameObject[] volcanicRocks;
    private int rocksFallen = 0;

    //// FLAME WHIP
    private int whipping = 0; // 0 => Not Whipping, 1 => Whipping, 2 => Whipped.

    //// CHANGE PHASE
    public static event Action<GameObject[], float[]> Flood;
    private bool flooding = false;

    //// HEAT WAVE
    private int waving = 0; // 0 => Not Waving, 1 => Waving, 2 => Waved.

    //// MOLTEN SPIRES
    private int spires = 2;

    //// CHANGE SIDE
    private Vector3 changeTo = Vector3.zero;

    // SCENE PROPERTIES
    private float maxLeft = -11f;
    private float maxRight = 11f;
    private float maxBottom = -2.6f;
    private float maxTop = 4.5f;
    private Vector3 leftFixedPos = new Vector3(-9f, -0.8f, 0f);
    private Vector3 rightFixedPos = new Vector3(9f, -0.8f, 0f);

    private new void Start()
    {
        transform.position = rightFixedPos;
        restTime = 4f;
        maxLife = 200f;
        changePhasePercentage = 0.6f;
        base.Start();
        
        ignarionAnim = GetComponent<Animator>();

        emerginFires = GameObject.FindGameObjectsWithTag("EmerginFire");
        EmerginFire.EmerginCompleted += () => emerging++;

        volcanicRocks = GameObject.FindGameObjectsWithTag("VolcanicRock");

        IgnarionSceneryManagement.SceneryChanged += () => { secondPhase = true; changingPhase = false; };
    }

    // EMERGIN FIRE ATTACK
    public bool EmerginFireAttack()
    {
        if (emerging == emerginFires.Count() || changingPhase)
        {
            emerging = -1;
            return false;
        }

        if (emerging == -1)
        {
            StartCoroutine(HandleEmerginFire());
        }

        return true;
    }

    private IEnumerator HandleEmerginFire(int moltenSpires = 0)
    {
        emerging = 0;

        rb.velocity = new Vector2(0f, rb.velocity.y);
        ignarionAnim.SetTrigger("EmerginFire");

        yield return new WaitUntil(() => ignarionAnim.GetCurrentAnimatorStateInfo(0).IsName("Kick"));

        yield return new WaitUntil(() => !ignarionAnim.GetCurrentAnimatorStateInfo(0).IsName("Kick"));

        foreach (GameObject fire in emerginFires)
        {
            Vector3 fireScale = fire.transform.localScale;
            float target;
            if (moltenSpires > 0)
            {
                if (moltenSpires % 2 == 0)
                {
                    fire.transform.rotation = Quaternion.Euler(0, 0, 90f);
                    fire.transform.position = new Vector2(maxRight, UnityEngine.Random.Range(maxBottom + (fireScale.x / 2f), maxTop - (fireScale.x / 2f)));
                    target = maxLeft - maxRight;
                }
                else
                {
                    fire.transform.rotation = Quaternion.Euler(0, 0, -90f);
                    fire.transform.position = new Vector2(maxLeft, UnityEngine.Random.Range(maxBottom + (fireScale.x / 2f), maxTop - (fireScale.x / 2f)));
                    target = maxRight - maxLeft;
                }
                moltenSpires -= 1;
            }
            else
            {
                Vector2 firePos;

                do
                {
                    firePos = new Vector2(UnityEngine.Random.Range(maxLeft + (fireScale.x / 2f), maxRight - (fireScale.x / 2f)), maxBottom);
                } while (PositionOccupied(firePos, fireScale.x, emerginFires));
                fire.transform.position = firePos;
                target = maxTop - maxBottom;
            }

            fire.GetComponent<EmerginFire>().Emerge(target);
        }
    }

    // VOLCANIC RAIN ATTACK
    public bool VolcanicRainAttack()
    {
        if (rocksFallen >= 12 && AllRocksFallen() || changingPhase)
        {
            rocksFallen = 0;
            return false;
        }
        if (rocksFallen == 0) ignarionAnim.SetTrigger("VolcanicRain");

        rb.velocity = new Vector2(0f, rb.velocity.y);
        foreach (GameObject rock in volcanicRocks)
        {
            if (rocksFallen < 12 && rock.GetComponent<VolcanicRock>().fallen)
            {
                Vector3 rockScale = rock.transform.localScale;
                Vector2 rockPos;
                do
                {
                    rockPos = new Vector2(UnityEngine.Random.Range(maxLeft + (rockScale.x / 2f), maxRight - (rockScale.x / 2f)), maxTop + 1.5f);
                } while (PositionOccupied(rockPos, rockScale.x, volcanicRocks));
                rock.transform.position = rockPos;

                rock.GetComponent<VolcanicRock>().Fall(maxBottom, maxLeft);
                rocksFallen++;
            }
        }

        return true;
    }

    private bool AllRocksFallen()
    {
        foreach (GameObject rock in volcanicRocks)
        {
            if (!rock.GetComponent<VolcanicRock>().fallen) return false;
        }
        return true;
    }

    private bool PositionOccupied(Vector2 position, float originalScale, GameObject[] attackElement)
    {
        foreach (GameObject element in attackElement)
        {
            if (Vector2.Distance(element.transform.position, position) <= originalScale)
                return true;
        }
        return false;
    }

    // FLAME WHIP ATTACK
    public int ApproachPlayer()
    {
        if (approachTime < 0f)
        {
            approachTime = Time.time;
        }

        if (Time.time - approachTime >= 6f || changingPhase)
        {
            approachTime = -1f;
            return 0;
        }

        transform.localScale = new Vector2(lookAtPlayer, transform.localScale.y);
        rb.velocity = new Vector2(speed * lookAtPlayer, rb.velocity.y);

        if (attackRange >= Mathf.Abs(player.transform.position.x - transform.position.x))
        {
            approachTime = -1f;
            return 2;
        }

        return 1;
    }

    public bool FlameWhipAttack()
    {
        if (whipping == 2)
        {
            whipping = 0;
            return false;
        }

        if (whipping == 0)
        {
            StartCoroutine(HandleFlameWhip());
        }

        return true;
    }

    private IEnumerator HandleFlameWhip()
    {
        whipping = 1;

        rb.velocity = new Vector2(0f, rb.velocity.y);
        for (int i = 0; i < 2; i++)
        {
            ignarionAnim.SetTrigger("FlameWhip");

            yield return new WaitUntil(() => ignarionAnim.GetCurrentAnimatorStateInfo(0).IsName("Whip"));
            yield return new WaitUntil(() => !ignarionAnim.GetCurrentAnimatorStateInfo(0).IsName("Whip"));
        }
        whipping = 2;
    }

    // CHANGE PHASE
    public bool ChangeIgnarion()
    {
        int direction = (Vector2.Distance(transform.position, leftFixedPos) < Vector2.Distance(transform.position, rightFixedPos)) ? -1 : 1;
        speed = 3f;

        if (transform.position.x <= leftFixedPos.x || transform.position.x >= rightFixedPos.x)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            transform.position = direction == -1 ? leftFixedPos : rightFixedPos;
            transform.localScale = new Vector2(-direction, transform.localScale.y);

            sr.color -= new Color(0f, 0.02f, 0.02f, 0f);
        }
        else
        {
            transform.localScale = new Vector2(direction, transform.localScale.y);
            rb.velocity = new Vector2(speed * direction, rb.velocity.y);
        }

        return sr.color != Color.red;
    }

    public bool ChangeScenery()
    {
        if (!flooding)
        {
            flooding = true;
            maxBottom = -1.6f;
            float[] sceneryLimits = { maxLeft, maxRight, maxBottom, maxTop };
            Flood?.Invoke(volcanicRocks, sceneryLimits);
        }

        return !secondPhase;
    }

    // HEAT WAVE ATTACK
    public bool HeatWaveAttack()
    {
        if (waving == 2)
        {
            waving = 0;
            return false;
        }

        if (waving == 0)
        {
            StartCoroutine(HandleHeatWave());
        }

        return true;
    }

    private IEnumerator HandleHeatWave()
    {
        waving = 1;

        ignarionAnim.SetTrigger("HeatWave");

        yield return new WaitUntil(() => ignarionAnim.GetCurrentAnimatorStateInfo(0).IsName("Wave"));
        yield return new WaitUntil(() => !ignarionAnim.GetCurrentAnimatorStateInfo(0).IsName("Wave"));

        waving = 2;
    }

    // MOLTEN SPIRES ATTACK
    public bool MoltenSpiresAttack()
    {
        if (emerging == emerginFires.Count())
        {
            emerging = -1;
            return false;
        }

        if (emerging == -1)
        {
            StartCoroutine(HandleEmerginFire(spires));
        }

        return true;
    }

    // CHANGE SIDE
    public bool ChangeSide()
    {
        if ((transform.position.x == leftFixedPos.x && changeTo == leftFixedPos) || (transform.position.x == rightFixedPos.x && changeTo == rightFixedPos))
        {
            changeTo = Vector3.zero;
            bc.isTrigger = false;
            rb.gravityScale = 1f;
            return false;
        }

        if (transform.position.x == leftFixedPos.x && Math.Abs(transform.position.y - leftFixedPos.y) <= 0.01 && changeTo == Vector3.zero)
        {
            changeTo = rightFixedPos;
            StartCoroutine(HandleChangeSide(rightFixedPos, 1));
        }
        else if (transform.position.x == rightFixedPos.x && Math.Abs(transform.position.y - rightFixedPos.y) <= 0.01 && changeTo == Vector3.zero)
        {
            changeTo = leftFixedPos;
            StartCoroutine(HandleChangeSide(leftFixedPos, -1));
        }

        return true;
    }

    private IEnumerator HandleChangeSide(Vector3 destiny, int direction)
    {
        rb.gravityScale = 0f;
        bc.isTrigger = true;

        while (transform.position.y > -2.5f)
        {
            transform.position += new Vector3(0f, -0.02f, 0f);
            yield return null;
        }

        speed = 5f;
        do
        {
            rb.velocity = new Vector2(speed * direction, rb.velocity.y);
            yield return null;
        }
        while (Math.Abs(transform.position.x - destiny.x) > 0.01f);

        rb.velocity = new Vector2(0f, rb.velocity.y);
        while (transform.position.y < destiny.y)
        {
            transform.position += new Vector3(0f, 0.02f, 0f);
            yield return null;
        }

        transform.position = destiny;
        transform.localScale = new Vector2(-direction, transform.localScale.y);
    }

    private void OnDestroy()
    {
        EmerginFire.EmerginCompleted -= () => emerging++;
        IgnarionSceneryManagement.SceneryChanged -= () => { secondPhase = true; changingPhase = false; };
    }
}