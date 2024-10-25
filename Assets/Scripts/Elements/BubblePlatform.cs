using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblePlatform : MonoBehaviour
{
    [SerializeField] private float useTime;
    [SerializeField] private float maxTime;

    public static event Action Bubble;

    private void Awake()
    {
        useTime = Time.time;
        maxTime = Time.time + 10f;
    }

    private void Update()
    {
        if (maxTime <= useTime)
        {
            Bubble?.Invoke();
            gameObject.SetActive(false);
        }
        else
        {
            useTime += Time.deltaTime;
        }
    }
}