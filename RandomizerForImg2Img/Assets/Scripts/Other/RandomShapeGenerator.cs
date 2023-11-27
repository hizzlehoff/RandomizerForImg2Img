using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomShapeGenerator : MonoBehaviour
{
    public Vector2 scaleMinMax = new Vector2(0.5f, 10f);
    public float reduceScale = 0.8f;
    public int framerate = 1;

    int count = 0;
    public int countMax = 100;

    GameObject p;

    public int numColors = 4;
    Color[] colors;

    int changeColors = 0;
    
    List<Material> materials = new List<Material>();
    Material gradientMaterial;
    public MeshRenderer gradientQuad;

    void Start()
    {
        colors = new Color[numColors];
        materials.Add(gradientQuad.sharedMaterial);
        RandomizeColors();
        Application.targetFrameRate = framerate;
    }

    private void Update()
    {
        changeColors++;
        if (changeColors >= 5)
        {
            RandomizeColors();
            changeColors = 0;
        }

        if (!p)
        {
            RandomShape();
            FrameCapture f = GetComponent<FrameCapture>();
            if (f) f.SaveFrame();
        }
        else
        {
            Destroy(p);
        }
    }

    void RandomizeColors()
    {
        // Background
        for (int i = 0; i < materials.Count; i++)
        {
            // Generate random colors for the top and bottom
            //Color topColor = Random.ColorHSV();
            Color topColor = new Color(Random.Range(0, 0.2f), Random.Range(0, 0.3f), Random.Range(0, 0.6f));
            //Color bottomColor = Random.ColorHSV();

            // Assign the colors to the material properties
            materials[i].SetColor("_TopColor", topColor * 0.2f);
            materials[i].SetColor("_BottomColor", topColor);
        }

        // Objects
        for (int i = 0; i < numColors; i++)
        {
            colors[i] = Random.ColorHSV();
        }
    }


    void RandomShape()
    {
        Vector2 mm = scaleMinMax;

        p = GenerateRandomCube(mm);

        for (int i=0; i < Random.Range(countMax / 2, countMax);i++)
        {
            SpawnAndCollide(p.transform);
            count++;
            mm *= reduceScale;
        }

        p.transform.position = Vector3.zero;
        Vector3 center = CalculateBoundingCenter(p.transform);
        p.transform.position = -center;
   }

    void SpawnAndCollide(Transform p)
    {
        Vector3 randomFaceNormal = GetFaceNormal();
        Vector3 spawnPosition = CalculateBoundingBox(p.transform);

        Debug.DrawLine(spawnPosition + (randomFaceNormal * 50), spawnPosition, Color.red, 90);

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(spawnPosition + (randomFaceNormal * 50), -randomFaceNormal, out hit, 100)) {
            GameObject child = GenerateRandomCube(scaleMinMax);
            child.name = "CubeChild_" + count.ToString();
            Vector3 offset = child.transform.localScale;

            Debug.Log(hit.transform.name +"<-"+ child.name);

            offset.x *= randomFaceNormal.x * 0.5f;
            offset.y *= randomFaceNormal.y * 0.5f;
            offset.z *= randomFaceNormal.z * 0.5f;

            child.transform.position = hit.point + offset;
            child.transform.parent = p;
        }
    }

    GameObject GenerateRandomCube(Vector2 mm)
    {
        float xM = Random.Range(1f, 2f);
        float yM = Random.Range(1f, 2f);
        float zM = Random.Range(1f, 2f);

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = new Vector3(Random.Range(mm.x, mm.y*xM), Random.Range(mm.x, mm.y * yM), Random.Range(mm.x, mm.y * zM));
        Destroy(cube.GetComponent<BoxCollider>());
        cube.AddComponent<MeshCollider>();


        Material material = new Material(Shader.Find("Standard"));

        material.color = colors[Random.Range(0,numColors)];  // Use modulo to cycle through the three colors

        material.SetFloat("_Metallic", 0.0f);
        material.SetFloat("_Glossiness", 0.175f);

        cube.GetComponent<Renderer>().material = material;


        //cube.GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);
        return cube;
    }


    Transform CreateRandomCube(Vector3 minMax, Vector3 position, Vector3 normal)
    {
        GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube);

        o.transform.localScale = new Vector3(Random.Range(minMax.x, minMax.y), Random.Range(minMax.x, minMax.y), Random.Range(minMax.x, minMax.y));
        
        if (position.magnitude != 0) {
            o.transform.position = position;
        }

        return o.transform;
    }

    void AlignWithParentFace(Transform child, Transform parent, Vector3 faceNormal)
    {
        Vector3 parentExtents = parent.localScale / 2f;
        Vector3 childExtents = child.localScale / 2f;

        Vector3 offset = Vector3.Scale(faceNormal, parentExtents + childExtents);
        child.position += offset;
    }

    Vector3 GetFaceNormal()
    {
        switch (Random.Range(0, 7)) {
            case 0:
                return Vector3.up;
            case 1:
                return Vector3.down;
            case 2:
                return Vector3.forward;
            case 3:
                return Vector3.forward;
            case 4:
                return Vector3.back;
            case 5:
                return Vector3.back;
            case 6:
                return Vector3.back;
            default:
                return Vector3.back;
        }
    }

    Vector3 GetRandomPositionOnFace(Transform cubeTransform, Vector3 faceNormal)
    {
        Vector3 extents = cubeTransform.localScale / 2f;
        Vector3 randomPosition = Vector3.Scale(Random.onUnitSphere, extents);
        randomPosition.Scale(faceNormal);
        randomPosition += cubeTransform.position;
        return randomPosition;
    }

    Vector3 CalculateBoundingBox(Transform root)
    {
        Bounds b = root.GetComponent<MeshRenderer>().bounds;
        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true)) {
            b.Encapsulate(renderer.bounds);
        }

        return new Vector3(0, Random.Range(b.min.y, b.max.y), Random.Range(b.min.z, b.max.z));
    }

    Vector3 CalculateBoundingCenter(Transform root)
    {
        Bounds b = root.GetComponent<MeshRenderer>().bounds;
        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            b.Encapsulate(renderer.bounds);
        }

        return b.center;
    }
}
