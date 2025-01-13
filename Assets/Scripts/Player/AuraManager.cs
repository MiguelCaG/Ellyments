using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuraManager : MonoBehaviour
{
    private float maxAura;
    private float totalAura;
    private float aura;

    public RectTransform auraTransform;
    public Slider auraSlider;

    private void Start()
    {
        auraTransform.sizeDelta = new Vector2(650f, 20f);
        maxAura = 20f;
        totalAura = 40f;
        aura = maxAura;
    }

    private void Update()
    {
        auraTransform.sizeDelta = new Vector2(650f * (maxAura / totalAura), auraTransform.sizeDelta.y);
        auraSlider.maxValue = maxAura;
        auraSlider.value = aura;
    }

    public float GetAura() { return aura; }

    public void AddAura(float add = 5f)
    {
        maxAura += add;
        aura = maxAura;
    }

    public void UpdateAura(float aura)
    {
        if (this.aura + aura < 0f) return;
        else if(this.aura + aura > maxAura) this.aura = maxAura;
        else this.aura += aura;
    }

    public bool CanUseAura(float auraWaste)
    {
        return aura - auraWaste >= 0f;
    }
}
