using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldNode : MonoBehaviour
{
    public int xPos, zPos;
    public float height;
    public bool accessible;

    
    public void InjectNode(int x, int z, float h) {
        xPos = x;
        zPos = z;
        height = h;
        accessible = true;
    }

    public void InjectNode(int x, int z, float h, bool acc) {
        xPos = x;
        zPos = z;
        height = h;
        accessible = acc;
    }

    public void InitializeNode() {
        transform.name = "(" + xPos + " , " + zPos + ")";
    }

}
