using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HitController : MonoBehaviour
{
    private Transform hitController;
    [SerializeField] private float hitRadius;

    private void Start()
    {
        hitController = gameObject.GetComponent<Transform>();
        PlayerBehaviour.RockPunch += Golpe;
    }

    private void Golpe()
    {
        Collider2D[] collides = Physics2D.OverlapCircleAll(hitController.position, hitRadius);

        foreach (Collider2D c in collides)
        {
            if (c.CompareTag("Enemy"))
            {
                c.GetComponent<Enemy>().UpdateLife(-3f);
            }
        }

        // TEMPORAL -------------------------------------
        StartCoroutine(ExampleCoroutine());
        // ----------------------------------------------
    }

    // TEMPORAL -------------------------------------
    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
    // ----------------------------------------------
}
