using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField] protected TextMeshPro text;

    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(AppearText());
            if (Input.GetAxis("Vertical") > 0f)
            {
                Debug.Log("ENTRA");
            }
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(FadeText());
        }
    }

    protected IEnumerator FadeText()
    {
        while (text.alpha > 0f)
        {
            text.alpha -= 0.01f;
            yield return null;
        }
    }

    protected IEnumerator AppearText()
    {
        while (text.alpha < 1f)
        {
            text.alpha += 0.01f;
            yield return null;
        }
    }
}
