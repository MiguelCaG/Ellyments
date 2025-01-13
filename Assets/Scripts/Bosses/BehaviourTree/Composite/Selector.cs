using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : BTNode
{
    private List<BTNode> childrens;
    private List<BTNode> failedChildrens;
    private Dictionary<BTNode, float> childrensProbabilities;
    private int childrenExecuting = 0;
    private float totalProbability = 0f;
    private float maxProbability;
    private float randomChildren;
    private float currentProbability = 0f;

    public Selector(List<BTNode> childrens, List<float> probabilities = null)
    {
        this.childrens = childrens;
        failedChildrens = new List<BTNode>();
        childrensProbabilities = new Dictionary<BTNode, float>();

        if (probabilities == null)
        {
            foreach (BTNode child in childrens)
            {
                childrensProbabilities[child] = 0f;
            }
        }
        else
        {
            for (int i = 0; i < childrens.Count; i++)
            {
                childrensProbabilities[childrens[i]] = probabilities[i];
                totalProbability += probabilities[i];
            }
        }

        randomChildren = Random.Range(0f, totalProbability);
        maxProbability = totalProbability;
    }

    public override BTStatus Execute()
    {
        BTStatus status = BTStatus.Failure;

        for (; childrenExecuting < childrens.Count; childrenExecuting++)
        {
            BTNode currentChild = childrens[childrenExecuting];
            if (failedChildrens.Contains(currentChild)) continue;

            currentProbability += childrensProbabilities[currentChild];
            //Debug.Log($"{currentProbability} /// {randomChildren} /// {childrenExecuting}");
            if (currentProbability >= randomChildren)
            {
                status = currentChild.Execute();
                if (status == BTStatus.Success || status == BTStatus.Running)
                {
                    currentProbability -= childrensProbabilities[currentChild];
                    break;
                }
                if (status == BTStatus.Failure)
                {
                    failedChildrens.Add(currentChild);
                    maxProbability -= childrensProbabilities[currentChild];
                    randomChildren = Random.Range(0f, maxProbability);
                    currentProbability = 0;
                    childrenExecuting = -1;
                }
            }
        }

        if (status != BTStatus.Running)
        {
            failedChildrens.Clear();
            childrenExecuting = 0;
            maxProbability = totalProbability;
            randomChildren = Random.Range(0f, totalProbability);
            currentProbability = 0f;
        }
        return status;
    }
}
