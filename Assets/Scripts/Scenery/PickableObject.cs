using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickableObject : InteractiveObject
{
    [SerializeField] private SceneData sD;

    private string sceneName;

    private bool picking = false;

    public static event Action ObjectPicked;

    private void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;

        if (sD.HasObjectFinalized(sceneName, gameObject.name))
        {
            gameObject.SetActive(false);
        }
    }

    public void PickObject()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;

        sD.MarkObjectFinalized(sceneName, gameObject.name);

        ObjectPicked?.Invoke();

        gameObject.SetActive(false);
    }

    private new void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(AppearText());
            if (Input.GetAxis("Vertical") > 0f && !picking)
            {
                picking = true;
                PickObject();
            }
        }
    }
}
