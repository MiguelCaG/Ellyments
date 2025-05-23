using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static AbilityManager;

public class TutorialSceneryManagement : MonoBehaviour
{
    [SerializeField] private AbilityManager aM;
    [SerializeField] private PlayerData pD;

    private GameObject player;

    [SerializeField] private GameObject[] breakables;

    [SerializeField] private GameObject[] enemies;
    private int kills = 0;

    [SerializeField] private GameObject fireElem;

    private GameObject flammable;

    [SerializeField] private TextMeshPro[] tutorials;
    private int iTuto = 0;

    private Dictionary<int, System.Func<bool>> tutorialConditions;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        foreach (GameObject enemy in enemies)
            enemy.GetComponent<Enemy>().Killed += UnlockFireball;

        flammable = GameObject.FindGameObjectWithTag("Flammable");

        if (aM.IsAbilityUnlocked(Ability.Fireball))
        {
            tutorials[4].text = "Abre el menu elemental manteniendo pulsado <sprite name=\"TabKey\"> y selecciona el poder elemental <sprite name=\"Fire\">.";
            StartCoroutine(AppearText(4));
        }

        tutorialConditions = new Dictionary<int, System.Func<bool>>
        {
            { 0, () => Input.GetAxis("Horizontal") != 0 && PlayerNear() },
            { 1, () => Input.GetKeyDown(KeyCode.Space) && PlayerNear() },
            { 2, () => Input.GetKey(KeyCode.S) && PlayerPosition(23.25f, 24.82f) },
            { 3, () => ObstaclesBroken() },
            { 4, () => FireElemSelected() },
            { 5, () => ObstaclesBurned() },
            { 6, () => CheckpointSet() },
        };
    }

    private void Update()
    {
        if (iTuto >= tutorials.Length) return;

        if (tutorialConditions[iTuto]())
        {
            StartCoroutine(FadeText(iTuto));
            iTuto++;
        }
    }

    private IEnumerator FadeText(int i)
    {
        while (tutorials[i].alpha > 0f)
        {
            tutorials[i].alpha -= 0.01f;
            yield return null;
        }
    }

    private IEnumerator AppearText(int i)
    {
        while (tutorials[i].alpha < 1f)
        {
            tutorials[i].alpha += 0.01f;
            yield return null;
        }
    }

    private bool PlayerNear(float distance = 5f)
    {
        return Vector3.Distance(player.transform.position, tutorials[iTuto].transform.position) <= distance;
    }

    private bool PlayerPosition(float pos1, float pos2)
    {
        return player.transform.position.x >= pos1 && player.transform.position.x <= pos2;
    }

    private bool ObstaclesBroken()
    {
        foreach (GameObject obs in breakables)
        {
            if (obs.activeSelf == true) return false;
        }
        return true;
    }

    private void UnlockFireball(GameObject enemy)
    {
        kills++;
        if (kills == enemies.Length && !aM.IsAbilityUnlocked(Ability.Fireball))
        {
            fireElem.transform.position = enemy.transform.position;
            fireElem.SetActive(true);

            tutorials[4].transform.position = enemy.transform.position + Vector3.up * 2f;
            StartCoroutine(AppearText(4));
        }
    }

    private bool FireElemSelected()
    {
        if (pD.currentElement == PlayerBehaviour.Element.Fire)
        {
            StartCoroutine(AppearText(5));
            return true;
        }
        return false;
    }

    private bool ObstaclesBurned()
    {
        if (flammable != null)
            return flammable.activeSelf == false;
        else
            return true;
    }

    private bool CheckpointSet()
    {
        return pD.lastCheckpoint.CheckpointEquals("TutorialZone", new Vector2(57.1f, -3.35f));
    }
}
