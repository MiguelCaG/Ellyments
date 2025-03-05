using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AbilityManager;

public class AbilityAltar : MonoBehaviour
{
    [SerializeField] private AbilityManager aM;

    [SerializeField] private Ability ability;

    private GameObject elemDrop;

    private void Start()
    {
        elemDrop = transform.GetChild(0).gameObject;

        if (!aM.IsAbilityUnlocked(ability))
        {
            elemDrop.SetActive(true);
        }
    }
}
