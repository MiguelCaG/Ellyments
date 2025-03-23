using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    private TMP_Text dialogueText;
    [SerializeField, TextArea(4, 6)] private string[] dialogueLines;

    [HideInInspector] public bool dialogueStarted = false;
    private int lineIndex;

    private float typingTime = 0.05f;

    private bool playerInRange = false;
    private int triggerDialogue = -1;
    private bool uniqueDialogue = false;

    private ElemDrop elemDrop;
    private StatueElection statueElection;

    private void Start()
    {
        dialogueText = dialoguePanel.transform.GetChild(0).GetComponent<TMP_Text>();

        elemDrop = GetComponent<ElemDrop>();
        statueElection = GetComponent<StatueElection>();
    }

    private void Update()
    {
        if (!playerInRange) return;

        if ((triggerDialogue != -1 || (dialogueStarted && Input.GetButtonDown("Fire1"))) && !uniqueDialogue)
        {
            if (!dialogueStarted) StartDialog();
            else if (dialogueText.text == dialogueLines[lineIndex]) NextDialogueLine();
            else { StopAllCoroutines(); dialogueText.text = dialogueLines[lineIndex]; }
        }
    }

    private void StartDialog()
    {
        EventManager.InvokeStopMove();

        lineIndex = 0;
        triggerDialogue = -1;
        dialogueStarted = true;
        dialoguePanel.SetActive(true);
        StartCoroutine(ShowLine());
    }

    private void NextDialogueLine()
    {
        if (++lineIndex < dialogueLines.Length) StartCoroutine(ShowLine());
        else EndDialogue();
    }

    private void EndDialogue()
    {
        playerInRange = false;
        dialoguePanel.SetActive(false);
        dialogueStarted = false;

        EventManager.InvokeStopMove();

        if (elemDrop != null)
        {
            gameObject.SetActive(false);
        }
        else if (statueElection != null)
        {
            uniqueDialogue = true;
        }
    }

    private IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;

        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            triggerDialogue = 1;
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
