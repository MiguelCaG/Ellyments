using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ZephyrosSceneryManagement : MonoBehaviour
{
    private GameObject grid;
    private GameObject firstFloor;
    private GameObject oneWayFloor;
    private GameObject hiddenFloor;
    private GameObject player;

    private void Start()
    {
        grid = FindObjectOfType<Grid>().gameObject;

        firstFloor = grid.transform.GetChild(1).gameObject;
        oneWayFloor = grid.transform.GetChild(2).gameObject;
        hiddenFloor = grid.transform.GetChild(3).gameObject;
        player = GameObject.FindGameObjectWithTag("Player");

        Zephyros.Swap += SwapFloor;
    }

    private void Update()
    {
        if (player)
        {
            hiddenFloor.GetComponent<Tilemap>().color = new Color(1f, 1f, 1f, player.transform.position.y <= -2.5 ? 0.5f : 1f);
        }
    }

    private void SwapFloor()
    {
        firstFloor.SetActive(false);
        oneWayFloor.SetActive(true);
    }

    private void OnDestroy()
    {
        Zephyros.Swap -= SwapFloor;
    }
}
