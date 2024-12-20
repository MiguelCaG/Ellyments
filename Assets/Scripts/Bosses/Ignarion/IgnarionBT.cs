using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnarionBT : MonoBehaviour
{
    private BTNode rootNode;

    private Ignarion ignarion;

    private void Start()
    {
        ignarion = GetComponent<Ignarion>();

        // FIRST FASE
        Leaf lifeChecker = new Leaf(() =>
        {
            if (ignarion.life > ignarion.maxLife * 0.6f)
                return BTStatus.Success;
            return BTStatus.Failure;
        });

        Leaf lookAtPlayer = new Leaf(() =>
        {
            ignarion.LookAtPlayer();
            return BTStatus.Success;
        });

        Leaf emerginFire = new Leaf(() =>
        {
            Debug.Log("EMERGIN FIRE");
            return ignarion.EmerginFire() ? BTStatus.Running : BTStatus.Success;
        });

        Sequence emerginFireSequence = new Sequence(new List<BTNode>()
        {
            lookAtPlayer,
            emerginFire
        });

        Leaf volcanicRock = new Leaf(() =>
        {
            Debug.Log("VOLCANIC ROCK");
            return ignarion.VolcanicRain() ? BTStatus.Running : BTStatus.Success;
        });

        Sequence volcanicRockSequence = new Sequence(new List<BTNode>()
        {
            lookAtPlayer,
            volcanicRock
        });

        Leaf approachPlayer = new Leaf(() =>
        {
            return ignarion.ApproachPlayer() ? BTStatus.Running : BTStatus.Success;
        });

        Leaf flameWhip = new Leaf(() =>
        {
            Debug.Log("FLAME WHIP");
            return BTStatus.Success;
        });

        Sequence flameWhipSequence = new Sequence(new List<BTNode>()
        {
            approachPlayer,
            flameWhip
        });

        Selector firstFaseSelector = new Selector(new List<BTNode>()
        {
            emerginFireSequence,
            volcanicRockSequence,
            flameWhipSequence
        }, new List<float> { 30f, 30f, 40f });

        Sequence firstFaseSequence = new Sequence(new List<BTNode>()
        {
            lifeChecker,
            firstFaseSelector
        });

        // CHANGE FASE
        Sequence changeFaseSequence = new Sequence(new List<BTNode>()
        {
            approachPlayer
        });

        // SECOND FASE
        Selector secondFaseSelector = new Selector(new List<BTNode>()
        {

        });

        // ROOT SELECTOR
        rootNode = new Selector(new List<BTNode>()
        {
            firstFaseSequence,
            changeFaseSequence,
            secondFaseSelector
        });
    }

    private void Update()
    {
        rootNode.Execute();
    }
}
