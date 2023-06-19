using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleState", menuName = "AI/FSM/States/Idle", order = 1)]
public class IdleState : FSMState
{
    public override void OnEnable() {
        base.OnEnable();
        StateType = FSMStateType.IDLE;
    }
    public override bool EnterState(Entity entity) {
        base.EnterState(entity);
        Debug.Log("Entered Idle");
        EnteredState = true;
        return EnteredState;
    }
    public override void UpdateState(Entity entity) {
        if(_executingEntity.moveDestinations.Count > 0) {

            _FSM.EnterState(FSMStateType.MOVE, entity);
        }
    }

    public override bool ExitState(Entity entity) {
        base.ExitState(entity);
        Debug.Log("Exiting Idle");
        return true;
    }
}
