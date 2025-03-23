using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    private GameObject movingGO;
    [SerializeField] private int floorContacts = 0;
    private GameObject[] safeZones;
    private Vector3 closestSafeZone;

    private void Start()
    {
        movingGO = transform.parent.gameObject;

        if (movingGO.CompareTag("Player"))
        {
            safeZones = GameObject.FindGameObjectsWithTag("SafeZone");
        }
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
        }
        else if (collision.gameObject.CompareTag("Hazzard"))
        {
            if (movingGO.CompareTag("Player"))
            {
                movingGO.gameObject.GetComponent<HealthManager>().UpdateLife(-1);
                movingGO.transform.position = closestSafeZone;
            }
            else if (movingGO.CompareTag("Enemy"))
            {
                movingGO.gameObject.GetComponent<Enemy>().UpdateLife(-movingGO.gameObject.GetComponent<Enemy>().maxLife, PlayerBehaviour.Element.None);
            }
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
                    if (safeZones.Length > 0)
                    {
                        closestSafeZone = ClosestSafeZone();
                    }
                }
                else if (movingGO.CompareTag("Enemy") && !collision.CompareTag("Player"))
                    movingGO.GetComponent<Enemy>().changeDirection = true;
            }
        }
    }

    private Vector3 ClosestSafeZone()
    {
        Vector3 safeZone = movingGO.transform.position;
        float distance = Mathf.Infinity;

        for (int i = 0; i < safeZones.Length; i++)
        {
            float newDistance = Mathf.Abs(Vector3.Distance(safeZones[i].transform.position, movingGO.transform.position));
            if (newDistance < distance)
            {
                distance = newDistance;
                safeZone = safeZones[i].transform.position;
            }
        }

        return safeZone;
    }
}
