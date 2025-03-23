using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    [SerializeField] private AbilityManager aM;
    [SerializeField] private PlayerActionsData pAD;
    [SerializeField] private PlayerData pD;
    [SerializeField] private SceneData sD;

    void Start()
    {
        aM.LoadState();
        pAD.LoadState();
        pD.LoadState();
        sD.LoadState();
    }
}
