using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : BTNode
{
    private Func<BTStatus> function;

    public Leaf(Func<BTStatus> function)
    {
        this.function = function;
    }

    public override BTStatus Execute()
    {
        return function();
    }
}
