using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCanvasManager : MonoBehaviour
{
    public GameObject bossHealtBar;
    private GameObject boss;

    private void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");
        if (boss != null)
        {
            bossHealtBar.SetActive(true);
        }
    }
}
