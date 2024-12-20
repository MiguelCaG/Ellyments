using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : BTNode
{
    private List<BTNode> childrens;
    private Dictionary<BTNode, float> childrensProbabilities;
    private int childrenExecuting = 0;
    private float randomChildren = Random.Range(1f, 100f);
    private float actualProbability = 0f;

    public Selector(List<BTNode> childrens)
    {
        this.childrens = childrens;
        childrensProbabilities = new Dictionary<BTNode, float>();
        foreach (BTNode child in childrens)
        {
            childrensProbabilities[child] = 100f;
        }
    }

    public Selector(List<BTNode> childrens, List<float> probabilities)
    {
        this.childrens = childrens;
        childrensProbabilities = new Dictionary<BTNode, float>();
        for (int i = 0; i < childrens.Count; i++)
        {
            childrensProbabilities[childrens[i]] = probabilities[i];
        }
    }

    public override BTStatus Execute()
    {
        BTStatus status = BTStatus.Failure;

        for (; childrenExecuting < childrens.Count; childrenExecuting++)
        {
            actualProbability += childrensProbabilities[childrens[childrenExecuting]];
            //Debug.Log($"{actualProbability} /// {randomChildren} /// {childrenExecuting}");
            if (actualProbability >= randomChildren)
            {
                status = childrens[childrenExecuting].Execute();
                if (status == BTStatus.Success || status == BTStatus.Running)
                {
                    actualProbability -= childrensProbabilities[childrens[childrenExecuting]];
                    break;
                }
            }
        }

        if (status != BTStatus.Running)
        {
            childrenExecuting = 0;
            randomChildren = Random.Range(1f, 100f);
            actualProbability = 0f;
        }
        return status;
    }
}
