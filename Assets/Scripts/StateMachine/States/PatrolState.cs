using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolState", menuName = "AI/FSM/States/Patrol", order = 3)]
public class PatrolState : FSMState
{
    public Vector3[] patrolPoints;
    int patrolPointIndex;

    public override void OnEnable() {
        base.OnEnable();
        StateType = FSMStateType.PATROL;
        patrolPointIndex = -1;
    }

    public override bool EnterState(Entity entity) {
        Debug.Log("Entering Patrol");
        EnteredState = false;
        
        
        if (base.EnterState(entity)) {
                //Grab and store Patrol Points;
                if (_executingEntity.moveDestinations.Count == 1) {
                _executingEntity.moveDestinations.Add(_executingEntity.transform.position);
            }

            patrolPoints = _executingEntity.moveDestinations.ToArray();
            
            if (patrolPoints == null || patrolPoints.Length == 0) {
                Debug.Log("PatrolState: Failed To Fetch Patrol Points");
            }
            else {
                
                patrolPointIndex++;
                if (patrolPointIndex > patrolPoints.Length - 1)
                    patrolPointIndex = 0;
                
                SetDestination(patrolPoints[patrolPointIndex]);
                EnteredState = true;
                
            }


        }
        return EnteredState;

    }
    public override void UpdateState(Entity entity) {
        if (EnteredState) {
            if(Vector3.Distance(_executingEntity.transform.position, patrolPoints[patrolPointIndex]) <= 3f) {
                _FSM.EnterState(FSMStateType.PATROL, entity);
            }
            else {

            }
        }
    }

    private void SetDestination(Vector3 destination) {
        if(_executingEntity != null)
            _executingEntity.FindPath(destination);
    }
    


}
