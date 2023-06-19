using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AIAction", menuName = "AI/Action", order = 0)]
public class AIAction: ScriptableObject{
    public string actionName;
    public FSMState stateToExecute;
    public AIActionType actionType;
    [System.NonSerialized]
    public BasicAI controllerRef;


    public AIAction() {
        
    }
    public AIAction(string name, FSMState executeState, AIActionType type) {
        actionName = name;
        stateToExecute = executeState;
        actionType = type;
    }

    public void SetController(BasicAI controller) {
        controllerRef = controller;
    }

    public void TriggerAction() {

        Debug.Log("Trigger Attemp");
        if (stateToExecute != null && controllerRef != null) {
            Debug.Log(controllerRef.name + " is executing " + stateToExecute.name);
            Debug.Log(controllerRef.finiteStateMachine);
            controllerRef.finiteStateMachine.EnterState(stateToExecute, controllerRef);
        }
        else {
            Debug.Log("Error");
        }
    }
}

public enum AIActionType
{
    POSITION,
    TARGET,
    SELF,
    NONE
};

