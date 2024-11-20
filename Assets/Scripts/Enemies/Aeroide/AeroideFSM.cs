using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AeroideFSM : MonoBehaviour
{
    public enum FSM1State { PATROL, CHASE, ATTACK, ESCAPE };
    public FSM1State fsm1State;

    private Aeroide aeroide;

    private void Start()
    {
        fsm1State = FSM1State.PATROL;
        aeroide = gameObject.GetComponent<Aeroide>();
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
                aeroide.Patrol();
                break;

            case FSM1State.CHASE:
                aeroide.Chase();
                break;

            case FSM1State.ATTACK:
                aeroide.Attack();
                break;

            case FSM1State.ESCAPE:
                aeroide.Escape();
                break;
        }
    }
}
