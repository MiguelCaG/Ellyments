using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnarionSceneryManagement : MonoBehaviour
{
    private GameObject grid;
    private GameObject lava;
    private GameObject platform;
    private GameObject wallPlatform;

    private GameObject[] volcanicRocks;
    private float maxLeft;
    private float maxRight;
    private float maxBottom;
    private float maxTop;

    private bool finishFlood = false;

    private int direction = 1;

    public static event Action sceneryChanged;
    
    private void Start()
    {
        grid = FindObjectOfType<Grid>().gameObject;

        lava = grid.transform.GetChild(1).gameObject;
        platform = grid.transform.GetChild(2).gameObject;
        wallPlatform = grid.transform.GetChild(3).gameObject;

        Ignarion.Flood += Flood;
    }

    private void Update()
    {
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
        while (lava.transform.position.y < 4.2)
        {
            lava.transform.position += new Vector3(0f, 0.02f, 0f);
            platform.transform.position += new Vector3(0f, 0.02f, 0f);
            wallPlatform.transform.position += new Vector3(0f, -0.03f, 0f);
            yield return null;
        }

        lava.transform.position = new Vector3(0f, 4.2f, 0f);
        platform.transform.position = new Vector3(0f, 4.6f, 0f);
        wallPlatform.transform.position = new Vector3(0f, -6.4f, 0f);

        finishFlood = true;
        sceneryChanged?.Invoke();
    }

    private void OnDestroy()
    {
        Ignarion.Flood -= Flood;
    }
}
