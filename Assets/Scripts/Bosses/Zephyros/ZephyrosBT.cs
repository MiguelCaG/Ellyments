using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZephyrosBT : MonoBehaviour
{
    [SerializeField] PlayerActionsData pAD;

    private float lastAttackingDistance;

    private BTNode rootNode;

    private Zephyros zephyros;

    private Selector firstPhaseSelector;
    private List<float> firstPhaseBaseWeights = new List<float> { 30f, 20f, 20f, 30f };
    private Selector secondPhaseSelector;
    private List<float> secondPhaseBaseWeights = new List<float> { 25f/*, 25f, 25f, 25f*/ };

    private void Start()
    {
        lastAttackingDistance = pAD.attackingDistance;

        zephyros = GetComponent<Zephyros>();

        // FIRST PHASE
        Leaf lifeChecker = new Leaf(() =>
        {
            if (zephyros.life > zephyros.maxLife * zephyros.changePhasePercentage)
                return BTStatus.Success;
            return BTStatus.Failure;
        });

        Leaf tornado = new Leaf(() =>
        {
            return zephyros.TornadoAttack() ? BTStatus.Running : BTStatus.Success;
        });

        Leaf rest = new Leaf(() =>
        {
            return zephyros.Rest() ? BTStatus.Running : BTStatus.Success;
        });

        Sequence tornadoSequence = new Sequence(new List<BTNode>()
        {
            tornado,
            rest
        });

        Leaf onrush = new Leaf(() =>
        {
            return zephyros.Onrush() ? BTStatus.Running : BTStatus.Success;
        });

        Leaf galeForce = new Leaf(() =>
        {
            return zephyros.GaleForceAttack() ? BTStatus.Running : BTStatus.Success;
        });

        Sequence galeForceSequence = new Sequence(new List<BTNode>()
        {
            onrush,
            galeForce
        });

        Leaf devouringGale = new Leaf(() =>
        {
            int status = zephyros.DevouringGaleAttack();
            return status switch
            {
                0 => BTStatus.Failure,
                1 => BTStatus.Running,
                2 => BTStatus.Success,
                _ => BTStatus.Failure,
            };
        });

        Leaf throwPlayer = new Leaf(() =>
        {
            return zephyros.ThrowPlayer() ? BTStatus.Running : BTStatus.Success;
        });

        Sequence devouringGaleSequence = new Sequence(new List<BTNode>()
        {
            onrush,
            devouringGale,
            throwPlayer
        });

        Leaf aerialRush = new Leaf(() =>
        {
            return zephyros.AerialRushAttack() ? BTStatus.Running : BTStatus.Success;
        });

        // FIRST PHASE SELECTOR
        firstPhaseSelector = new Selector(new List<BTNode>()
        {
            tornadoSequence,
            galeForceSequence,
            devouringGaleSequence,
            aerialRush
        }, firstPhaseBaseWeights);

        Sequence firstPhaseSequence = new Sequence(new List<BTNode>()
        {
            lifeChecker,
            firstPhaseSelector
        });

        // CHANGE PHASE
        Leaf hasPhaseChanged = new Leaf(() =>
        {
            if (zephyros.secondPhase)
                return BTStatus.Failure;
            return BTStatus.Success;
        });

        Leaf changeZephyros = new Leaf(() =>
        {
            return zephyros.ChangeZephyros() ? BTStatus.Running : BTStatus.Success;
        });

        Leaf changeScenery = new Leaf(() =>
        {
            zephyros.ChangeScenery();
            return BTStatus.Success;
        });

        // CHANGE PHASE SEQUENCE
        Sequence changePhaseSequence = new Sequence(new List<BTNode>()
        {
            hasPhaseChanged,
            changeZephyros,
            changeScenery
        });

        // SECOND PHASE
        //Leaf blizzard = new Leaf(() =>
        //{

        //});

        //Leaf sandstorm = new Leaf(() =>
        //{

        //});

        //Leaf staticShock = new Leaf(() =>
        //{

        //});

        // SECOND PHASE SELECTOR
        secondPhaseSelector = new Selector(new List<BTNode>()
        {
            firstPhaseSelector,
            //blizzard,
            //sandstorm,
            //staticShock
        }, secondPhaseBaseWeights);


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
        if (zephyros.alive)
        {
            BTStatus status = rootNode.Execute();

            if (status != BTStatus.Running)
            {
                UpdateWeights();
            }
        }
    }

    private void UpdateWeights()
    {
        if (Mathf.Approximately(lastAttackingDistance, pAD.attackingDistance)) return;

        lastAttackingDistance = pAD.attackingDistance;

        firstPhaseSelector.SetWeights(
            firstPhaseSelector.Childrens,
            new List<float> {
                Mathf.Clamp(firstPhaseBaseWeights[0] + pAD.attackingDistance, 5f, 50f),
                Mathf.Clamp(firstPhaseBaseWeights[1] - pAD.attackingDistance, 5f, 50f),
                Mathf.Clamp(firstPhaseBaseWeights[2] - pAD.attackingDistance, 5f, 50f),
                Mathf.Clamp(firstPhaseBaseWeights[3] + pAD.attackingDistance, 5f, 50f)
            }
        );
    }
}
