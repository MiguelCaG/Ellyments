using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmerginFire : MonoBehaviour
{
    private static List<Vector2> emerginLocations = new List<Vector2>();

    private float maxLeft = -11f;
    private float maxRight = 11.5f;
    private float maxBottom = -2.6f;
    private float maxTop = 4.5f;

    private Vector3 originalScale;
    private float targetHeight;

    private void Start()
    {
        emerginLocations.Add(transform.position);
        originalScale = transform.localScale;
        targetHeight = maxTop - maxBottom;
        Ignarion.Emerge += Emerge;
    }

    private void Emerge()
    {
        emerginLocations.Remove(new Vector2(transform.position.x, maxBottom));
        Vector2 newLocation;

        do
        {
            newLocation = new Vector2(Random.Range(maxLeft + (originalScale.x / 2f), maxRight - (originalScale.x / 2f)), maxBottom);
        } while (EmerginLocationOccupied(newLocation));

        transform.position = newLocation;
        emerginLocations.Add(newLocation);
        StartCoroutine(ScaleUp());
    }

    private bool EmerginLocationOccupied(Vector2 location)
    {
        foreach (Vector2 emerginLocation in emerginLocations)
        {
            if (Vector2.Distance(emerginLocation, location) <= originalScale.x)
                return true;
        }
        return false;
    }

    private IEnumerator ScaleUp()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        Vector2 startingLocation= transform.position;
        while (GetComponent<SpriteRenderer>().color != new Color(1f, 0.5f, 0f, 1f))
        {
            GetComponent<SpriteRenderer>().color -= new Color(0f, 0.01f, 0.02f, 0f);
            yield return null;
        }
        GetComponent<BoxCollider2D>().enabled = true;
        while (transform.localScale.y < targetHeight)
        {
            transform.localScale += new Vector3(0f, 0.1f, 0f);
            float heightDifference = transform.localScale.y - originalScale.y;
            transform.position = new Vector2(startingLocation.x, startingLocation.y + heightDifference / 2f);
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        GetComponent<BoxCollider2D>().enabled = false;
        transform.localScale = originalScale;
    }

    private void OnDestroy()
    {
        Ignarion.Emerge -= Emerge;
    }
}
