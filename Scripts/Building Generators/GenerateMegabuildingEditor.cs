#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerateBuilding), true)]
[CanEditMultipleObjects]
public class GenerateBuildingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GenerateBuilding generator = (GenerateBuilding)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Building"))
        {
            generator.Generate(generator.BuildingHeight);
        }

        if (GUILayout.Button("Rotate"))
        {
            generator.SetRotation(new Vector3(0, 30, 0));
        }
    }
}

#endif