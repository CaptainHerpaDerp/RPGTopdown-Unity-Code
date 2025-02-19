using System.Collections.Generic;
using UnityEngine;


public class Triangle
{
    public Vector3 a, b, c;

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }

    public List<Vector3> GetVertices()
    {
        return new List<Vector3> { a, b, c };
    }

    /// <summary>
    /// Returns true if all this triangle's verticies are shared with the compareTriangle
    /// </summary>
    /// <param name="compareTriangle"></param>
    /// <returns></returns>
    public bool SharesAllVerticies(Triangle compareTriangle)
    {
        if (compareTriangle.HasPoint(a) && compareTriangle.HasPoint(b) && compareTriangle.HasPoint(c))
        {
            return true;
        }

        return false;
    }

    public int GetSharedVerticies(Triangle compareTriangle)
    {
        int sharedVerticies = 0;

        if (compareTriangle.HasPoint(a))
        {
            sharedVerticies++;
        }

        if (compareTriangle.HasPoint(b))
        {
            sharedVerticies++;
        }

        if (compareTriangle.HasPoint(c))
        {
            sharedVerticies++;
        }

        return sharedVerticies;
    }

    public bool HasPoint(Vector3 position)
    {
        if (a == position || b == position || c == position)
        {
            return true;
        }

        return false;
    }

    public Vector3 GetPoint(Vector3 position)
    {
        if (a == position)
        {
            return a;
        }
        else if (b == position)
        {
            return b;
        }
        else if (c == position)
        {
            return c;
        }

        return Vector3.zero;
    }

    public List<Edge> GetEdges()
    {
        List<Edge> edges = new()
        {
            new Edge(a, b),
            new Edge(b, c),
            new Edge(c, a)
        };

        return edges;
    }

    // Return the center of the triangle
    public Vector3 Center()
    {
        float x = (a.x + b.x + c.x) / 3;
        float y = (a.y + b.y + c.y) / 3;
        float z = (a.z + b.z + c.z) / 3;

        return new Vector3(x, y, z);
    }
}

public class Edge
{
    public Vector3 a, b;

    public Edge(Vector3 a, Vector3 b)
    {
        this.a = a;
        this.b = b;
    }
}

public class VoronoiCell
{
    private List<Edge> edges;

    public VoronoiCell(Vector3 center)
    {
        Center = center;
        edges = new List<Edge>();
    }

    public Vector3 Center { get; }

    public void AddEdge(Vector3 point1, Vector3 point2)
    {
        edges.Add(new Edge(point1, point2));
    }

    public void AddEdge(Edge edge)
    {
        edges.Add(edge);
    }

    public List<Edge> GetEdges()
    {
        return edges;
    }
}

public class Polygon
{
    public List<Vector3> vertices = new();

    public void AddVertex(Vector3 vertex)
    {
        vertices.Add(vertex);
    }

    public void Close()
    {
        if (vertices.Count > 2)
        {
            vertices.Add(vertices[0]); // Close the loop
        }
    }
}