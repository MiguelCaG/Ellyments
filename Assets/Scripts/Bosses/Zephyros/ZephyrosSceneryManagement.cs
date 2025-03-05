using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class ZephyrosSceneryManagement : MonoBehaviour
{
    [SerializeField] private SceneData sD;

    private GameObject grid;
    private GameObject firstFloor;
    private GameObject oneWayFloor;
    private GameObject hiddenFloor;
    private GameObject bossDoors;

    private Vector3 originalPosition;

    [SerializeField] private GameObject zephyros;
    private GameObject player;

    private bool fightStarted = false;

    private string sceneName;

    public static event Action StopMove;
    public static event Action InitHealthBar;

    private void Start()
    {
        grid = FindObjectOfType<Grid>().gameObject;

        firstFloor = grid.transform.GetChild(1).gameObject;
        oneWayFloor = grid.transform.GetChild(2).gameObject;
        hiddenFloor = grid.transform.GetChild(3).gameObject;
        bossDoors = grid.transform.GetChild(4).gameObject;

        originalPosition = bossDoors.transform.position;

        player = GameObject.FindGameObjectWithTag("Player");

        sceneName = SceneManager.GetActiveScene().name;

        if (sD.IsObjectDestroyed(sceneName, firstFloor.name) && sD.IsObjectDestroyed(sceneName, oneWayFloor.name))
        {
            firstFloor.SetActive(false);
            oneWayFloor.SetActive(true);
        }

        Zephyros.Swap += SwapFloor;
        Boss.BossKilled += () => StartCoroutine(HandleFinishFight());
    }

    private void Update()
    {
        if (player)
        {
            if (player.transform.position.x >= -10.5f && !fightStarted)
            {
                fightStarted = true;

                if (!sD.IsObjectDestroyed(sceneName, zephyros.name))
                    StartCoroutine(HandleCloseDoors());
            }

            hiddenFloor.GetComponent<Tilemap>().color = new Color(1f, 1f, 1f, player.transform.position.y <= -2.5 ? 0.5f : 1f);
        }
    }

    private void SwapFloor()
    {
        firstFloor.SetActive(false);
        oneWayFloor.SetActive(true);
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

        zephyros.SetActive(true);
        InitHealthBar?.Invoke();
        StopMove?.Invoke();
    }

    private IEnumerator HandleFinishFight()
    {
        sD.MarkObjectDestroyed(sceneName, zephyros.name);
        sD.MarkObjectDestroyed(sceneName, firstFloor.name);
        sD.MarkObjectDestroyed(sceneName, oneWayFloor.name);

        yield return new WaitForSeconds(2);

        while (bossDoors.transform.position.y < originalPosition.y)
        {
            bossDoors.transform.position += new Vector3(0f, 1f, 0f) * Time.fixedDeltaTime;
            yield return null;
        }

        bossDoors.transform.position = originalPosition;
    }

    private void OnDestroy()
    {
        Zephyros.Swap -= SwapFloor;
        Boss.BossKilled -= () => StartCoroutine(HandleFinishFight());
    }
}
