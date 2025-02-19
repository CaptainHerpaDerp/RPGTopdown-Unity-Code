using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Generator))]
public class GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Space(10);

        Generator generator = (Generator)target;

        // Button to add a new item
        if (GUILayout.Button("Regenerate"))
        {
            // Add a new empty item to the shop data
            generator.Regenerate();
        }

        GUILayout.Space(10);

        DrawDefaultInspector();
    }
}
