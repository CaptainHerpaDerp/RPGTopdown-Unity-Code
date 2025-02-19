using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingGenerator))]
public class BuildingGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BuildingGenerator generator = (BuildingGenerator)target;

        // Button to add a new item
        if (GUILayout.Button("Reset Buildings"))
        {
            generator.BuildingGenerators.Clear();
            generator.BuildingGeneratorScalePreferences.Clear();
        }

        for (int i = 0; i < generator.BuildingGenerators.Count; i++)
        {
            if (generator.BuildingGeneratorScalePreferences.Count <= i)
            {
                continue; // Skip the rest of the loop iteration to prevent accessing null elements
            }

            if (generator.BuildingGeneratorScalePreferences[i] < generator.BuildingScaleMin)
            {
                generator.BuildingGeneratorScalePreferences[i] = generator.BuildingScaleMin;
            }

            EditorGUILayout.BeginHorizontal();

            GUILayoutOption[] buttonOptions = { GUILayout.Width(20) };

            if (GUILayout.Button("X", buttonOptions))
            {
                generator.BuildingGenerators.RemoveAt(i);
                generator.BuildingGeneratorScalePreferences.RemoveAt(i);
                continue; // Skip the rest of the loop iteration to prevent accessing null elements
            }

            try
            {
                generator.BuildingGenerators[i] = EditorGUILayout.ObjectField(generator.BuildingGenerators[i], typeof(GenerateBuilding), true) as GenerateBuilding;
                generator.BuildingGeneratorScalePreferences[i] = EditorGUILayout.FloatField("Scale Preference", generator.BuildingGeneratorScalePreferences[i]);
            }
            catch (System.ArgumentOutOfRangeException)
            {

            }

            EditorGUILayout.EndHorizontal();
        }

        // Button to add a new item
        if (GUILayout.Button("Add Building"))
        {
            // Add a new empty item to the shop data
            generator.BuildingGenerators.Add(null);
            generator.BuildingGeneratorScalePreferences.Add(1);
        }

        EditorGUILayout.Space();

        DrawDefaultInspector();

        EditorGUILayout.Space();

        // Min-max slider for heatMapHeightLow and heatMapHeightPeak
        EditorGUILayout.LabelField("Building Height Settings", EditorStyles.boldLabel);
        float minValue = generator.heatMapHeightLow;
        float maxValue = generator.heatMapHeightPeak;

        // Ensure that minValue is not greater than maxValue
        if (minValue > maxValue)
        {
            minValue = maxValue;
        }

        // set the sliders gui width to 100
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.MinMaxSlider(ref minValue, ref maxValue, 0, 100);
        if (EditorGUI.EndChangeCheck())
        {
            // Update the generator properties
            generator.heatMapHeightLow = minValue;
            generator.heatMapHeightPeak = maxValue;
        } 
    }
}
