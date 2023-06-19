using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(FiniteStateMachine))]
public class BasicAI: Entity
{
    public float speed;
    public List<AIAction> validActions;
    

    [System.NonSerialized]
    public FiniteStateMachine finiteStateMachine;



    public override void Awake() {
        
    }

    public void TriggerState(FSMState stateToExecute) {
        if (finiteStateMachine.validStates.Contains(stateToExecute)) {
            finiteStateMachine.EnterState(stateToExecute, this);
        }
        else {
            print("Entity cannot enter that state as it is not present in FSM.validStates");
        }
    }

    override public void Start() {
        grid = FindObjectOfType<Grid>();
        finiteStateMachine = this.GetComponent<FiniteStateMachine>();
        finiteStateMachine.Initialize(this);
        foreach (AIAction action in validActions) {
            action.SetController(this);

        }
    }

    public AIAction getActionFromAction(AIAction otherAction) {
        
        foreach(AIAction action in validActions) {
            if (action.stateToExecute == otherAction.stateToExecute)
                return action;
        }
        return null;
    }

    public override void Update() {
        getNearestNodeNodes();
        Move();

        
    }

    void Move() {
        
        if (currentPath != null && currentPath.Count > 0) {
            transform.position += (currentPath[0] - transform.position).normalized * speed * Time.deltaTime;

            if(Vector3.Distance(transform.position, currentPath[0]) < .5f) {
                currentPath.RemoveAt(0);
            }
        }
    }

    

    private void OnDrawGizmos() {
        if (drawDebugMoveLines && currentPath != null && currentPath.Count > 0) {
            if (GetComponent<LineRenderer>() == null)
                gameObject.AddComponent<LineRenderer>();
            Vector3[] pathPoints = new Vector3[currentPath.Count];
            for (int i = 0; i < currentPath.Count; i++) {
                pathPoints[i] = currentPath[i];
                pathPoints[i].y += 0.5f;
            }
            LineRenderer line = GetComponent<LineRenderer>();
            line.positionCount = pathPoints.Length;
            line.SetPositions(pathPoints);
        }
        else {
            if(GetComponent<LineRenderer>() != null) {
                LineRenderer line = GetComponent<LineRenderer>();
                line.positionCount = 0;
            }
            
        }
    }
}
