using System.Collections.Generic;
using UnityEngine;

public class VoronoiGeneration : MonoBehaviour
{
    Triangulation triangulation;
    private List<Triangle> delaunayTriangles;
    private List<VoronoiCell> voronoiCells;


    void Start()
    {
        triangulation = GetComponent<Triangulation>();
    }

    public void Generate()
    {
        triangulation.Triangulate();

        delaunayTriangles = new(triangulation.GetDelaunayTriangulation());
        voronoiCells = new();

        GenerateVoronoiCells();
    }

    private void GenerateVoronoiCells()
    {
        voronoiCells.Clear();

        HashSet<Edge> processedEdges = new HashSet<Edge>();

        // Iterate through each Delaunay triangle
        foreach (Triangle delaunayTriangle in delaunayTriangles)
        {
            Vector3 circumcenter = triangulation.GetCircumcenter(delaunayTriangle);
            VoronoiCell voronoiCell = new VoronoiCell(circumcenter);

            // Iterate through each edge of the Delaunay triangle
            foreach (Edge delaunayEdge in delaunayTriangle.GetEdges())
            {
                // Avoid processing duplicate edges
                if (!processedEdges.Contains(delaunayEdge))
                {
                    // Find connected triangles (Voronoi cells)
                    List<Triangle> connectedTriangles = FindConnectedTriangles(delaunayEdge);

                    // Get Voronoi cell vertices (midpoints of Delaunay edges)
                    Vector3 vertex1 = (delaunayEdge.a + delaunayEdge.b) / 2f;
                    Vector3 vertex2 = circumcenter;

                    // Connect the Voronoi cell vertices to form Voronoi cell edges
                    Edge voronoiEdge = new Edge(vertex1, vertex2);

                    // Add the Voronoi edge to the cell
                    voronoiCell.AddEdge(voronoiEdge);

                    // Add the Voronoi edge to the processed set
                    processedEdges.Add(voronoiEdge);
                }
            }

            voronoiCells.Add(voronoiCell);
        }
    }


    private List<Triangle> FindConnectedTriangles(Edge edge)
    {
        List<Triangle> connectedTriangles = new();

        foreach (Triangle delaunayTriangle in delaunayTriangles)
        {
            // Check if the edge is part of the Delaunay triangle
            if (delaunayTriangle.GetEdges().Exists(e =>
                (e.a == edge.a && e.b == edge.b) || (e.a == edge.b && e.b == edge.a)))
            {
                connectedTriangles.Add(delaunayTriangle);
            }
        }

        return connectedTriangles;
    }

    private void OnDrawGizmos()
    {
        if (delaunayTriangles != null)
        {
            Gizmos.color = Color.blue;
            foreach (Triangle delaunayTriangle in delaunayTriangles)
            {
                Gizmos.DrawLine(delaunayTriangle.a, delaunayTriangle.b);
                Gizmos.DrawLine(delaunayTriangle.b, delaunayTriangle.c);
                Gizmos.DrawLine(delaunayTriangle.c, delaunayTriangle.a);
            }
        }

        if (voronoiCells != null)
        {
            Gizmos.color = Color.green;
            foreach (VoronoiCell voronoiCell in voronoiCells)
            {
                foreach (Edge edge in voronoiCell.GetEdges())
                {
                    Gizmos.DrawLine(edge.a, edge.b);
                }
            }
        }
    }

    public void ClearPoints()
    {
        delaunayTriangles.Clear();
        voronoiCells.Clear();
    }
}
