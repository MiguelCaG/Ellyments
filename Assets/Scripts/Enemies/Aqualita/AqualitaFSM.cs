using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AqualitaFSM : MonoBehaviour
{
    public enum FSM1State { PATROL, CHASE, ATTACK, HEAL };
    public FSM1State fsm1State;

    private Aqualita aqualita;

    private void Start()
    {
        fsm1State = FSM1State.PATROL;
        aqualita = gameObject.GetComponent<Aqualita>();
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
                aqualita.Patrol();
                break;

            case FSM1State.CHASE:
                aqualita.Chase();
                break;

            case FSM1State.ATTACK:
                aqualita.Attack();
                break;

            case FSM1State.HEAL:
                aqualita.Heal();
                break;
        }
    }
}
