using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomizer : MonoBehaviour
{
    public int framerate = 2;
    public MeshRenderer gradientQuad;

    public int numberOfShapes = 5;
    public float minSize = 1f;
    public float maxSize = 3f;
    public Vector3 boundingBoxSize = new Vector3(10f, 5f, 10f);
    public float yOffset = -2f;
    public float yOffsetLights = 3f;

    List<Material> materials = new List<Material>();
    Material gradientMaterial;

    public Transform[] lightTransforms;

    public float minLightRadius;
    public float maxLightRadius;
    public float minLightIntensity;
    public float maxLightIntensity;

    public int numColors = 4;
    Color[] colors;

    int changeColors = 0;

    void Start()
    {
        colors = new Color[numColors];

        Application.targetFrameRate = framerate;
        materials.Add(gradientQuad.sharedMaterial);
        RandomizeColors();
    }

    private void Update()
    {
        changeColors++;
        if (changeColors >= 5) {
            RandomizeColors();
            changeColors = 0;
        }

        RemoveShapes();
        SpawnShapes();
        ChangeLigths();
    }

    void RandomizeColors()
    {
        // Background
        for (int i = 0; i < materials.Count; i++) {
            // Generate random colors for the top and bottom
            Color topColor = Random.ColorHSV();
            Color bottomColor = Random.ColorHSV();

            // Assign the colors to the material properties
            materials[i].SetColor("_TopColor", topColor);
            materials[i].SetColor("_BottomColor", bottomColor);
        }

        // Objects
        for (int i = 0; i < numColors; i++) {
            colors[i] = Random.ColorHSV();
        }
    }

    void SpawnShapes()
    {

        for (int i = 0; i < numberOfShapes; i++)
        {
            GameObject shape;

            switch (Random.Range(0, 4))
            {
                case 0:
                    shape = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    break;
                case 1:
                    shape = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    break;
                case 2:
                    shape = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    break;
                case 3:
                    shape = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    break;
                default:
                    shape = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    break;
            }

            shape.transform.SetParent(transform);

            // Randomize rotation
            shape.transform.rotation = Random.rotation;

            // Randomize position within bounding box
            float x = Random.Range(-boundingBoxSize.x / 2f, boundingBoxSize.x / 2f);
            float y = Random.Range(-boundingBoxSize.y / 2f, boundingBoxSize.y / 2f) + yOffset;
            float z = Random.Range(-boundingBoxSize.z / 2f, boundingBoxSize.z / 2f);
            shape.transform.localPosition = new Vector3(x, y, z);

            // Assign one of the three random colors
            Material material = new Material(Shader.Find("Standard"));

            float index = (((shape.transform.localPosition.y - yOffset) + boundingBoxSize.y/2) / boundingBoxSize.y) * numColors;

            // Randomize size
            if (index > 2) {
                float randomSizeX = Random.Range(minSize + 0.25f, maxSize + 0.25f);
                float randomSizeY = Random.Range(minSize + 0.25f, maxSize + 0.25f);
                float randomSizeZ = Random.Range(minSize + 0.25f, maxSize + 0.25f);
                shape.transform.localScale = new Vector3(randomSizeX, randomSizeY, randomSizeZ);
            }
            else {
                float randomSize = Random.Range(minSize, maxSize);
                shape.transform.localScale = new Vector3(randomSize, randomSize, randomSize);
            }


            material.color = colors[Mathf.Clamp((int)index % numColors, 0, numColors-1)];  // Use modulo to cycle through the three colors

            material.SetFloat("_Metallic", 0.0f);
            material.SetFloat("_Glossiness", 0.175f);

            shape.GetComponent<Renderer>().material = material;


        }
    }


    void ChangeLigths()
    {
        foreach (Transform lightTransform in lightTransforms) {
            Vector3 p = new Vector3(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f)).normalized * Random.Range(5f, 10f);

            lightTransform.position = p;

            Light lightComponent = lightTransform.GetComponent<Light>();

            // Randomize color
            lightComponent.color = Random.ColorHSV();

            // Randomize radius
            lightComponent.range = Random.Range(minLightRadius, maxLightRadius);

            // Set other light properties as needed (e.g., type, intensity, etc.)
            lightComponent.intensity = Random.Range(minLightIntensity, maxLightIntensity);
        }
    }

    void RemoveShapes()
    {
        // Destroy all child objects
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
