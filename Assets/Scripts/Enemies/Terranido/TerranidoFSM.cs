using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerranidoFSM : MonoBehaviour
{
    public enum FSM1State { SLEEP, EXPAND, CHASE, ATTACK };
    public FSM1State fsm1State;

    private Terranido terranido;

    private void Start()
    {
        fsm1State = FSM1State.SLEEP;
        terranido = gameObject.GetComponent<Terranido>();
    }

    private void FixedUpdate()
    {
        FSMLevel1();
    }


    private void FSMLevel1()
    {
        switch (fsm1State)
        {
            case FSM1State.SLEEP:
                terranido.Sleep();
                break;

            case FSM1State.EXPAND:
                terranido.Expand();
                break;

            case FSM1State.CHASE:
                terranido.Chase();
                break;

            case FSM1State.ATTACK:
                terranido.Attack();
                break;
        }
    }
}
