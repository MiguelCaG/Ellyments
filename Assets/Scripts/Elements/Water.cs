using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Water : MonoBehaviour
{
    private TilemapCollider2D tmc;

    void Start()
    {
        tmc = GetComponent<TilemapCollider2D>();
        PlayerBehaviour.Bubble += Swim;
        BubblePlatform.Bubble += Shink;
    }

    private void OnDestroy()
    {
        PlayerBehaviour.Bubble -= Swim;
        BubblePlatform.Bubble -= Shink;
    }

    public void Swim()
    {
        tmc.enabled = true;
    }

    public void Shink()
    {
        tmc.enabled = false;
    }
}
