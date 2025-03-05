using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerBehaviour;

public class Hit
{
    private Vector3 hitOrigin;
    private float hitRadius;
    private float hitDamage;
    private Element elemAttack;

    public Hit(Vector3 hitOrigin, float hitRadius, float hitDamage, Element elemAttack)
    {
        this.hitOrigin = hitOrigin;
        this.hitRadius = hitRadius;
        this.hitDamage = hitDamage;
        this.elemAttack = elemAttack;
    }

    public Vector3 GetHitOrigin()
    {
        return this.hitOrigin;
    }

    public void SetHitOrigin(Vector3 newHitOrigin)
    {
        this.hitOrigin = newHitOrigin;
    }

    public float GetHitRadius()
    {
        return this.hitRadius;
    }

    public float GetHitDamage()
    {
        return this.hitDamage;
    }

    public Element GetElemAttack()
    {
        return this.elemAttack;
    }
}
