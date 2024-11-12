using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class CheckCollision : MonoBehaviour
{
    public static event Action Stamp;
    private GameObject movingGO;

    private void Start()
    {
        movingGO = transform.parent.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (movingGO.CompareTag("Player"))
            Stamp?.Invoke();
        else if (movingGO.CompareTag("Enemy") && !collision.CompareTag("Player"))
            movingGO.GetComponent<Enemy>().changeDirection = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (movingGO.CompareTag("Enemy") && !collision.CompareTag("Player"))
            movingGO.GetComponent<Enemy>().changeDirection = false;
    }
}
