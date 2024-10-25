using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    public static bool onGround;
    private int floorContacts = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("EN: " + collision.gameObject.tag);
        if (collision.gameObject.tag == "Floor")
        {
            floorContacts++;
            onGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("FUERA: " + collision.gameObject.tag);
        if (collision.gameObject.tag == "Floor")
        {
            floorContacts--;
            if (floorContacts <= 0)
            {
                onGround = false;
            }
        }
    }
}
