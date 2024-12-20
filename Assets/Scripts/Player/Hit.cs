using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit
{
    private Vector3 hitOrigin;
    private float hitRadius;
    private float hitDamage;

    public Hit(Vector3 hitOrigin, float hitRadius, float hitDamage)
    {
        this.hitOrigin = hitOrigin;
        this.hitRadius = hitRadius;
        this.hitDamage = hitDamage;
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
}
