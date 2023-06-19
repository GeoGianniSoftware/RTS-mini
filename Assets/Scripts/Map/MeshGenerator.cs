using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]

public class MeshGenerator : MonoBehaviour
{

    //Class References
    MapManager MM;

    [Header("Color & Material")]
    public MapDisplayMode displayMode = MapDisplayMode.Render;

    public Gradient colorGrade;
    public Material materialPreset;

    [Space]
    [Space]

    GameObject[] cubes;
    GameObject ocean;
    GameObject preview;


    [Header("Scale & Position")]
    public int xSize = 20;
    public int zSize = 20;
    [Range(0.1f,25)]
    public float density = 0.1f;
    [ReadOnly]
    Vector3 globalPos;
   

    
    [Header("Seed")]
    public int seed;
    public bool randomSeed;
    [ReadOnly]
    public int randomSeedInt;

    

    [Header("Generation")]
    [Space]
    [Space]
    
    
    public bool blocky;
    [Space]
    public float terrainSteps = 5f;
    [Min(0)]
    public float maxHeight = 5f;
    [Space]
    public bool shelves;
    [Range(0.01f,1f)]
    public float shelfHeight = 0.1f;
    
    public float oceanHeight;
    [Min(0)]
    public float oceanDistance;
    [Space]
    [Range(0.0018f, 1.25f)]
    public float noiseSize = .3f;

    [ReadOnly]
    public int vertCount;
    

    [Header("Prefabs")]

    [Space]
    [Space]
    public GameObject cubePrefab;
    public GameObject oceanPrefab;

    [Header("Preview")]

    [Space]
    [Space]
    public bool previewEnabled;
    public MapDisplayMode previewMode;
  
    public Vector3 previewOffset;
    public float previewScale;

    public GameObject previewPrefab;
    Renderer previewRender;


    [Header("EDITOR")]

    [Space]
    [Space]
    public bool autoUpdate;
    private void Start() {
        MM = GetComponent<MapManager>();
        BuildTerrain(true);
    }

    bool ErrorCheck() {
        if(xSize + zSize <= 2 || !cubePrefab || !oceanPrefab || !previewPrefab || oceanDistance < 0|| maxHeight < 0 || !materialPreset){
            print("Variables incorrectly set!");
            return false;
        }

        return true;
    }

    // Start is called before the first frame update
    public void BuildTerrain(bool buildForGame)
    {
        if (ErrorCheck()) {
            globalPos = new Vector3((xSize * density / 2) - (density / 2), 0, (zSize * density / 2) - (density / 2));

            UpdateMesh(buildForGame);

            
        }
    }

    public void ShuffleSeed() {
        if (randomSeed)
            randomSeedInt = Random.Range(1, 1000);
        BuildTerrain(false);
    }



    float getNoiseAt(int x, int z) {
        int _seed = seed;
        if (randomSeed)
            _seed = randomSeedInt;
        

        float y = 0;
        
         y += (Mathf.PerlinNoise((x * noiseSize)+_seed , (z * noiseSize) + _seed));
      
        return y;
        
    }
    

    void UpdateMesh(bool buildForGame) {
        if (cubes != null && cubes.Length != 0) {
            foreach (GameObject g in cubes) {
                DestroyImmediate(g);
            }
        }
        else {
            if(transform.childCount > 0) {
                foreach(Transform t in transform.GetComponentsInChildren<Transform>()) {
                    if(t != this.transform && t != null)
                    DestroyImmediate(t.gameObject);
                }
            }
        }
        if(ocean != null) {
            DestroyImmediate(ocean);
        }
        
        cubes = new GameObject[xSize * zSize];

        Vector3 oceanPos = new Vector3(globalPos.x, 0, globalPos.z);

        ocean = Instantiate(oceanPrefab, oceanPos, Quaternion.identity);
        ocean.transform.Rotate(transform.right, 90);
        ocean.transform.localScale = new Vector3((xSize * density *.1f)+oceanDistance*2 , (zSize * density * .1f) + oceanDistance * 2, 1);
        ocean.transform.SetParent(transform, true);

        UpdatePreview();




        //Node Creation
        WorldNode[,] nodesTemp = new WorldNode[xSize, zSize];

        for (int i = 0, z = 0; z < zSize; z++) {
            for (int x = 0; x < xSize; x++) {
                GameObject cube = Instantiate(cubePrefab, new Vector3(x*density, -50, z*density), Quaternion.identity);
                WorldNode n = cube.GetComponent<WorldNode>();
                
                //Data Calculation
                float noise = getNoiseAt(x, z);
                float calculation = 0;
                if (shelves) 
                    if(noise < .85)
                        noise-=shelfHeight;
                    
                calculation = noise * terrainSteps - oceanHeight;
                float height = calculation;
                float heightPercentage = height / (terrainSteps - oceanHeight);

                bool empty = false;

                

                
                if (blocky)
                    height = Mathf.RoundToInt(height);
                if(height <= 1/terrainSteps) {
                    DestroyImmediate(cube.GetComponent<MeshFilter>());
                    DestroyImmediate(cube.GetComponent<MeshRenderer>());
                    empty = true;
                    
                }

                float finHeight = height * maxHeight;
                if(finHeight <= 0) {
                    finHeight = 0.1f;
                }

                cube.transform.localScale = new Vector3(1 * density, finHeight, 1 * density);

                //Map Color to blocks
                if(displayMode == MapDisplayMode.Render && !empty) {
                    cube.GetComponent<MeshRenderer>().sharedMaterial = new Material(materialPreset);
                    cube.GetComponent<MeshRenderer>().sharedMaterial.color = colorGrade.Evaluate(heightPercentage);
                }else if(displayMode == MapDisplayMode.NoiseMap && !empty) {
                    cube.GetComponent<MeshRenderer>().sharedMaterial = new Material(materialPreset);
                    cube.GetComponent<MeshRenderer>().sharedMaterial.color = Color.Lerp(Color.black, Color.white, getNoiseAt(x,z));
                }
                


                cubes[i] = cube;
                Transform parentTrans;

                if(transform.Find("Nodes") == null) {
                    GameObject parent = new GameObject("Nodes");
                    parent.transform.SetParent(transform);
                    parent.transform.position = Vector3.zero;
                    parentTrans = parent.transform;
                }
                else {
                    parentTrans = transform.Find("Nodes");
                }
                cube.transform.SetParent(parentTrans, true);
                cube.transform.position = new Vector3(x*density, (cube.transform.localScale.y / 2)+transform.position.y, z*density);
                i++;

                
                nodesTemp[x, z] = n;
                nodesTemp[x, z].InjectNode(x, z, height, !empty);
            }
        }
        //Only Run if Game Is Running
        if (buildForGame && nodesTemp.Length > 0) {
            MM.InitializeMap(nodesTemp);
        }
        else if(!autoUpdate) {
        }
        vertCount = cubes.Length * 8;
    }

    public void UpdatePreview() {
        if (preview != null) {
            DestroyImmediate(preview);
        }

        if (!previewEnabled) {
            return;
        }

        Vector3 Sizeref = new Vector3(previewOffset.x* density * xSize, 1, previewOffset.z * zSize * density);

        preview = Instantiate(previewPrefab, globalPos + (Sizeref), Quaternion.identity);
        preview.transform.Rotate(transform.right, 90);
        preview.transform.localScale = new Vector3((xSize * density * .1f) + previewScale * 2, (zSize * density * .1f) + previewScale * 2, 1);
        preview.transform.SetParent(transform, true);

        Texture2D previewTexture = new Texture2D(xSize, zSize);
        previewRender = preview.GetComponent<MeshRenderer>();
        previewRender.sharedMaterial = new Material(materialPreset);



        Color[] colorMap = new Color[xSize * zSize];
        for (int x = 0; x < xSize; x++) {
            for (int z = 0; z < zSize; z++) {

                float noise = getNoiseAt(x, z);

                float calculation = 0;
                if (shelves) 
                    if (noise < .85)
                        noise -= shelfHeight;
                    calculation = (noise * terrainSteps) - oceanHeight;


                float height = calculation;
                float heightPercentage = height / (terrainSteps - oceanHeight);

                if (previewMode == MapDisplayMode.NoiseMap) {
                    colorMap[x * zSize + z] = Color.Lerp(Color.black, Color.white, getNoiseAt(x, z));
                }
                else if (previewMode == MapDisplayMode.Render) {
                    if (height < 2/terrainSteps) {
                        colorMap[x * zSize + z] = Color.blue;
                        continue;
                    }
                        
                    colorMap[x * zSize + z] = colorGrade.Evaluate(heightPercentage);

                }

            }
        }
        previewTexture.SetPixels(colorMap);
        previewTexture.Apply();
        previewRender.sharedMaterial.mainTexture = previewTexture;
    }

    public void PrepareForGame() {
        if(previewEnabled)
            preview.SetActive(false);
        this.enabled = false;
    }
    
}

public enum MapDisplayMode
{
    Render,
    NoiseMap
}
