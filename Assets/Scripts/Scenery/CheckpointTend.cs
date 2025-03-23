using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointTend : InteractiveObject
{
    [SerializeField] private PlayerData pD;
    [SerializeField] private Animator tendAnim;

    private bool settingSpawn = false;

    private new void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(AppearText());
            if (Input.GetAxis("Vertical") > 0f && !settingSpawn)
            {
                HealthManager hM = collision.GetComponent<HealthManager>();
                hM.UpdateLife(hM.GetMaxHealth());
                StartCoroutine(HandleSetSpawnAnim());
                pD.lastCheckpoint = new PlayerData.Checkpoint(false, SceneManager.GetActiveScene().name, transform.position);
                
                pD.SaveState();
            }
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
