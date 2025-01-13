using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanicRock : MonoBehaviour
{
    private float fallingSpeed = 5f;
    [HideInInspector] public bool fallen = true;
    public void Fall(float maxBottom, float maxLeft)
    {
        StartCoroutine(FallDown(maxBottom, maxLeft));
    }
    private IEnumerator FallDown(float maxBottom, float maxLeft)
    {
        fallen = false;

        yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 2f));

        float desviation = UnityEngine.Random.Range(1f, 4f);

        Vector3 startingPosition = transform.position;

        float yDistance = startingPosition.y - (maxBottom);
        float timeToFall = yDistance / fallingSpeed;

        float xOffset = (fallingSpeed / desviation) * timeToFall;

        float finalX = startingPosition.x - xOffset;

        desviation *= finalX < maxLeft ? -1 : 1;

        while (transform.position.y > maxBottom)
        {
            transform.position -= new Vector3(fallingSpeed / desviation * Time.deltaTime, fallingSpeed * Time.deltaTime, 0f);
            yield return null;
        }

        fallen = true;
        transform.position = new Vector3(transform.position.x, 10f); ;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<HealthManager>().UpdateLife(-1);
        }
    }
}
