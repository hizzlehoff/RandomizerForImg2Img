using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridRandomizer))]
public class GridRandomizerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridRandomizer gridRandomizer = (GridRandomizer)target;

        if (GUILayout.Button("Randomize Grid")) {
            gridRandomizer.CreateGrid();
        }

        if (GUILayout.Button("Random walk Grid"))
        {
            gridRandomizer.CreateRandomWalkGrid();
        }
    }
}
