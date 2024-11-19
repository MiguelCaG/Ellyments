using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamarkoFSM : MonoBehaviour
{
    public enum FSM1State { PATROL, SHOOT, RELOAD };
    public FSM1State fsm1State;

    private Flamarko flamarko;

    private void Start()
    {
        fsm1State = FSM1State.PATROL;
        flamarko = gameObject.GetComponent<Flamarko>();
    }

    private void FixedUpdate()
    {
        FSMLevel1();
    }


    private void FSMLevel1()
    {
        switch (fsm1State)
        {
            case FSM1State.PATROL:
                flamarko.Patrol();
                break;

            case FSM1State.SHOOT:
                flamarko.Shoot();
                break;

            case FSM1State.RELOAD:
                flamarko.Reload();
                break;
        }
    }
}
