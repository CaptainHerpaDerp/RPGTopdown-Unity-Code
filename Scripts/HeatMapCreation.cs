using System.Collections.Generic;
using UnityEngine;

public class HeatMapCreation : MonoBehaviour
{
    // Heatmap - sum of distances to all other points, points with the highest sum of distances are the most central

    public Dictionary<Vector3, float> GenerateHeatMap(List<Vector3> nodes)
    {
        Dictionary<Vector3, float> nodeHeatPairs = new();

        foreach (var node in nodes)
        {
            float distanceSum = 0;

            foreach (var otherNode in nodes)
            {
                if (node != otherNode)
                {
                    distanceSum += Vector3.Distance(node, otherNode); 
                }
            }

            // Calculate average distance
            float averageDistance = distanceSum / (nodes.Count - 1);

            // Invert the average distance to get the heat value
            float heatValue = 1 / averageDistance;

            nodeHeatPairs.Add(node, heatValue);
        }

        return nodeHeatPairs;
    }
}
