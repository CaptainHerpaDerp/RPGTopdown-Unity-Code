using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Triangulation : MonoBehaviour 
{
    [SerializeField] private int V = 10;
    public List<Vector3> Points { get; private set; }
    private List<Triangle> triangles;

    Triangle superTriangle;

    [SerializeField] bool removeBadTriangles = false;

    public Triangulation()
    {
        Points = new List<Vector3>();
        triangles = new List<Triangle>();
    }

    public void AddPoint(Vector3 point)
    {
        Points.Add(point);
    }

    public void ClearPoints()
    {
        Points.Clear();
        triangles.Clear();
    }

    public List<Triangle> GetDelaunayTriangulation()
    {
        if (Points.Count < 3)
        {
            Debug.LogError("At least 3 Points are required to perform a triangulation");
            return null;
        }    

        Triangulate();

        if (triangles.Count == 0)
        {
            Debug.LogError("No triangles were created");
            return null;
        }
           
        return triangles;
    }

    public void Triangulate()
    {
        AddSuperTriangle();

        foreach (Vector3 point in Points)
        {
            List<Triangle> badTriangles = FindBadTriangles(point);

            List<Edge> polygon = FindPolygonalHole(badTriangles);

            foreach (Triangle triangle in badTriangles)
            {
                triangles.Remove(triangle);
            }

            foreach (Edge edge in polygon)
            {
                Triangle newTri = new Triangle(edge.a, edge.b, point);
                triangles.Add(newTri);
            }
        }

        // Remove bad triangles outside of the loop
        if (removeBadTriangles)
        {
            List<Triangle> badTrianglesToRemove = new();

            foreach (Triangle triangle in triangles)
            {
                if (ContainsVertexFromSuperTriangle(triangle))
                {
                    badTrianglesToRemove.Add(triangle);
                }

                if (TriangleArea(triangle) == 0)
                {
                    badTrianglesToRemove.Add(triangle);
                }
            }

            foreach (Triangle triangle in badTrianglesToRemove)
            {
                triangles.Remove(triangle);
            }
        }

        // Remove Duplicate Triangles
        List<Triangle> trianglesToRemove = new();

        for (int i = 0; i < triangles.Count; i++)
        {
            Triangle triangle1 = triangles[i];
            for (int j = i + 1; j < triangles.Count; j++)
            {
                Triangle triangle2 = triangles[j];

                // Check if the triangles have the same vertices
                if (triangle1.SharesAllVerticies(triangle2))
                {
                    trianglesToRemove.Add(triangle2);
                }
            }
        }

        foreach (Triangle triangle in trianglesToRemove)
        {
            triangles.Remove(triangle);
        }

       // Debug.Log("Duplicate Count: " + trianglesToRemove.Count);

        foreach (Triangle triangle in trianglesToRemove)
        {
            triangles.Remove(triangle);
        }
    }

    private bool ContainsVertexFromSuperTriangle(Triangle triangle)
    {
        return triangle.a == superTriangle.a || triangle.a == superTriangle.b || triangle.a == superTriangle.c ||
               triangle.b == superTriangle.a || triangle.b == superTriangle.b || triangle.b == superTriangle.c ||
               triangle.c == superTriangle.a || triangle.c == superTriangle.b || triangle.c == superTriangle.c;
    }

    private void AddSuperTriangle()
    {
        // Create a super-triangle large enough to contain all Points
        float minX = float.MaxValue, minZ = float.MaxValue;
        float maxX = float.MinValue, maxZ = float.MinValue;

        foreach (Vector3 point in Points)
        {
            minX = Mathf.Min(minX, point.x);
            minZ = Mathf.Min(minZ, point.y);
            maxX = Mathf.Max(maxX, point.x);
            maxZ = Mathf.Max(maxZ, point.y);
        }

        Vector3 super1 = new Vector3((minX + maxX) / 2, 0, minZ - V); // Adjust the yOffset as needed
        Vector3 super2 = new Vector3(minX - V, 0, maxZ + V);
        Vector3 super3 = new Vector3(maxX + V, 0, maxZ + V);

        superTriangle = new Triangle(super1, super2, super3);
        triangles.Add(superTriangle);
    }

    private List<Triangle> FindBadTriangles(Vector3 point)
    {
        List<Triangle> badTriangles = new List<Triangle>();

        foreach (Triangle triangle in triangles)
        {
            if (IsPointInCircumcircle(point, triangle))
            {
                badTriangles.Add(triangle);
            }
        }

        return badTriangles;
    }

    private bool IsPointInCircumcircle(Vector3 point, Triangle triangle)
    {
        Vector3 center = GetCircumcenter(triangle);
        float radius = Vector3.Distance(center, triangle.a);

        return Vector3.Distance(center, point) < radius;
    }

    private List<Edge> FindPolygonalHole(List<Triangle> badTriangles)
    {
        List<Edge> polygon = new List<Edge>();

        foreach (Triangle triangle in badTriangles)
        {
            foreach (Edge edge in triangle.GetEdges())
            {
                if (!IsEdgeShared(edge, badTriangles))
                {
                    polygon.Add(edge);
                }
            }
        }

        return polygon;
    }

    public Vector3 GetCircumcenter(Triangle triangle)
    {
        float D = 2 * (triangle.a.x * (triangle.b.z - triangle.c.z) + triangle.b.x * (triangle.c.z - triangle.a.z) + triangle.c.x * (triangle.a.z - triangle.b.z));
        float Ux = ((triangle.a.x * triangle.a.x + triangle.a.z * triangle.a.z) * (triangle.b.z - triangle.c.z) + (triangle.b.x * triangle.b.x + triangle.b.z * triangle.b.z) * (triangle.c.z - triangle.a.z) + (triangle.c.x * triangle.c.x + triangle.c.z * triangle.c.z) * (triangle.a.z - triangle.b.z)) / D;
        float Uz = ((triangle.a.x * triangle.a.x + triangle.a.z * triangle.a.z) * (triangle.c.x - triangle.b.x) + (triangle.b.x * triangle.b.x + triangle.b.z * triangle.b.z) * (triangle.a.x - triangle.c.x) + (triangle.c.x * triangle.c.x + triangle.c.z * triangle.c.z) * (triangle.b.x - triangle.a.x)) / D;

        return new Vector3(Ux, 0, Uz);
    }

    // Return the area of a triangle
    private float TriangleArea(Triangle triangle)
    {
        float a = Vector3.Distance(triangle.a, triangle.b);
        float b = Vector3.Distance(triangle.b, triangle.c);
        float c = Vector3.Distance(triangle.c, triangle.a);

        float s = (a + b + c) / 2;

        return Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
    }

    private bool IsEdgeShared(Edge edge, List<Triangle> triangles)
    {
        int count = 0;
        foreach (Triangle triangle in triangles)
        {
            foreach (Edge triangleEdge in triangle.GetEdges())
            {
                if (edge.a == triangleEdge.a && edge.b == triangleEdge.b || edge.a == triangleEdge.b && edge.b == triangleEdge.a)
                {
                    count++;
                }
            }
        }

        return count > 1;
    }
}
