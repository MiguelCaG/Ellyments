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

        // FIRST PHASE
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
            return ignarion.EmerginFireAttack() ? BTStatus.Running : BTStatus.Success;
        });

        Sequence emerginFireSequence = new Sequence(new List<BTNode>()
        {
            lookAtPlayer,
            emerginFire
        });

        Leaf volcanicRock = new Leaf(() =>
        {
            return ignarion.VolcanicRainAttack() ? BTStatus.Running : BTStatus.Success;
        });

        Sequence volcanicRockSequence = new Sequence(new List<BTNode>()
        {
            lookAtPlayer,
            volcanicRock
        });

        Leaf approachPlayer = new Leaf(() =>
        {
            int status = ignarion.ApproachPlayer();
            return status switch
            {
                0 => BTStatus.Failure,
                1 => BTStatus.Running,
                2 => BTStatus.Success,
                _ => BTStatus.Failure,
            };
        });

        Leaf flameWhip = new Leaf(() =>
        {
            return ignarion.FlameWhipAttack() ? BTStatus.Running : BTStatus.Success;
        });

        Sequence flameWhipSequence = new Sequence(new List<BTNode>()
        {
            approachPlayer,
            flameWhip
        });

        // FIRST PHASE SELECTOR

        Selector firstPhaseSelector = new Selector(new List<BTNode>()
        {
            emerginFireSequence,
            volcanicRockSequence,
            flameWhipSequence
        }, new List<float> { 30f, 30f, 40f });

        Sequence firstPhaseSequence = new Sequence(new List<BTNode>()
        {
            lifeChecker,
            firstPhaseSelector
        });

        // CHANGE PHASE

        Leaf hasPhaseChanged = new Leaf(() =>
        {
            if (ignarion.secondPhase)
                return BTStatus.Failure;
            return BTStatus.Success;
        });

        Leaf changeIgnarion = new Leaf(() =>
        {
            return ignarion.ChangeIgnarion() ? BTStatus.Running : BTStatus.Success;
        });

        Leaf changeScenery = new Leaf(() =>
        {
            return ignarion.ChangeScenery() ? BTStatus.Running : BTStatus.Success;
        });

        // CHANGE PHASE SELECTOR
        Sequence changePhaseSequence = new Sequence(new List<BTNode>()
        {
            hasPhaseChanged,
            changeIgnarion,
            changeScenery
        });

        // SECOND PHASE
        Leaf heatWave = new Leaf(() =>
        {
            return ignarion.HeatWaveAttack() ? BTStatus.Running : BTStatus.Success;
        });

        Leaf rest = new Leaf(() =>
        {
            return ignarion.Rest() ? BTStatus.Running : BTStatus.Success;
        });

        Sequence heatWaveSequence = new Sequence(new List<BTNode>()
        {
            heatWave,
            rest
        });

        Leaf moltenSpires = new Leaf(() =>
        {
            return ignarion.MoltenSpiresAttack() ? BTStatus.Running : BTStatus.Success;
        });

        Leaf changeSide = new Leaf(() =>
        {
            return ignarion.ChangeSide() ? BTStatus.Running : BTStatus.Success;
        });

        Sequence changeSideSequence = new Sequence(new List<BTNode>()
        {
            changeSide,
            rest
        });

        // SECOND PHASE SELECTOR
        Selector secondPhaseSelector = new Selector(new List<BTNode>()
        {
            heatWaveSequence,
            moltenSpires,
            changeSideSequence
        }, new List<float> { 40f, 40f, 20f });

        // ROOT SELECTOR
        rootNode = new Selector(new List<BTNode>()
        {
            firstPhaseSequence,
            changePhaseSequence,
            secondPhaseSelector
        });
    }

    private void Update()
    {
        rootNode.Execute();
    }
}
