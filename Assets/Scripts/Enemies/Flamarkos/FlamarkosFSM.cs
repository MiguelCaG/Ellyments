using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamarkosFSM : MonoBehaviour
{
    public enum FSM1State { PATROL, SHOOT, RELOAD };
    public FSM1State fsm1State;

    private Flamarkos flamarkos;

    private void Start()
    {
        fsm1State = FSM1State.PATROL;
        flamarkos = gameObject.GetComponent<Flamarkos>();
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
                flamarkos.Patrol();
                break;

            case FSM1State.SHOOT:
                flamarkos.Shoot();
                break;

            case FSM1State.RELOAD:
                flamarkos.Reload();
                break;
        }
    }
}
