using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshGenerator))]
public class DisplayTerrainEditor : Editor
{
    public override void OnInspectorGUI() {

        MeshGenerator myScript = (MeshGenerator)target;
        if (DrawDefaultInspector()) {
            if (myScript.autoUpdate) {
                myScript.BuildTerrain(false);
            }
            else {
                myScript.UpdatePreview();
            }
        }

        

        if (GUILayout.Button("Update")) {
            myScript.BuildTerrain(false);
        }
        if (GUILayout.Button("Randomize Seed")) {
            myScript.ShuffleSeed();
        }
    }
}
