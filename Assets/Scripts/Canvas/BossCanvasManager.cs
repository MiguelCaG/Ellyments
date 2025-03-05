using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCanvasManager : MonoBehaviour
{
    public GameObject bossHealtBar;
    private GameObject boss;

    private void Start()
    {
        IgnarionSceneryManagement.InitHealthBar += BossHealthBar;
        ZephyrosSceneryManagement.InitHealthBar += BossHealthBar;
    }

    private void BossHealthBar()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");
        if (boss != null)
        {
            bossHealtBar.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        IgnarionSceneryManagement.InitHealthBar -= BossHealthBar;
        ZephyrosSceneryManagement.InitHealthBar -= BossHealthBar;
    }
}
