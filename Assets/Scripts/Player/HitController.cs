using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HitController : MonoBehaviour
{
    private GameObject attackerGO;

    private void Start()
    {
        attackerGO = transform.parent.gameObject;
        if (attackerGO.CompareTag("Player"))
        {
            PlayerBehaviour.Damage += Hit;
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

    private void Hit(Hit hit)
    {
        Hit(hit, false);
    }

    private void Hit(Hit hit, bool restoreAura)
    {
        Vector3 origin = hit.GetHitOrigin();
        float radius = hit.GetHitRadius();
        float damage = hit.GetHitDamage();

        Collider2D[] collides = Physics2D.OverlapCircleAll(origin, radius);

        foreach (Collider2D c in collides)
        {
            if (attackerGO.CompareTag("Player"))
            {
                if(c.CompareTag("Enemy")) c.GetComponent<Enemy>().UpdateLife(damage);
                else if(c.CompareTag("Boss")) c.GetComponent<Boss>().UpdateLife(damage);
                if ((c.CompareTag("Enemy") || c.CompareTag("Boss")) && restoreAura) attackerGO.GetComponent<AuraManager>().UpdateAura(1f);
            }
            else if (attackerGO.CompareTag("Enemy") && c.CompareTag("Player"))
            {
                c.GetComponent<HealthManager>().UpdateLife((int)damage);
            }
        }

        // TEMPORAL -------------------------------------
        StartCoroutine(ExampleCoroutine(hit.GetHitDamage()));
        // ----------------------------------------------
    }

    private void OnDestroy()
    {
        if (attackerGO.CompareTag("Player"))
        {
            PlayerBehaviour.Damage -= Hit;
        }
        else if (attackerGO != null && attackerGO.CompareTag("Enemy"))
        {
            Enemy enemy = attackerGO.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Damage -= Hit;
            }
        }
    }

    // TEMPORAL -------------------------------------
    IEnumerator ExampleCoroutine(float damage)
    {
        yield return new WaitForSeconds(0.5f);

        if (damage == -5f || damage == -15f)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (damage == -10f)
        {
            transform.parent.transform.GetChild(4).gameObject.SetActive(false);
            transform.parent.transform.GetChild(4).gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
    // ----------------------------------------------
}
