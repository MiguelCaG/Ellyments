using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HitController : MonoBehaviour
{
    [SerializeField] private PlayerActionsData pAD;

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
        PlayerBehaviour.Element attackType = hit.GetElemAttack();

        int layerMask = LayerMask.GetMask("Default", "Enemy", "Player");
        Collider2D[] collides = Physics2D.OverlapCircleAll(origin, radius, layerMask);

        foreach (Collider2D c in collides)
        {
            if (attackerGO.CompareTag("Player"))
            {
                if (c.CompareTag("Breakable")) c.gameObject.GetComponent<Destructibleobject>().DestroyObject();
                if(c.CompareTag("Enemy")) c.GetComponent<Enemy>().UpdateLife(damage, attackType);
                else if(c.CompareTag("Boss")) c.GetComponent<Boss>().UpdateLife(damage);
                if ((c.CompareTag("Enemy") || c.CompareTag("Boss")))
                {
                    pAD.attackingDistance = Mathf.Clamp(pAD.attackingDistance - (damage / -10f), -25f, 25f);
                    pAD.aggressiveness = Mathf.Round((pAD.aggressiveness + 0.1f) * 10f) / 10f;

                    pAD.SaveState();

                    if (restoreAura) attackerGO.GetComponent<AuraManager>().UpdateAura(1f);
                }
                if (c.CompareTag("Dummy") && restoreAura) attackerGO.GetComponent<AuraManager>().UpdateAura(2f);
            }
            else if (attackerGO.CompareTag("Enemy") && c.CompareTag("Player"))
            {
                //HealthManager hm = c.gameObject.GetComponent<HealthManager>();
                //Debug.Log("Objeto colisionado: " + c.gameObject.name + " ¿Tiene HealthManager?: " + (hm != null));
                c.gameObject.GetComponent<HealthManager>().UpdateLife((int)damage);
            }
        }
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
}
