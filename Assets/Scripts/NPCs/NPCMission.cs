using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCMission : MonoBehaviour
{
    [SerializeField] private SceneData sD;

    private GameObject dialogueMark;
    [SerializeField] private GameObject dialoguePanel;
    private TMP_Text dialogueText;
    [SerializeField, TextArea(4, 6)] private string[] dialogueLines;

    [SerializeField] private int[] differentDialogues;
    private int dialogueIndex;
    private int triggerDialogue = -1;

    private bool dialogueStarted = false;
    private int lineIndex;

    private float typingTime = 0.05f;

    private GameObject npc;
    private Vector3 origLookDirection;

    private GameObject triggerZone;

    private GameObject player;
    private bool playerInRange = false;

    [SerializeField] private GameObject missionObject;
    private bool missionComplete = false;

    [SerializeField] private GameObject mine;

    private void Start()
    {
        dialogueMark = transform.GetChild(0).gameObject;
        dialogueText = dialoguePanel.transform.GetChild(0).GetComponent<TMP_Text>();

        npc = transform.GetChild(1).gameObject;
        origLookDirection = npc.transform.localScale;

        triggerZone = transform.GetChild(2).gameObject;
        if (sD.HasObjectFinalized(SceneManager.GetActiveScene().name, missionObject.name))
        {
            triggerZone.SetActive(false);
            mine.SetActive(false);
            dialogueIndex = differentDialogues.Length - 1;
        }

        if (sD.HasObjectFinalized(SceneManager.GetActiveScene().name, gameObject.name))
        {
            gameObject.SetActive(false);
        }

        player = GameObject.FindGameObjectWithTag("Player");

        EventManager.ZoneTriggered += ZoneTriggered;
        PickableObject.ObjectPicked += MissionComplete;
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (triggerDialogue != -1 || (!dialogueStarted && Input.GetAxis("Vertical") > 0f) || (dialogueStarted && Input.GetButtonDown("Fire1")))
        {
            if (!dialogueStarted) StartDialog();
            else if (dialogueText.text == dialogueLines[lineIndex]) NextDialogueLine();
            else { StopAllCoroutines(); dialogueText.text = dialogueLines[lineIndex]; }
        }
    }

    private void StartDialog()
    {
        EventManager.InvokeStopMove();

        npc.transform.localScale = new Vector2(GetLookDirection(), transform.localScale.y);

        lineIndex = triggerDialogue == 0 ? 0 : differentDialogues[dialogueIndex - 1];
        triggerDialogue = -1;
        dialogueStarted = true;
        dialogueMark.SetActive(false);
        dialoguePanel.SetActive(true);
        StartCoroutine(ShowLine());
    }

    private void NextDialogueLine()
    {
        if (++lineIndex < differentDialogues[dialogueIndex]) StartCoroutine(ShowLine());
        else EndDialogue();
    }

    private void EndDialogue()
    {
        playerInRange = false;
        dialoguePanel.SetActive(false);
        dialogueMark.SetActive(dialogueIndex != 0);
        dialogueStarted = false;

        if (sD.HasObjectFinalized(SceneManager.GetActiveScene().name, missionObject.name))
        {
            dialogueIndex = differentDialogues.Length - 1;
            sD.MarkObjectFinalized(SceneManager.GetActiveScene().name, gameObject.name);
        }
        else
            dialogueIndex = Mathf.Min(dialogueIndex + 1, differentDialogues.Length - 2);

        npc.transform.localScale = origLookDirection;

        EventManager.InvokeStopMove();
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

    private int GetLookDirection() => player.transform.position.x - npc.transform.position.x >= 0 ? 1 : -1;

    private void ZoneTriggered()
    {
        triggerDialogue = 0;
        playerInRange = true;
        triggerZone.SetActive(false);
    }

    private void MissionComplete()
    {
        dialogueIndex = differentDialogues.Length - 1;
        sD.MarkObjectFinalized(SceneManager.GetActiveScene().name, mine.name);
        mine.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dialogueMark.SetActive(true);
            if (!mine.activeSelf && !missionComplete)
            {
                missionComplete = true;
                triggerDialogue = 1;
                playerInRange = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            dialogueMark.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        EventManager.ZoneTriggered -= ZoneTriggered;
        PickableObject.ObjectPicked -= MissionComplete;
    }
}
