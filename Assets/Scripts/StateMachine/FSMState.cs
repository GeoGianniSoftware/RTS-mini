using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExecutionState
{
    NONE,
    ACTIVE,
    COMPLETED,
    TERMINATED
};

public enum FSMStateType
{
    IDLE,
    MOVE,
    PATROL
};

public abstract class FSMState: ScriptableObject
{
    protected Entity _executingEntity;
    protected FiniteStateMachine _FSM;
    public ExecutionState ExecutionState { get; protected set; }
    public FSMStateType StateType { get; protected set; }
    public bool EnteredState { get; protected set; }

    public virtual void OnEnable() {
        ExecutionState = ExecutionState.NONE;
    }

    public virtual bool EnterState(Entity entity) {
        bool success = true;
        ExecutionState = ExecutionState.ACTIVE;
        //Does the executing Entity Exist?
        _executingEntity = entity;
        success = (_executingEntity != null);

        return success;
    }

    public abstract void UpdateState(Entity entity);

    public virtual bool ExitState(Entity entity) {
        ExecutionState = ExecutionState.COMPLETED;
        return true;
    }

    public virtual void SetExecutingFSM(FiniteStateMachine fsm) {
        if(fsm != null) {
            _FSM = fsm;
        }
    }

}
