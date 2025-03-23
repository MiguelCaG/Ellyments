using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner confiner;

    private void OnEnable()
    {
        PlayerBehaviour.OnPlayerRespawnOrTravel += UpdateCameraConfiner;
    }

    private void OnDisable()
    {
        PlayerBehaviour.OnPlayerRespawnOrTravel -= UpdateCameraConfiner;
    }

    private void UpdateCameraConfiner(PolygonCollider2D newConfiner)
    {
        if (confiner != null && newConfiner != null)
        {
            confiner.m_BoundingShape2D = newConfiner;
            confiner.InvalidatePathCache();
        }
    }
}
