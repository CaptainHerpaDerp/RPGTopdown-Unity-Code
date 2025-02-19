using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolyGenerator : MonoBehaviour
{
    Triangulation triangulation;
    private List<Triangle> delaunayTriangles;
    private List<Polygon> polygons;
    private List<Vector3> nodes;
    private Dictionary<Vector3, List<Triangle>> nodeTriangleGroups;

    [SerializeField] private int pointNodeIndex;

    [SerializeField] private bool drawPoints, drawTriangles, drawTriangleAreas, drawPolygons, drawConnections;

    public Dictionary<Vector3, List<Vector3>> NodeTriangleGroups()
    {
        // Convert nodeTriangleGroups to a dictionary where the values are centers of the triangles
        Dictionary<Vector3, List<Vector3>> result = new();

        foreach (var group in nodeTriangleGroups)
        {
            List<Vector3> triangleCenters = new();

            foreach (var triangle in group.Value)
            {
                triangleCenters.Add(triangle.Center());
            }

            result.Add(group.Key, triangleCenters);
        }

        return result;
    }

    public List<Vector3> TriangleCenters()
    {
        List<Vector3> triangleCenters = new();

        foreach (var triangle in delaunayTriangles)
        {
            triangleCenters.Add(triangle.Center());
        }

        return triangleCenters;
    }

    public void Generate()
    {
        triangulation = GetComponent<Triangulation>();
        nodes = triangulation.Points;

        triangulation.Triangulate();

        delaunayTriangles = new(triangulation.GetDelaunayTriangulation());
        //GeneratePolygons();
        GeneratePolygonsMethod2();
    }
    
    private void GeneratePolygonsMethod2()
    {
        polygons = new List<Polygon>();

        Triangle startTriangle = null;

        nodeTriangleGroups = new();

        if (delaunayTriangles.Count == 0)
        {
            Debug.LogError("No triangles to generate polygons from!");
            return;
        }

        if (nodes.Count == 0)
        {
            Debug.LogError("No nodes to generate polygons from!");
            return;
        }

        foreach (var parentNode in nodes)
        {
            // Create a nodeTriangleGroups instance for the current node
            if (!nodeTriangleGroups.ContainsKey(parentNode))
                nodeTriangleGroups.Add(parentNode, new());

            List<Triangle> adjacentTriangles = new();

            // Add every triangle that shares a point with the node to the adjacent triangles list
            foreach (Triangle triangle in delaunayTriangles)
            {
                if (triangle.HasPoint(parentNode))
                {              
                    adjacentTriangles.Add(triangle);
                }
            }

            //Debug.Log($"Adjacent triangles for node at index {nodes.IndexOf(parentNode)}: {adjacentTriangles.Count}");

            List<Triangle> remainingTriangles = new(adjacentTriangles);

            if (adjacentTriangles.Count == 0)
                continue;

            // We need to assign the correct triangle that only connects to one triangle, in the case that a full circle cannot be created

            Dictionary<Triangle, int> triangleConnections = new();

            foreach(Triangle triangle in adjacentTriangles)
            {
                triangleConnections.Add(triangle, 0);

                foreach (Triangle triangle2 in remainingTriangles)
                {
                    if (triangle == triangle2)
                        continue;

                    if (triangle.GetSharedVerticies(triangle2) == 2)
                    {
                        triangleConnections[triangle]++;
                    }
                }
            }
                
            int minConnections = int.MaxValue;

            // Set the start triangle to the triangle connections key with the least connections
            foreach (var triangle in triangleConnections.Keys)
            {
                if (triangleConnections[triangle] < minConnections)
                {
                    minConnections = triangleConnections[triangle];
                    startTriangle = triangle;
                }
            }

            if (startTriangle == null)
            {
                Debug.LogError("Fatal Error, StartTriangle was not able to be assigned!");
                continue;
            }

            // Add the start triangle as the group's first entry 
            nodeTriangleGroups[parentNode].Add(startTriangle);

            // Go through each of the adjacent triangles, check if the current start triangle is adjacent to it, if so, add it and set it to the start triangle
            for (int i = 0; i < adjacentTriangles.Count; i++)
            {
                Triangle adjacentTriangle = adjacentTriangles[i];

                if (adjacentTriangle == startTriangle)
                    continue;

                if (nodeTriangleGroups[parentNode].Contains(adjacentTriangle))
                    continue;

                // If the number of shared points between the reference triangle and the current triangle is 2, they are connected
                if (startTriangle.GetSharedVerticies(adjacentTriangle) == 2)
                {
                    nodeTriangleGroups[parentNode].Add(adjacentTriangle);

                    if (nodeTriangleGroups[parentNode].Count == adjacentTriangles.Count)
                    {
                       // Debug.Log($"Connected All Triangles {nodeTriangleGroups[parentNode].Count} , {adjacentTriangles.Count}");
                        break;
                    }

                    startTriangle = adjacentTriangle;
                    i = -1; // Reset the loop to start from the beginning
                }
            }
        }

        if (nodeTriangleGroups.Count == 0)
        {
            Debug.LogError("No nodeTriangleGroups were generated!");
            return;
        }

        foreach (Vector3 parentNode in nodeTriangleGroups.Keys)
        {

            Triangle firstTriangle = nodeTriangleGroups[parentNode][0];

            Triangle lastTriangle = nodeTriangleGroups[parentNode].Last();

            if (firstTriangle.GetSharedVerticies(lastTriangle) == 2)
            {
                nodeTriangleGroups[parentNode].Add(firstTriangle);
            }
        }
    }

    private void GeneratePolygons()
    {
        polygons = new();

        Triangle startTriangle = delaunayTriangles[0];

        nodeTriangleGroups = new();

        // Heatmap - sum of distances to all other points, points with the highest sum of distances are the most central

        foreach (var currentNode in nodes)
        {
            if (!nodeTriangleGroups.ContainsKey(currentNode))
                nodeTriangleGroups.Add(currentNode, new());

            // Find the start triangle that shares the current node
            foreach (Triangle triangle in delaunayTriangles)
            {
                if (triangle.a == currentNode || triangle.b == currentNode || triangle.c == currentNode)
                {
                    startTriangle = triangle;
                    break;
                }
            }

            nodeTriangleGroups[currentNode].Add(startTriangle);

            List<Triangle> adjacentTriangles = new();

            // Record all the triangles that share the current node
            foreach (Triangle triangle in delaunayTriangles)
            {
                // Check if any of the triangles' tips are equal to the current node
                if (triangle.a == currentNode || triangle.b == currentNode || triangle.c == currentNode)
                {
                    adjacentTriangles.Add(triangle);
                }
            }

            Triangle currentTriangle = startTriangle;

            // Find a triangle adjacent to the start triangle that contains the same point
            foreach (Triangle triangle in adjacentTriangles)
            {
                if (currentTriangle == triangle)
                    continue;

                // If the current node has recorded the center of this triangle, skip it
                if (nodeTriangleGroups[currentNode].Contains(triangle))
                    continue;

                Triangle adjacentTriangle = null;

                // Also check adjacency to the previous triangle

                int sharedCount = triangle.GetSharedVerticies(currentTriangle);

                if (sharedCount == 3)
                {
                    Debug.LogError("Found duplicate triangle!");
                    continue;
                }
                else if (sharedCount == 2)
                {
                    Debug.Log("Found valid connection");
                }
                else if (sharedCount == 1)
                {
                    Debug.Log("Found invalid connection");
                    continue;
                }
                else
                {
                    Debug.Log($"Abnormal shared count: {sharedCount}");
                }

                // Check if the triangle shares the current node and if it's not the same as the start triangle
                if (triangle.a == currentNode && currentTriangle.HasPoint(triangle.a) || triangle.b == currentNode && currentTriangle.HasPoint(triangle.a) || triangle.c == currentNode && currentTriangle.HasPoint(triangle.a))
                {
                    if (triangle.a == currentTriangle.a || triangle.a == currentTriangle.b || triangle.a == currentTriangle.c)
                    {
                        adjacentTriangle = triangle;
                    }
                    else if (triangle.b == currentTriangle.a || triangle.b == currentTriangle.b || triangle.b == currentTriangle.c)
                    {
                        adjacentTriangle = triangle;
                    }
                    else if (triangle.c == currentTriangle.a || triangle.c == currentTriangle.b || triangle.c == currentTriangle.c)
                    {
                        adjacentTriangle = triangle;
                    }
                    else
                    {
                        continue;
                    }

                    nodeTriangleGroups[currentNode].Add(adjacentTriangle);

                    // Set the current triangle to the found adjacent triangle so that the next iteration can check for its adjacent triangles
                    currentTriangle = adjacentTriangle;
                }
            }

            List<Triangle> remainingTriangles = new();

            // Find any remaining adjacent triangles that are not in the list
            foreach (Triangle triangle in adjacentTriangles)
            {
                if (!nodeTriangleGroups[currentNode].Contains(triangle))
                {
                    remainingTriangles.Add(triangle);
                }
            }

            if (remainingTriangles.Count == 0)
                continue;

            currentTriangle = remainingTriangles[0];

            if (remainingTriangles.Count == 1)
            {
                Debug.Log("Only one remaining triangle");

                // Match the lone triangle with any of the existing triangles that share 2 points
                foreach (Triangle triangle in adjacentTriangles)
                {
                    if (triangle == currentTriangle)
                        continue;

                    int sharedCount = 0;

                    foreach (var vertex in triangle.GetVertices())
                    {
                        foreach (var vertex2 in currentTriangle.GetVertices())
                        {
                            if (vertex == vertex2)
                            {
                                sharedCount++;
                            }
                        }
                    }

                    if (sharedCount == 2)
                    {
                        Debug.Log("Connected remaining triangle with valid connection");

                        // Add the center of the triangle to the list, but ensure its in the correct index
                        if (nodeTriangleGroups[currentNode].IndexOf(triangle) == 0)
                        {
                            nodeTriangleGroups[currentNode].Insert(0, currentTriangle);
                        }
                        else
                        {
                            nodeTriangleGroups[currentNode].Add(currentTriangle);
                        }

                      //  nodeTriangleGroups[currentNode].Add(currentTriangle.Center());
                        break;
                    }
                }

                continue;
            }

            // Find a triangle adjacent to the start triangle that contains the same point
            foreach (Triangle triangle in remainingTriangles)
            {
                if (currentTriangle == triangle)
                    continue;

                Triangle adjacentTriangle = null;

                // Also check adjacency to the previous triangle

                // Check if the triangle shares the current node and if it's not the same as the start triangle
                int sharedCount = 0;

                foreach (var vertex in triangle.GetVertices())
                {
                    foreach (var vertex2 in currentTriangle.GetVertices())
                    {
                        if (vertex == vertex2)
                        {
                            sharedCount++;
                        }
                    }
                }

                if (sharedCount == 3)
                {
                    Debug.LogError("Found duplicate triangle!");
                    continue;
                }
                else if (sharedCount == 2)
                {
                    Debug.Log("Found valid connection");
                }
                else if (sharedCount == 1)
                {
                    Debug.Log("Found invalid connection");
                    continue;
                }
                else
                {
                    Debug.Log($"Abnormal shared count: {sharedCount}");
                }

                if (triangle.a == currentNode && currentTriangle.HasPoint(triangle.a) || triangle.b == currentNode && currentTriangle.HasPoint(triangle.a) || triangle.c == currentNode && currentTriangle.HasPoint(triangle.a))
                {   

                    if (triangle.a == currentTriangle.a || triangle.a == currentTriangle.b || triangle.a == currentTriangle.c)
                    {
                        adjacentTriangle = triangle;
                    }
                    else if (triangle.b == currentTriangle.a || triangle.b == currentTriangle.b || triangle.b == currentTriangle.c)
                    {
                        adjacentTriangle = triangle;
                    }
                    else if (triangle.c == currentTriangle.a || triangle.c == currentTriangle.b || triangle.c == currentTriangle.c)
                    {
                        adjacentTriangle = triangle;
                    }
                    else
                    {
                        continue;
                    }

                    nodeTriangleGroups[currentNode].Add(adjacentTriangle);

                    // Set the current triangle to the found adjacent triangle so that the next iteration can check for its adjacent triangles
                    currentTriangle = adjacentTriangle;
                }
            }
        }

        foreach (Vector3 parentNode in nodeTriangleGroups.Keys)
        {
            Triangle firstTriangle = nodeTriangleGroups[parentNode][0];

            Triangle lastTriangle = nodeTriangleGroups[parentNode].Last();

            if (firstTriangle.GetSharedVerticies(lastTriangle) == 2)
            {
                nodeTriangleGroups[parentNode].Add(firstTriangle);
            }
        }
    }


    void OnDrawGizmos()
    {
        if (delaunayTriangles == null)
        {
            return;
        }

        if (polygons == null)
        {
            return;
        }

        if (drawPoints)
        {
            Gizmos.color = Color.gray;
            foreach (var point in triangulation.Points)
            {
                Gizmos.DrawSphere(point, 1);
            }
        }

        if (pointNodeIndex != -1 && pointNodeIndex < NodeTriangleGroups().Count)
        {
            Dictionary<Vector3, List<Vector3>> nodeTriangleCenterPairs = NodeTriangleGroups();

            Gizmos.color = Color.yellow;

            // Draw a yellow sphere at the parent node
            Gizmos.DrawSphere(nodeTriangleCenterPairs.Keys.ToArray()[pointNodeIndex], 2);

            Gizmos.color = Color.cyan;
            foreach (Vector3 triangleCenter in nodeTriangleCenterPairs.Values.ToArray()[pointNodeIndex])
            {
                Gizmos.DrawSphere(triangleCenter, 1.5f);
            }
        }

        Gizmos.color = Color.green;
        foreach (Polygon polygon in polygons)
        {
            for (int i = 0; i < polygon.vertices.Count - 1; i++)
            {
                Debug.Log("Drawing line from " + polygon.vertices[i] + " to " + polygon.vertices[i + 1]);
                Gizmos.DrawLine(polygon.vertices[i], polygon.vertices[i + 1]);
            }
        }

        if (drawPolygons) {

            Dictionary<Vector3, List<Vector3>> nodeTriangleCenterPairs = NodeTriangleGroups();
            Gizmos.color = Color.red;

            foreach (Vector3 nodeParent in nodeTriangleCenterPairs.Keys)
            {
                for (int i = 0; i < nodeTriangleCenterPairs[nodeParent].Count - 1; i++)
                {
                    Gizmos.DrawLine(nodeTriangleCenterPairs[nodeParent][i], nodeTriangleCenterPairs[nodeParent][i + 1]);
                }
            }
        }

        if (drawTriangleAreas)
            foreach (Triangle triangle in delaunayTriangles)
            {
                Gizmos.color = Color.blue;

                Vector3 center = triangle.Center();

                Gizmos.DrawSphere(center, 1);

                Gizmos.DrawLine(center, triangle.a);
                Gizmos.DrawLine(center, triangle.b);
                Gizmos.DrawLine(center, triangle.c);
            }


        if (drawTriangles)
            foreach (Triangle triangle in delaunayTriangles)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(triangle.a, triangle.b);
                Gizmos.DrawLine(triangle.b, triangle.c);
                Gizmos.DrawLine(triangle.c, triangle.a);
            }

        if (drawConnections)
            foreach (Triangle triangle in delaunayTriangles)
            {
                // Calculate the centroid of the triangle
                Vector3 centroid = (triangle.a + triangle.b + triangle.c) / 3f;

                // Draw a small sphere at the centroid for reference
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(triangle.Center(), 1f);
            }
    }


    public void ClearPoints()
    {
        if (delaunayTriangles != null)
        {
            delaunayTriangles.Clear();
        }
    }
}
