using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Entity: MonoBehaviour {
    public float Rotation;
    //Health
    //Owner
    public bool canScaleHeights;

    [ReadOnly]
    public WorldNode currentWorldNode;
    [ReadOnly]
    public NavNode currentNavNode;

    [System.NonSerialized]
    public Grid grid;
    [System.NonSerialized]
    public List<Vector3> currentPath;
    public List<NavNode> currentPathNodes;



    public List<Vector3> moveDestinations = new List<Vector3>();
    public bool drawDebugMoveLines;

    public virtual void Awake() {

    }

    public virtual void Update() {

    }


    public virtual void Start() {

    }

    public void SetPath(List<Vector3> positions) {
        currentPath = positions;
    }

    public void ClearPath() {
        currentPath.Clear();
        currentPathNodes.Clear();
        moveDestinations.Clear();
    }

    //Pathfinding Start//

    Vector3 finalPos;
    public void FindPath(Vector3 targetPos) {
        if (grid == null) {
            print("Grid is null!");
            return;
        }

        if(targetPos.y > transform.position.y && !canScaleHeights) {
            return;
        }

        finalPos = targetPos;

        NavNode startNode = grid.NavNodeFromWorldPos(transform.position);

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
                if ((neighbour.IsObstructed || closedSet.Contains(neighbour)) || (!canScaleHeights && neighbour.height != currentNode.height)) {
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
    public void RetracePath(NavNode startNode, NavNode endNode) {

        List<NavNode> path = new List<NavNode>();
        NavNode currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        if(currentPath != null) {

            currentPath.Clear();
        }
        else {

            currentPath = new List<Vector3>();
        }

        currentPathNodes = path;
        for (int i = 0; i < path.Count; i++) {
            currentPath.Add(path[i].worldPos);
            if(i == path.Count - 1) {
                currentPath[i] = finalPos;
            }
        }

    }
    public int getManhattenDistance(NavNode a_nodeA, NavNode a_nodeB) {
        int ix = Mathf.Abs(a_nodeA.gridX - a_nodeB.gridX);//x1-x2
        int iy = Mathf.Abs(a_nodeA.gridY - a_nodeB.gridY);//y1-y2

        return ix + iy;//Return the sum
    }
    public void getNearestNodeNodes() {
        if (grid == null)
            return;
        NavNode temp;
        currentWorldNode = grid.MM.WorldNodeFromWorldPos(transform.position);
        temp = grid.NavNodeFromWorldPos(transform.position);

        if (temp.occupant != null && temp.occupant != this)
            return;

        if (currentNavNode != null)
            currentNavNode.occupant = null;

        currentNavNode = temp;

        if (currentNavNode.occupant == null) {
            currentNavNode.occupant = this.gameObject;

        }
    }
    //Pathfinding End//

}
