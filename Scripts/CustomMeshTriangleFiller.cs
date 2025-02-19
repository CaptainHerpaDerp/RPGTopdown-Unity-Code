using System.Collections.Generic;
using UnityEngine;

public class CustomMeshTriangleFiller : MonoBehaviour
{
    Triangulation triangulation;

    [SerializeField] private Material triangleMat;
    [SerializeField] private GameObject trianglePrefab;

    private List<GameObject> triangleObjects;

    [SerializeField] private float padding;

    public void FillTriangles()
    {
        triangulation = GetComponent<Triangulation>();

        List<Triangle> triangles = new(triangulation.GetDelaunayTriangulation());

        if (triangles.Count == 0)
        {
            Debug.LogError("No triangles to fill");
            return;
        }

        triangleObjects = new();

        MeshBuilder builder = new();

        foreach (Triangle triangle in triangles)
        {
            GameObject newTri = Instantiate(trianglePrefab, triangle.a, Quaternion.identity);

            int v1 = builder.AddVertex(triangle.a, new Vector2(0, 0));
            int v2 = builder.AddVertex(triangle.b, new Vector2(1, 0));
            int v3 = builder.AddVertex(triangle.c, new Vector2(0, 1));
            builder.AddTriangle(v1, v2, v3);

            newTri.GetComponent<MeshFilter>().mesh = builder.CreateMesh();
            newTri.GetComponent<MeshRenderer>().material = triangleMat;

            triangleObjects.Add(newTri);
        }

        GameObject lastPoly = triangleObjects[^1];

        lastPoly.transform.localScale = new Vector3(padding, 1, padding);
        lastPoly.transform.position = Vector3.zero + new Vector3(-padding*4, 0, -padding * 4);

        for (int i = 0; i < triangleObjects.Count - 1; i++)
        {
            Destroy(triangleObjects[i]);
        }

    }

    public void ClearMeshes()
    {
        if (triangleObjects != null)
        {
            foreach (GameObject triangle in triangleObjects)
            {
                Destroy(triangle);
            }

            triangleObjects.Clear();
        }
    }
}
