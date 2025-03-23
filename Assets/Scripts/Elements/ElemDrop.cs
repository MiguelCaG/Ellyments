using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static AbilityManager;

public class ElemDrop : MonoBehaviour
{
    [SerializeField] private AbilityManager aM;
    [SerializeField] private Ability unlockeableAbility;

    private float initialRiseHeight = 1f;
    private float fallSpeed = 5f;
    private float floatHeight = 0.2f;
    private float floatSpeed = 2f;

    private Vector3 originalPosition;
    private bool hasFallen = false;

    [SerializeField] private bool election = false;

    private void OnEnable()
    {
        originalPosition = transform.position;
        StartCoroutine(DropSequence());
    }

    private IEnumerator DropSequence()
    {
        Vector3 risePosition = originalPosition + Vector3.up * initialRiseHeight;
        yield return MoveToPosition(risePosition, fallSpeed);

        yield return MoveToPosition(originalPosition, fallSpeed);

        hasFallen = true;

        gameObject.GetComponent<CircleCollider2D>().enabled = true;
    }

    private void Update()
    {
        if (hasFallen)
        {
            transform.position = originalPosition + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        }
    }

    private IEnumerator MoveToPosition(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * speed);
            yield return null;
        }
        transform.position = target;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") || election) return;

        if (!aM.IsAbilityUnlocked(unlockeableAbility))
        {
            aM.UnlockAbility(unlockeableAbility);
        }
    }
}
