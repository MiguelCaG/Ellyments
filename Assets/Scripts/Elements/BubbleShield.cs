using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShield : MonoBehaviour
{
    private float useTime;
    private float maxTime;

    public static event Action Bubble;

    private void Awake()
    {
        useTime = -1f;
        maxTime = 3f;
    }

    private void Update()
    {
        if (useTime < 0)
        {
            useTime = Time.time;
        }
        if (useTime + maxTime <= Time.time)
        {
            useTime = -1f;
            Bubble?.Invoke();
            gameObject.SetActive(false);
        }
    }
}