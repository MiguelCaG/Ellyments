using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuraManager : MonoBehaviour
{
    [SerializeField] private PlayerData pD;

    private float totalAura;

    public RectTransform auraTransform;
    public Slider auraSlider;

    private void Start()
    {
        auraTransform.sizeDelta = new Vector2(650f, 20f);
        totalAura = 40f;
    }

    private void Update()
    {
        auraTransform.sizeDelta = new Vector2(650f * (pD.maxAura / totalAura), auraTransform.sizeDelta.y);
        auraSlider.maxValue = pD.maxAura;
        auraSlider.value = pD.aura;
    }

    public float GetAura() { return pD.aura; }

    public void AddAura(float add = 5f)
    {
        pD.maxAura += add;
        pD.aura = pD.maxAura;

        pD.SaveState();
    }

    public void UpdateAura(float aura)
    {
        if (pD.aura + aura < 0f) return;
        else if(pD.aura + aura > pD.maxAura) pD.aura = pD.maxAura;
        else pD.aura += aura;

        pD.SaveState();
    }

    public bool CanUseAura(float auraWaste)
    {
        return pD.aura - auraWaste >= 0f;
    }
}
