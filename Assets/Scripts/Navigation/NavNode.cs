using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NavNode: IHeapItem<NavNode>
{
    public int gridX, gridY;
    

    public bool IsObstructed;
    public GameObject occupant;

    public Vector3 worldPos;
    public WorldNode worldNode;

    public NavNode Parent;

    public float height;
    public int gCost, hCost;
    int heapIndex;

    public int FCost { get { return gCost + hCost; } }

    public NavNode(bool isObs, Vector3 a_pos, int a_gridX, int a_gridY) {
        gridX = a_gridX;
        gridY = a_gridY;

        IsObstructed = isObs;
        worldPos = a_pos;
        height = worldPos.y;
    }

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public int CompareTo(NavNode nodeToCompare) {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }

    public bool isAccessible() {

        Grid grid = GameObject.FindObjectOfType<Grid>();
        bool isBlocked = false;

        if (Physics.CheckSphere(worldPos, grid.nodeRadius, grid.ObstructionMask) || !grid.MM.getNodeFromCoord(gridX,gridY).accessible) {
            isBlocked = true;
        }
        return isBlocked;
    }


}
