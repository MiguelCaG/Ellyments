using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointTend : MonoBehaviour
{
    [SerializeField] private PlayerData pD;
    [SerializeField] private TextMeshPro pressW;
    [SerializeField] private Animator tendAnim;

    private bool settingSpawn = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(AppearText());
            if (Input.GetAxis("Vertical") > 0f && !settingSpawn)
            {
                StartCoroutine(HandleSetSpawnAnim());
                pD.lastCheckpoint = new PlayerData.Checkpoint(false, SceneManager.GetActiveScene().name, transform.position);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(FadeText());
        }
    }

    private IEnumerator FadeText()
    {
        while (pressW.alpha > 0f)
        {
            pressW.alpha -= 0.01f;
            yield return null;
        }
    }

    private IEnumerator AppearText()
    {
        while (pressW.alpha < 1f)
        {
            pressW.alpha += 0.01f;
            yield return null;
        }
    }

    private IEnumerator HandleSetSpawnAnim()
    {
        settingSpawn = true;

        tendAnim.SetTrigger("SpawnSet");

        yield return new WaitUntil(() => tendAnim.GetCurrentAnimatorStateInfo(0).IsName("CheckpointOn"));

        yield return new WaitUntil(() => !tendAnim.GetCurrentAnimatorStateInfo(0).IsName("CheckpointOn"));

        settingSpawn = false;
    }
}
