using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ignarion : MonoBehaviour
{
    // BOSS PROPERTIES
    private Rigidbody2D rb;
    private float speed = 1f;
    private float attackRange = 2f;
    public float maxLife = 100f;
    public float life;
    private Animator ignarionAnim;

    // FIGHT ELEMENTS
    //// EMERGIN FIRE
    public static event Action Emerge;
    private bool isEmerging = false;
    private bool emerged = false;

    //// VOLCANIC ROCKS
    private GameObject[] volcanicRocks;
    private int rocksFallen = 0;

    // SCENE PROPERTIES
    private float maxLeft = -11f;
    private float maxRight = 11.5f;
    private float maxBottom = -2.6f;
    private float maxTop = 4.5f;

    // PLAYER PROPERTIES
    private GameObject player;
    private int lookAtPlayer;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        life = maxLife;
        ignarionAnim = GetComponent<Animator>();

        volcanicRocks = GameObject.FindGameObjectsWithTag("VolcanicRock");

        player = GameObject.FindGameObjectWithTag("Player");
    }

    // LOOK AT PLAYER
    private void FixedUpdate()
    {
        lookAtPlayer = player.transform.position.x - transform.position.x >= 0 ? 1 : -1;
    }

    public void LookAtPlayer()
    {
        transform.localScale = new Vector2(lookAtPlayer, transform.localScale.y);
    }

    // EMERGIN FIRE ATTACK
    public bool EmerginFire()
    {
        if (emerged)
        {
            emerged = false;
            return false;
        }

        if (!isEmerging && !emerged)
        {
            StartCoroutine(HandleEmerginFire());
        }

        return true;
    }

    private IEnumerator HandleEmerginFire()
    {
        isEmerging = true;

        rb.velocity = new Vector2(0f, rb.velocity.y);
        ignarionAnim.SetTrigger("EmerginFire");

        yield return new WaitUntil(() => ignarionAnim.GetCurrentAnimatorStateInfo(0).IsName("Kick"));

        yield return new WaitUntil(() => !ignarionAnim.GetCurrentAnimatorStateInfo(0).IsName("Kick"));

        Emerge?.Invoke();

        yield return new WaitForSeconds(3f);

        emerged = true;
        isEmerging = false;
    }

    // VOLCANIC RAIN ATTACK
    public bool VolcanicRain()
    {
        if (rocksFallen >= 8 && AllRocksFallen())
        {
            rocksFallen = 0;
            foreach (GameObject rock in volcanicRocks) rock.transform.position = new Vector3(rock.transform.position.x, maxTop + 1.5f);
            return false;
        }
        if (rocksFallen == 0) ignarionAnim.SetTrigger("VolcanicRain");

        rb.velocity = new Vector2(0f, rb.velocity.y);
        foreach (GameObject rock in volcanicRocks)
        {
            if (rocksFallen < 8 && rock.GetComponent<VolcanicRock>().fallen)
            {
                Vector3 rockScale = rock.transform.localScale;
                Vector2 rockPos;
                do
                {
                    rockPos = new Vector2(UnityEngine.Random.Range(maxLeft + (rockScale.x / 2f), maxRight - (rockScale.x / 2f)), maxTop + 1.5f);
                } while (PositionOccupied(rockPos, rockScale.x));
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

    private bool PositionOccupied(Vector2 position, float originalScale)
    {
        foreach (GameObject rock in volcanicRocks)
        {
            if (Vector2.Distance(rock.transform.position, position) <= originalScale)
                return true;
        }
        return false;
    }

    public bool ApproachPlayer()
    {
        transform.localScale = new Vector2(lookAtPlayer, transform.localScale.y);
        rb.velocity = new Vector2(speed * lookAtPlayer, rb.velocity.y);

        return !(attackRange >= Mathf.Abs(player.transform.position.x - transform.position.x));
    }
}