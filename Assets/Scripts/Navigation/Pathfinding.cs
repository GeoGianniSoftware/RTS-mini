using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour
{

    Grid grid;
    public NavNode nearestNode;
    public List<NavNode> currentPath;
    public bool drawDebugMoveLines;

    private void Awake() {
        grid = FindObjectOfType<Grid>();
    }

    void Update() {
        
        
    }


    void FindPath(Vector3 startPos, Vector3 targetPos, bool allowHeightDifference) {
        
        NavNode startNode = grid.NavNodeFromWorldPos(startPos);
        
        NavNode targetNode = grid.NavNodeFromWorldPos(targetPos);
        
        Heap<NavNode> openSet = new Heap<NavNode>(grid.MaxSize);
        HashSet<NavNode> closedSet = new HashSet<NavNode>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            NavNode currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode) {
                
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (NavNode neighbour in grid.getNeighboringNodes(currentNode)) {
                if ((neighbour.IsObstructed || closedSet.Contains(neighbour)) || (!allowHeightDifference && neighbour.height != currentNode.height)) {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + getManhattenDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = getManhattenDistance(neighbour, targetNode);
                    neighbour.Parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else {
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
    }

    void RetracePath(NavNode startNode, NavNode endNode) {
        
        List<NavNode> path = new List<NavNode>();
        NavNode currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();

        currentPath = path;

    }


    int getManhattenDistance(NavNode a_nodeA, NavNode a_nodeB) {
        int ix = Mathf.Abs(a_nodeA.gridX - a_nodeB.gridX);//x1-x2
        int iy = Mathf.Abs(a_nodeA.gridY - a_nodeB.gridY);//y1-y2

        return ix + iy;//Return the sum
    }


    void getNearestNode() {
        NavNode temp;
        temp = grid.NavNodeFromWorldPos(transform.position);

        if (temp.occupant != null && temp.occupant != this)
            return;

        if (nearestNode != null)
            nearestNode.occupant = null;

        nearestNode = temp;

        if (nearestNode.occupant == null) {
            nearestNode.occupant = this.gameObject;

        }
    }


    private void OnDrawGizmos() {
        if (drawDebugMoveLines && currentPath != null && currentPath.Count > 0) {
            if (GetComponent<LineRenderer>() == null)
                gameObject.AddComponent<LineRenderer>();
            Vector3[] pathPoints = new Vector3[currentPath.Count];
            for (int i = 0; i < currentPath.Count; i++) {
                pathPoints[i] = currentPath[i].worldPos;
            }
            LineRenderer line = GetComponent<LineRenderer>();
            line.positionCount = pathPoints.Length;
            line.SetPositions(pathPoints);
        }
    }
}
