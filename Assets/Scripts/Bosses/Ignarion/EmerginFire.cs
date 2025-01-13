using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmerginFire : MonoBehaviour
{
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private float target;

    public static event Action EmerginCompleted;

    private void Start()
    {
        originalScale = transform.localScale;
        originalRotation = transform.localRotation;
    }

    public void Emerge(float target)
    {
        this.target = target;
        StartCoroutine(ScaleUp());
    }

    private IEnumerator ScaleUp()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        Vector2 startingLocation = transform.position;
        while (GetComponent<SpriteRenderer>().color != new Color(1f, 1f, 1f, 1f))
        {
            GetComponent<SpriteRenderer>().color += new Color(0f, 0f, 0f, 0.01f);
            yield return null;
        }
        GetComponent<BoxCollider2D>().enabled = true;
        while (transform.localScale.y < Math.Abs(target))
        {
            transform.localScale += new Vector3(0f, 0.1f, 0f);
            float heightDifference = transform.localScale.y - originalScale.y;
            if (transform.eulerAngles.z == 0f)
            {
                transform.position = new Vector2(startingLocation.x, startingLocation.y + heightDifference / 2f);
            }
            else if (transform.eulerAngles.z == 90f)
            {
                transform.localScale += new Vector3(0f, 0.2f, 0f);
                transform.position = new Vector2(startingLocation.x - heightDifference / 2f, startingLocation.y);
            }
            else if (transform.eulerAngles.z == 270f)
            {
                transform.localScale += new Vector3(0f, 0.2f, 0f);
                transform.position = new Vector2(startingLocation.x + heightDifference / 2f, startingLocation.y);
            }
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        GetComponent<BoxCollider2D>().enabled = false;
        transform.localScale = originalScale;
        transform.localRotation = originalRotation;
        EmerginCompleted?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<HealthManager>().UpdateLife(-1);
        }
    }
}
