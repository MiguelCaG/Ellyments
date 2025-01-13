using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    private GameObject movingGO;
    private int floorContacts = 0;
    private Vector3 lastSafeZone;

    private void Start()
    {
        movingGO = transform.parent.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("EN: " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Floor"))
        {
            floorContacts++;
            if (movingGO.CompareTag("Player"))
                movingGO.GetComponent<PlayerBehaviour>().onGround = true;
            else if (movingGO.CompareTag("Enemy") && !collision.CompareTag("Player"))
                movingGO.GetComponent<Enemy>().changeDirection = false;
        } else if (collision.gameObject.CompareTag("Hazzard") && movingGO.CompareTag("Player"))
        {
            movingGO.gameObject.GetComponent<HealthManager>().UpdateLife(-1);
            Debug.LogWarning("TODO: LINE BELOW DOESN'T WORK");
            movingGO.transform.position = lastSafeZone; // DOESN'T WORK AS EXPECTED
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("FUERA: " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Floor"))
        {
            floorContacts--;
            if (floorContacts <= 0)
            {
                if (movingGO.CompareTag("Player"))
                {
                    movingGO.GetComponent<PlayerBehaviour>().onGround = false;
                    lastSafeZone = movingGO.transform.position;
                }
                else if (movingGO.CompareTag("Enemy") && !collision.CompareTag("Player"))
                    movingGO.GetComponent<Enemy>().changeDirection = true;
            }
        }
    }
}
