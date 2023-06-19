using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public WorldNode[,] nodes;
    Grid GRID;
    public MeshGenerator meshGen;
    public bool printMapLogs;
    public GameObject prefab;

    // Start is called before the first frame update
    private void Start() {
        GRID = GetComponent<Grid>();
        meshGen = FindObjectOfType<MeshGenerator>();
        RandomObjectGen(prefab, 250);
    }

    public void InitializeMap(WorldNode[,] nodeMap)
    {
        if(printMapLogs)
            print("Starting Initializing a " + nodeMap.GetLength(0) + " by " + nodeMap.GetLength(1) + " map.");

        meshGen = FindObjectOfType<MeshGenerator>();

        if (meshGen == null)
            return;

        nodes = nodeMap;
        if (nodes.Length < 1)
            return;
        foreach(WorldNode n in nodes) {
            if(n != null)
            n.InitializeNode();
        }
        if (printMapLogs)
            print("Finished initializing map with " + nodes.Length + " nodes.");
        
        meshGen.PrepareForGame();
        GetComponent<Grid>().Initialize();
    }


    public void RandomObjectGen(GameObject objectToGen) {
        int mapX = nodes.GetLength(0);
        int mapY = nodes.GetLength(1);
        int randX = Random.Range(0, mapX);
        int randY = Random.Range(0, mapY);

        WorldNode node = nodes[randX, randY];

        Vector3 pos = node.transform.position;
        pos.y += node.transform.localScale.y/2 + objectToGen.transform.localScale.y/2;

        if (node.accessible)
            Instantiate(objectToGen, pos, Quaternion.identity);
        else
            RandomObjectGen(objectToGen);
    }
    public void RandomObjectGen(GameObject objectToGen, int amt) {
        int count = 0;
        while(count < amt) {
            int mapX = nodes.GetLength(0);
            int mapY = nodes.GetLength(1);
            int randX = Random.Range(0, mapX);
            int randY = Random.Range(0, mapY);

            WorldNode node = nodes[randX, randY];

            Vector3 pos = node.transform.position;
            pos.y += node.transform.localScale.y / 2 + objectToGen.transform.localScale.y / 2;
            if (node.accessible) {
                Instantiate(objectToGen, pos, Quaternion.identity);
                GRID.getNavNodesInWorldNode(node);
                count++;
            }
        }
        

        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public WorldNode getNodeFromCoord(int x, int z) {
        foreach(WorldNode n in nodes) {
            if(n.xPos == x && n.zPos == z) {
                return n;
            }
        }
        return null;
    }

    public WorldNode WorldNodeFromWorldPos(Vector3 worldPos) {
        worldPos = new Vector3(Mathf.Abs(worldPos.x), Mathf.Abs(worldPos.y), Mathf.Abs(worldPos.z));
        float _x = worldPos.x / (meshGen.density);
        float _z = worldPos.z / (meshGen.density);


        if (nodes[(int)_x, (int)_z] != null)
            return nodes[Mathf.RoundToInt(_x), Mathf.RoundToInt(_z)];
        else return null;
    }
}
