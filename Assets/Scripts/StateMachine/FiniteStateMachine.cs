using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine : MonoBehaviour
{

    FSMState currentState;

    [SerializeField]
    public List<FSMState> validStates;
    Dictionary<FSMStateType, FSMState> fsmStates;
    public Entity controller;


    public void Initialize(Entity entityToInitializeStates) {
        currentState = null;
        fsmStates = new Dictionary<FSMStateType, FSMState>();

        controller = entityToInitializeStates;
        foreach (FSMState state in validStates) {
            state.SetExecutingFSM(this);
            fsmStates.Add(state.StateType, state);
        }

        EnterState(FSMStateType.IDLE, entityToInitializeStates);
    }


    public void Update() {
        if(currentState != null) {
            currentState.UpdateState(controller);
        }
    }

    #region STATE MANAGEMENT
    public void EnterState(FSMState nextState, Entity entity) {
        controller = entity;
        if(nextState == null || !validStates.Contains(nextState)) {
            return;
        }

        if(currentState != null)
            currentState.ExitState(entity);

        currentState = nextState;
        currentState.EnterState(entity);
    }


    public void EnterState(FSMStateType stateType, Entity entity) {
        if (fsmStates == null) {

            print("fsmStates is null");
            return;
        }
        if (fsmStates.ContainsKey(stateType)) {
            FSMState nextState = fsmStates[stateType];
            
            EnterState(nextState, entity);
        }
    }

    #endregion
}
