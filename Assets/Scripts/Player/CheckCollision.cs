using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class CheckCollision : MonoBehaviour
{
    public static event Action Stamp;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Stamp?.Invoke();
    }
}
