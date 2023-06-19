using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [System.NonSerialized]
    public MapManager MM;
    [System.NonSerialized]
    public Pathfinding PATH;

    public LayerMask ObstructionMask;
    [ReadOnly]
    public Vector2 gridWorldSize;
    [Min(1)]
    public int navSubdivisions = 1;
    [ReadOnly]
    public float nodeRadius;
    public float Distance;

    public NavNode[,] grid;
    public bool drawDebug;


    float nodeDiamater;
    int gridSizeX, gridSizeY;

    public void Initialize() {
        MM = FindObjectOfType<MapManager>();
        PATH = FindObjectOfType<Pathfinding>();
        gridWorldSize.x = MM.meshGen.density * MM.meshGen.xSize;
        gridWorldSize.y = MM.meshGen.density * MM.meshGen.zSize;
        
        nodeRadius = MM.meshGen.density/(2* navSubdivisions);
        nodeDiamater = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiamater);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiamater);
        print("Creating NavGrid of size " + gridSizeX + " by " + gridSizeY);
        CreateGrid();
    }

    public int MaxSize {
        get {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid() {
        grid = new NavNode[gridSizeX, gridSizeY];
        if(MM.nodes != null) {

            WorldNode blNode = MM.nodes[0, 0];
            Vector3 blNodePos = blNode.transform.position - transform.right * blNode.transform.localScale.x/2 -transform.forward*blNode.transform.localScale.z/2;

            Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
            for (int x = 0; x < gridSizeX; x++) {
                for (int y = 0; y < gridSizeY; y++) {
                    Vector3 worldPoint = blNodePos + Vector3.right * (x * nodeDiamater + nodeRadius) + Vector3.forward * (y * nodeDiamater + nodeRadius);

                    WorldNode worldNode = MM.WorldNodeFromWorldPos(worldPoint);

                    if(worldNode == null) {
                        print("World Reference Null");
                        return;
                    }

                    worldPoint.y = (worldNode.height*MM.meshGen.maxHeight);
                    bool isBlocked = false;

                    if (Physics.CheckSphere(worldPoint, nodeRadius, ObstructionMask) || !worldNode.accessible) {
                        isBlocked = true;
                    }

                    grid[x, y] = new NavNode(isBlocked, worldPoint, x, y);
                    grid[x, y].worldNode = worldNode;

                }
            }
        }
        else {
            print("MapManager Nodes are null");
        }
       
    }

    public void UpdateNavGrid() {
        if (MM.nodes != null) {

            WorldNode blNode = MM.nodes[0, 0];
            Vector3 blNodePos = blNode.transform.position - transform.right * blNode.transform.localScale.x / 2 - transform.forward * blNode.transform.localScale.z / 2;

            Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
            for (int x = 0; x < gridSizeX; x++) {
                for (int y = 0; y < gridSizeY; y++) {
                    Vector3 worldPoint = blNodePos + Vector3.right * (x * nodeDiamater + nodeRadius) + Vector3.forward * (y * nodeDiamater + nodeRadius);

                    WorldNode worldNode = MM.WorldNodeFromWorldPos(worldPoint);

                    if (worldNode == null) {
                        print("World Reference Null");
                        return;
                    }

                    worldPoint.y = (worldNode.height * MM.meshGen.maxHeight);
                    bool isBlocked = false;

                    if (Physics.CheckSphere(worldPoint, nodeRadius, ObstructionMask) || !worldNode.accessible) {
                        isBlocked = true;
                    }

                    grid[x, y].IsObstructed = isBlocked;

                }
            }
        }
    }

    
    public List<NavNode> getNavNodesInWorldNode(WorldNode wNode) {
        int startX = wNode.xPos * navSubdivisions;
        int endX = startX + navSubdivisions;
        int startZ = wNode.zPos * navSubdivisions;
        int endZ = startZ + navSubdivisions;

        List<NavNode> temp = new List<NavNode>();
        for (int x = startX; x < endX; x++) {
            for (int z = startZ; x < endZ; x++) {
                if(grid[x,z] != null) {
                    temp.Add(grid[x, z]);
                }
                else {
                    print("Error, nodeNull @: " + x + "," + z);
                }
            }
        }


        if (temp.Count > 0)
            return temp;
        else
            return null;
    }


    public List<NavNode> getNeighboringNodes(NavNode a_NeighborNode) {
        List<NavNode> NeighborList = new List<NavNode>();//Make a new list of all available neighbors.

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                //if we are on the node tha was passed in, skip this iteration.
                if (x == 0 && y == 0) {
                    continue;
                }

                int checkX = a_NeighborNode.gridX + x;
                int checkY = a_NeighborNode.gridY + y;

                //Make sure the node is within the grid.
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    
                    NeighborList.Add(grid[checkX, checkY]); //Adds to the neighbours list.
                }

            }
        }

        return NeighborList;//Return the neighbors list.
    }

    //Gets the closest node to the given world position.
    public NavNode NavNodeFromWorldPos(Vector3 worldPos) {
        
        Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        float percentX = (worldPos.x+(MM.meshGen.density/2))/ gridWorldSize.x;
        float percentY = (worldPos.z + (MM.meshGen.density / 2)) / gridWorldSize.y;
        
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY-1) * percentY);

        return grid[x, y];
    }

    private void OnDrawGizmos() {
       
        if(grid != null && drawDebug) {
            foreach(NavNode node in grid) {
                if (node == null)
                    return;
                if (node.IsObstructed) {
                    Gizmos.color = Color.red;

                }else if (node.occupant != null) {
                    Gizmos.color = Color.magenta;

                }
                else {
                    Gizmos.color = Color.white;
                    
                }

                float off = .45f;
 
                

                Gizmos.DrawCube(node.worldPos, (Vector3.one * (nodeDiamater - Distance))* off);
            }
        }
    }



}
