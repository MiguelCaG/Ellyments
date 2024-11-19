using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HitController : MonoBehaviour
{
    private Transform hitController;
    private float hitRadius;

    private GameObject attackerGO;

    private void Start()
    {
        hitController = gameObject.GetComponent<Transform>();
        hitRadius = 0.5f;

        attackerGO = transform.parent.gameObject;
        if (attackerGO.CompareTag("Player"))
        {
            PlayerBehaviour.RockPunch += Hit;
        }
        else if (attackerGO.CompareTag("Enemy"))
        {
            Enemy enemy = attackerGO.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Damage += Hit;

            }
        }

    }

    private void Hit()
    {
        Collider2D[] collides = Physics2D.OverlapCircleAll(hitController.position, hitRadius);

        foreach (Collider2D c in collides)
        {
            if (attackerGO.CompareTag("Player") && c.CompareTag("Enemy"))
            {
                c.GetComponent<Enemy>().UpdateLife(-5f);
            }
            else if (attackerGO.CompareTag("Enemy") && c.CompareTag("Player"))
            {
                Debug.Log("PLAYER HIT");
            }
        }

        // TEMPORAL -------------------------------------
        StartCoroutine(ExampleCoroutine());
        // ----------------------------------------------
    }

    private void OnDestroy()
    {
        if (attackerGO != null && attackerGO.CompareTag("Enemy"))
        {
            Enemy enemy = attackerGO.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Damage -= Hit;
            }
        }
    }

    // TEMPORAL -------------------------------------
    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
    // ----------------------------------------------
}
