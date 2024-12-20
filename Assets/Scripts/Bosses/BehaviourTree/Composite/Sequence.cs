using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : BTNode
{
    private List<BTNode> childrens;
    private int childrenExecuting = 0;

    public Sequence(List<BTNode> childrens)
    {
        this.childrens = childrens;
    }

    public override BTStatus Execute()
    {
        BTStatus status = BTStatus.Failure;

        for (; childrenExecuting < childrens.Count; childrenExecuting++)
        {
            status = childrens[childrenExecuting].Execute();
            if (status == BTStatus.Failure || status == BTStatus.Running) break;
        }

        if (status != BTStatus.Running) childrenExecuting = 0;

        return status;
    }
}
