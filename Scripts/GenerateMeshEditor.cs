using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomMeshTest))]
public class GenerateMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CustomMeshTest generator = (CustomMeshTest)target;

        if (GUILayout.Button("Generate Building"))
        {
            generator.Generate();
        }
    }
}
