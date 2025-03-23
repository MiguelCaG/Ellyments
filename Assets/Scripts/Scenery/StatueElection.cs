using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static AbilityManager;

public class StatueElection : MonoBehaviour
{
    [SerializeField] private AbilityManager aM;

    [SerializeField] private Ability ability1;
    [SerializeField] private Ability ability2;

    private GameObject[] elemDrops;
    private TextMeshPro[] electionTexts;

    private void Start()
    {
        elemDrops = new GameObject[2];
        electionTexts = new TextMeshPro[3];

        for (int i = 0; i < transform.childCount; i++)
        {
            if (i < 2) elemDrops[i] = transform.GetChild(i).gameObject;
            else electionTexts[i-2] = transform.GetChild(i).GetComponent<TextMeshPro>();
        }

        if (!aM.IsAbilityUnlocked(ability1) && !aM.IsAbilityUnlocked(ability2))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
            for(int i = 0; i < elemDrops.Length; i++)
            {
                elemDrops[i].SetActive(true);
            }
        }
    }

    private void Elected(Ability ability)
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;

        aM.UnlockAbility(ability);

        for (int i = 0; i < elemDrops.Length; i++)
        {
            elemDrops[i].SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !gameObject.GetComponent<DialogueManager>().dialogueStarted)
        {
            StartCoroutine(AppearText(electionTexts[0]));
            if (collision.transform.position.x - transform.position.x < 0f)
            {
                StartCoroutine(FadeText(electionTexts[2]));
                StartCoroutine(AppearText(electionTexts[1]));
                if (Input.GetAxis("Vertical") > 0f) Elected(ability1);
            }
            else
            {
                StartCoroutine(FadeText(electionTexts[1]));
                StartCoroutine(AppearText(electionTexts[2]));
                if (Input.GetAxis("Vertical") > 0f) Elected(ability2);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (TextMeshPro text in electionTexts)
            {
                StartCoroutine(FadeText(text));
            }
        }
    }

    private IEnumerator FadeText(TextMeshPro text)
    {
        while (text.alpha > 0f)
        {
            text.alpha -= 0.01f;
            yield return null;
        }
    }

    private IEnumerator AppearText(TextMeshPro text)
    {
        while (text.alpha < 1f)
        {
            text.alpha += 0.01f;
            yield return null;
        }
    }
}
