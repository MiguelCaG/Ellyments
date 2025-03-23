using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Destructibleobject : MonoBehaviour
{
    [SerializeField] private SceneData sD;

    private string sceneName;

    private void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;

        if (sD.HasObjectFinalized(sceneName, gameObject.name))
        {
            gameObject.SetActive(false);
        }
    }

    public void DestroyObject()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        sD.MarkObjectFinalized(sceneName, gameObject.name);

        gameObject.SetActive(false);
    }
}
