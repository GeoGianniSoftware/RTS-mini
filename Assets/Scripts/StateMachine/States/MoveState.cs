using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveState", menuName = "AI/FSM/States/Move", order = 2)]
public class MoveState : FSMState
{

    public Vector3 movePoint;

    public override void OnEnable() {
        base.OnEnable();
        StateType = FSMStateType.MOVE;
    }

    public override bool EnterState(Entity entity) {
        EnteredState = false;
        if (base.EnterState(entity)) {
            

            //Grab move point and set it as the destination
            if (entity.moveDestinations.Count == 0) {
                Debug.Log("Failed To Fetch Move Point");
                return false;
            }
            else {

                movePoint = entity.moveDestinations[0];
                SetDestination(movePoint);
                EnteredState = true;

            }


        }
        return EnteredState;

    }
    public override void UpdateState(Entity entity) {
        if (EnteredState) {
            if (Vector3.Distance(entity.transform.position, movePoint) <= 3f) {
                if (entity.moveDestinations.Count == 0) {

                    return;
                }

                entity.moveDestinations.RemoveAt(0);
                if(entity.moveDestinations.Count > 0) {
                    Debug.Log(_FSM.controller.name);
                    _FSM.EnterState(FSMStateType.MOVE, entity);
                }
                else {
                    _FSM.EnterState(FSMStateType.IDLE, entity);
                }
                
            }
            else {
            }
        }
    }

    private void SetDestination(Vector3 destination) {
        if (_executingEntity != null)
            _executingEntity.FindPath(destination);
    }



}

