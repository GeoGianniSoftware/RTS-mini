using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    public List<BasicAI> basicAIs = new List<BasicAI>();
    public GameObject actionPanel;

    List<BasicAI> selectedUnits = new List<BasicAI>();

    List<Vector3> tempMovePoints;
    List<Vector3> tempTargets;

    FSMStateType nextTempActionType;

    // Start is called before the first frame update
    void Start() {
        SelectUnits(basicAIs);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SelectUnits(List<BasicAI> unitsToSelect) {
        selectedUnits.AddRange(unitsToSelect);
        int childrenCount = actionPanel.transform.childCount;


        if (childrenCount >= 1) {
            for (int i = childrenCount; i > 0; i++) {
                if(actionPanel.transform.GetChild(childrenCount - 1) != null)
                Destroy(actionPanel.transform.GetChild(childrenCount - 1));
            }
        }

        List<FSMStateType> stateList = new List<FSMStateType>();

        foreach(BasicAI unit in selectedUnits) {
            if (unit == null) {
                print("Null");
                return;
            }
            foreach (AIAction action in unit.validActions) {
                if (!stateList.Contains(action.stateToExecute.StateType)) {
                    
                    GameObject actionButton = Instantiate((GameObject)Resources.Load("UI/ActionButton"), actionPanel.transform, false);
                    actionButton.GetComponent<ActionButton>().SetAction(action.stateToExecute.StateType);
                    stateList.Add(action.stateToExecute.StateType);
                }
                
            }
        }

    }

    void SelectUnit(BasicAI unitToSelect) {
        selectedUnits.Add(unitToSelect);
        int childrenCount = actionPanel.transform.childCount;

        if (actionPanel.transform.childCount > 0){
            for (int i = childrenCount; i > 0; i++) {
                Destroy(actionPanel.transform.GetChild(childrenCount - 1));
            }
        }


        foreach(AIAction action in unitToSelect.validActions) {
            GameObject actionButton = Instantiate((GameObject)Resources.Load("UI/ActionButton"), actionPanel.transform, false);
            actionButton.GetComponent<ActionButton>().SetAction(action.stateToExecute.StateType);
        }

    }

    List<Vector3> movePoints;
    public void startActionLoop(FSMStateType actionType) {
        nextTempActionType = actionType;
        if(actionType == FSMStateType.IDLE) {
            foreach(BasicAI unit in basicAIs) {
                unit.ClearPath();
            }
        }
        StartCoroutine(checkForInput());
        
    }

    void endActionLoop(List<BasicAI> units) {

        StopCoroutine(checkForInput());
        foreach (BasicAI unit in units) {
            if (unit == null) {
                print("Null units");
                continue;
            }

            print("Attempting to ender State");
                FiniteStateMachine fsm = unit.GetComponent<FiniteStateMachine>();
                
                fsm.EnterState(nextTempActionType, unit);
        }
    }

    public List<BasicAI> getSelected() {
        return selectedUnits;
    }

    bool firstLoopRun = true;
    IEnumerator checkForInput() {
        firstLoopRun = true;
        tempMovePoints = new List<Vector3>();
        List<BasicAI> units = basicAIs;
        bool exit = false;
        while (nextTempActionType != null) {
            
            if (Input.GetMouseButtonDown(1)) {
                
                if (Input.GetKey(KeyCode.LeftControl)) {
                    tempMovePoints.Add(getWorldPosFromCursorPos());
                   

                }
                
                else{
                    if (firstLoopRun) {
                        tempMovePoints.Clear();
                        firstLoopRun = false;
                    }

                    tempMovePoints.Add(getWorldPosFromCursorPos());
                    exit = true;
                    

                }

                
            }

            if (!Input.GetKey(KeyCode.LeftControl) && tempMovePoints.Count > 0) {
                exit = true;
            }

            if (exit) {
                if(tempMovePoints.Count > 0) {
                    Vector3 offset = new Vector3(0,0,0);
                    for (int i = 0; i < units.Count; i++) {
                        if (units[i] == null) {
                            print("UNIT NULL?");
                            continue;
                        }
                        List<Vector3> finMovePoints = new List<Vector3>();
                        for (int z = 0; z < tempMovePoints.Count; z++) {
                            finMovePoints.Add((tempMovePoints[z] + offset));
                        }
                        print("Ding: " + tempMovePoints.Count);
                        units[i].moveDestinations = (finMovePoints);
                        offset.x += 2;
                        if((i+1)%3 == 0 && i != 0) {
                            offset.z += 2;
                            offset.x = 0;
                        }
                    }


                    endActionLoop(units);
                }

                yield break;
            }

            yield return null;
        }
    }

    Vector3 getWorldPosFromCursorPos() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {
            if(hit.point != null) {
                return hit.point;
            }
        }

        print("Error returning " + Vector3.one * -1);
        return Vector3.one * -1;
    }

    Entity getEntityFromCursorPos() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {
            if (hit.point != null && hit.transform.GetComponent<Entity>()) {
                return hit.transform.GetComponent<Entity>();
            }
        }

        print("Error, returning null");
        return null;
    }
}
