using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IgnarionSceneryManagement : MonoBehaviour
{
    [SerializeField] private SceneData sD;

    private GameObject grid;
    private GameObject lava;
    private GameObject platform;
    private GameObject wallPlatform;
    private GameObject bossDoors;

    private Vector3[] originalPositions = new Vector3[4];

    private GameObject[] volcanicRocks;
    private float maxLeft;
    private float maxRight;
    private float maxBottom;
    private float maxTop;

    private bool finishFlood = false;

    private int direction = 1;

    [SerializeField] private GameObject ignarion;
    private GameObject player;

    private bool fightStarted = false;

    private string sceneName;

    public static event Action StopMove;
    public static event Action InitHealthBar;
    public static event Action SceneryChanged;
    
    private void Start()
    {
        grid = FindObjectOfType<Grid>().gameObject;

        lava = grid.transform.GetChild(1).gameObject;
        platform = grid.transform.GetChild(2).gameObject;
        wallPlatform = grid.transform.GetChild(3).gameObject;
        bossDoors = grid.transform.GetChild(4).gameObject;

        for(int i = 1; i < grid.transform.childCount; i++)
        {
            originalPositions[i - 1] = grid.transform.GetChild(i).transform.position;
        }

        player = GameObject.FindGameObjectWithTag("Player");

        sceneName = SceneManager.GetActiveScene().name;

        Ignarion.Flood += Flood;
        Boss.BossKilled += () => StartCoroutine(HandleFinishFight());
    }

    private void Update()
    {
        if (player.transform.position.x >= -10.5f && !fightStarted)
        {
            fightStarted = true;

            if (!sD.IsObjectDestroyed(sceneName, ignarion.name))
                StartCoroutine(HandleCloseDoors());
        }

        if (finishFlood)
        {
            if (platform.transform.position.y >= 4.6f)
            {
                direction = -1;
            }
            else if (platform.transform.position.y <= 4.4f)
            {
                direction = 1;
            }
            platform.transform.position += new Vector3(0f, 0.001f * direction, 0f);

            foreach (GameObject rock in volcanicRocks)
            {
                if (rock.GetComponent<VolcanicRock>().fallen)
                {
                    Vector3 rockScale = rock.transform.localScale;
                    Vector2 rockPos;
                    do
                    {
                        rockPos = new Vector2(UnityEngine.Random.Range(maxLeft + (rockScale.x / 2f), maxRight - (rockScale.x / 2f)), maxTop + 1.5f);
                    } while (PositionOccupied(rockPos, rockScale.x));
                    rock.transform.position = rockPos;

                    rock.GetComponent<VolcanicRock>().Fall(maxBottom, maxLeft);
                }
            }
        }
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

    private void Flood(GameObject[] volcanicRocks, float[] scenaryLimits)
    {
        this.volcanicRocks = volcanicRocks;
        maxLeft = scenaryLimits[0];
        maxRight = scenaryLimits[1];
        maxBottom = scenaryLimits[2];
        maxTop = scenaryLimits[3];
        StartCoroutine(HandleFlood());
    }

    private IEnumerator HandleFlood()
    {
        while (lava.transform.position.y < 4.2f)
        {
            lava.transform.position += new Vector3(0f, 0.8f, 0f) * Time.fixedDeltaTime;
            platform.transform.position += new Vector3(0f, 0.8f, 0f) * Time.fixedDeltaTime;
            wallPlatform.transform.position += new Vector3(0f, -1.2f, 0f) * Time.fixedDeltaTime;
            yield return null;
        }

        lava.transform.position = new Vector3(0f, 4.2f, 0f);
        platform.transform.position = new Vector3(0f, 4.6f, 0f);
        wallPlatform.transform.position = new Vector3(0f, -6.4f, 0f);

        finishFlood = true;
        SceneryChanged?.Invoke();
    }

    private IEnumerator HandleCloseDoors()
    {
        StopMove?.Invoke();

        while (bossDoors.transform.position.y > -1.91f)
        {
            bossDoors.transform.position -= new Vector3(0f, 1f, 0f) * Time.fixedDeltaTime;
            yield return null;
        }

        bossDoors.transform.position = new Vector3(0f, -1.92f, 0f);

        ignarion.SetActive(true);
        InitHealthBar?.Invoke();
        StopMove?.Invoke();
    }

    private IEnumerator HandleFinishFight()
    {
        finishFlood = false;
        sD.MarkObjectDestroyed(sceneName, ignarion.name);

        while (lava.transform.position.y > originalPositions[0].y)
        {
            lava.transform.position -= new Vector3(0f, 0.4f, 0f) * Time.fixedDeltaTime;
            platform.transform.position -= new Vector3(0f, 0.4f, 0f) * Time.fixedDeltaTime;
            yield return null;
        }

        lava.transform.position = originalPositions[0];
        platform.transform.position = originalPositions[1];

        wallPlatform.GetComponent<CompositeCollider2D>().isTrigger = true;
        while (wallPlatform.transform.position.y < originalPositions[2].y)
        {
            wallPlatform.transform.position += new Vector3(0f, 0.6f, 0f) * Time.fixedDeltaTime;
            yield return null;
        }

        wallPlatform.transform.position = originalPositions[2];

        yield return new WaitForSeconds(2);

        while (bossDoors.transform.position.y < originalPositions[3].y)
        {
            bossDoors.transform.position += new Vector3(0f, 1f, 0f) * Time.fixedDeltaTime;
            yield return null;
        }

        bossDoors.transform.position = originalPositions[3];
    }

    private void OnDestroy()
    {
        Ignarion.Flood -= Flood;
        Boss.BossKilled -= () => StartCoroutine(HandleFinishFight());
    }
}
