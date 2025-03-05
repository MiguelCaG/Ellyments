using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZoneChanger : MonoBehaviour
{
    [SerializeField] private PlayerData pD;

    public static event Action<string> ChangeScene;

    [SerializeField] private string newScene;
    [SerializeField] private Vector2 newPosition;
    [SerializeField] private float newDirection;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (newScene != null || newScene != "")
            {
                pD.newTravelZone = new PlayerData.TravelZone(true, newScene, newPosition, newDirection);
                ChangeScene?.Invoke(pD.newTravelZone.newScene);
            }
        }
    }
}
