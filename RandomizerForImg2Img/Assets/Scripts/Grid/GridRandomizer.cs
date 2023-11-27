using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRandomizer : MonoBehaviour
{
    public int xSize = 5;
    public int ySize = 5;
    public int zSize = 5;

    public GameObject[] prefabs;
    public bool mirrorGrid = true;
 
    Grid grid;
    GameObject rootObject;

    public float occupationThreshold = 0.6f;

    public Vector2 randomWalksMinMax = new Vector2(2,5);
    public Vector2 randomWalksLenhthMinMax = new Vector2(5, 10);

    public bool autoGenerate = false;

    public int numColors = 4;
    public bool randomizeBackground = false;

    Color[] colors;
    int changeColors = 0;

    List<Material> materials = new List<Material>();
    Material gradientMaterial;
    public MeshRenderer gradientQuad;

    public int framerate = 5;

    void Start()
    {
        colors = new Color[numColors];
        materials.Add(gradientQuad.sharedMaterial);

        RandomizeColors();

        Application.targetFrameRate = framerate;

        CreateGrid();
    }

    private void Update()
    {
        if (!autoGenerate) return;

        changeColors++;
        if (changeColors >= 5) {
            RandomizeColors();
            changeColors = 0;
        }

        if (Random.Range(0, 1f) > 0.5f) {
            CreateGrid();
        }
        else {
            CreateRandomWalkGrid();
        }

        ColorGrid();

        FrameCapture f = GetComponent<FrameCapture>();
        if (f) f.SaveFrame();
    }
    void RandomizeColors()
    {
        float baseHue = Random.Range(0f, 1f);

        // Background
        if (randomizeBackground) {
            float hue = Random.Range(0f, 1f);
            for (int i = 0; i < materials.Count; i++) {
                Color topColor = Color.HSVToRGB(hue + Random.Range(-.33f, .33f), Random.Range(.15f, .4f), Random.Range(.1f, .5f));
                Color bottomColor = Color.HSVToRGB(hue + Random.Range(-.33f, .33f), Random.Range(.15f, .4f), Random.Range(.1f, .5f));
                materials[i].SetColor("_TopColor", topColor);
                materials[i].SetColor("_BottomColor", bottomColor);
            }
        }

        // Objects
        colors = new Color[numColors];
        for (int i = 0; i < numColors; i++) {
            colors[i] = Color.HSVToRGB(baseHue + Random.Range(-.2f,.2f), Random.Range(.2f, .45f), Random.Range(.4f, .8f));
        }
    }

    public void CreateGrid()
    {
        if (prefabs == null) return;
        grid = new Grid(xSize, ySize, zSize);
        grid.RandomizeGrid(occupationThreshold, mirrorGrid);
        RandomizeColors();
        VisualizeGrid();
        CenterShape(rootObject.transform);
    }

    public void CreateRandomWalkGrid()
    {
        if (prefabs == null) return;
        grid = new Grid(xSize, ySize, zSize);
        grid.RandomWalksGrid((int)randomWalksMinMax.x, (int)randomWalksMinMax.y, (int)randomWalksLenhthMinMax.x, (int)randomWalksLenhthMinMax.y, mirrorGrid);
        RandomizeColors();
        VisualizeGrid();
        CenterShape(rootObject.transform);
    }

    void VisualizeGrid()
    {
        if (rootObject) GameObject.Destroy(rootObject);

        rootObject = new GameObject("Root");
        rootObject.transform.position = Vector3.zero;

        grid.SetTypes();

        for (int z = 0; z < grid.zSize; z++) {
            for (int y = 0; y < grid.ySize; y++) {
                for (int x = 0; x < grid.xSize; x++) {
                    if (grid.IsOccupied(x, y, z) && grid.GetType(x, y, z) != (int)GridItemType.Floater) {
                        GameObject o = Instantiate(prefabs[grid.GetType(x,y,z)], new Vector3(x, y, z), Quaternion.identity);
                        o.name = x.ToString() + "-" + y.ToString() + "-" + z.ToString();
                        o.transform.parent = rootObject.transform;
                        grid.SetTransform(x, y, z, o.transform, grid.GetOrientation(x,y,z));
                    }
                }
            }
        }

        ColorGrid();
    }

    void ColorGrid()
    {
        float yStep = numColors / (float) ySize;
        float zStep = numColors / (float) zSize;

        if (Random.Range(0, 2) == 0) {
            yStep = 0;
        }
        else {
            zStep = 0;
        }

        Material[] materials = new Material[numColors];
        for (int i = 0; i < numColors; i++) {
            materials[i] = new Material(Shader.Find("Standard"));
            materials[i].color = colors[i];
            materials[i].SetFloat("_Metallic", 0.0f);
            materials[i].SetFloat("_Glossiness", 0.175f);
        }

        float colorIndex = 0;

        for (int z = 0; z < grid.zSize; z++) {
            for (int y = 0; y < grid.ySize; y++) {
                for (int x = 0; x < grid.xSize; x++) {
                    if (grid.IsOccupied(x, y, z)) {
                        Transform t = grid.GetTransform(x, y, z);
                        if (t) {
                            MeshRenderer mr = t.GetComponent<MeshRenderer>();
                            colorIndex = yStep * y + zStep * z;
                            if (mr) mr.material = materials[(int)colorIndex];
                        }
                    }
                }
            }
        }
    }

    void CenterShape(Transform root)
    {
        if (root.childCount == 0) return;

        Bounds totalBounds = root.GetChild(0).GetComponent<MeshRenderer>().bounds;

        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true)) {
            totalBounds.Encapsulate(renderer.bounds.center);
        }

        root.transform.position = -totalBounds.center;
    }
}
