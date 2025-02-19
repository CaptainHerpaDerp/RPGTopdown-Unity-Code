using UnityEngine;

public class CustomMeshTest : MonoBehaviour
{
    [SerializeField] private float length, width, height;

    private void Start()
    {
        Generate();
    }

    private void OnEnable()
    {
        Generate();
    }

    public void Generate()
    {
        MeshBuilder builder = new MeshBuilder();

        // Default Rectangle

        //// Vertices
        //int v1 = builder.AddVertex(new Vector3(-1, 0, -1), new Vector2(0, 0));
        //int v2 = builder.AddVertex(new Vector3(-1, 0, 1), new Vector2(0, 1));
        //int v3 = builder.AddVertex(new Vector3(1, 0, 1), new Vector2(1, 1));
        //int v4 = builder.AddVertex(new Vector3(1, 0, -1), new Vector2(1, 0));
        //int v5 = builder.AddVertex(new Vector3(-1, 1, -1), new Vector2(0, 0));
        //int v6 = builder.AddVertex(new Vector3(-1, 1, 1), new Vector2(0, 1));
        //int v7 = builder.AddVertex(new Vector3(1, 1, 1), new Vector2(1, 1));
        //int v8 = builder.AddVertex(new Vector3(1, 1, -1), new Vector2(1, 0));

        //// Top face
        //builder.AddQuad(v5, v6, v7, v8);

        //// Bottom face
        //builder.AddQuad(v1, v4, v3, v2);

        //// Side faces
        //builder.AddQuad(v1, v2, v6, v5);
        //builder.AddQuad(v2, v3, v7, v6);
        //builder.AddQuad(v3, v4, v8, v7);    
        //builder.AddQuad(v4, v1, v5, v8); 


        // Flat-Top Pyramid

        //Vertices

        // Vertices
        int v1 = builder.AddVertex(new Vector3(-width / 2, 0, -length / 2), new Vector2(0, 0));
        int v2 = builder.AddVertex(new Vector3(-width / 2, 0, length / 2), new Vector2(0, 1));
        int v3 = builder.AddVertex(new Vector3(width / 2, 0, length / 2), new Vector2(1, 1));
        int v4 = builder.AddVertex(new Vector3(width / 2, 0, -length / 2), new Vector2(1, 0));
        int v5 = builder.AddVertex(new Vector3(-width / 4, height, -length / 4), new Vector2(0, 0));
        int v6 = builder.AddVertex(new Vector3(-width / 4, height, length / 4), new Vector2(0, 1));
        int v7 = builder.AddVertex(new Vector3(width / 4, height, length / 4), new Vector2(1, 1));
        int v8 = builder.AddVertex(new Vector3(width / 4, height, -length / 4), new Vector2(1, 0));

        // Top face
        builder.AddQuad(v5, v6, v7, v8);

        // Bottom face
        builder.AddQuad(v1, v2, v3, v4);

        // Side faces
        builder.AddQuad(v1, v2, v6, v5);
        builder.AddQuad(v2, v3, v7, v6);
        builder.AddQuad(v3, v4, v8, v7);
        builder.AddQuad(v4, v1, v5, v8);


        // Pentagonal

        // Vertices

        //float radius = 0.5f;
        //float height = 1.0f;

        //int numVertices = 5;
        //Vector3[] bottomVertices = new Vector3[numVertices];
        //Vector3[] topVertices = new Vector3[numVertices];
        //Vector2[] uvs = new Vector2[numVertices * 2];

        //// Create bottom and top vertices
        //for (int i = 0; i < numVertices; i++)
        //{
        //    float angle = i * 2 * Mathf.PI / numVertices;
        //    bottomVertices[i] = new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));
        //    topVertices[i] = new Vector3(radius * Mathf.Cos(angle), height, radius * Mathf.Sin(angle));
        //    uvs[i] = new Vector2(bottomVertices[i].x + 0.5f, bottomVertices[i].z + 0.5f); // Mapping UVs to [0, 1] range
        //    uvs[i + numVertices] = new Vector2(topVertices[i].x + 0.5f, topVertices[i].z + 0.5f); // Mapping UVs to [0, 1] range
        //}

        //// Add bottom vertices to the builder
        //int[] bottomVertexIndices = new int[numVertices];
        //for (int i = 0; i < numVertices; i++)
        //{
        //    bottomVertexIndices[i] = builder.AddVertex(bottomVertices[i], uvs[i]);
        //}

        //// Create bottom face
        //for (int i = 0; i < numVertices - 2; i++)
        //{
        //    builder.AddTriangle(bottomVertexIndices[0], bottomVertexIndices[i + 1], bottomVertexIndices[i + 2]);
        //}

        //// Add top vertices to the builder
        //int[] topVertexIndices = new int[numVertices];
        //for (int i = 0; i < numVertices; i++)
        //{
        //    topVertexIndices[i] = builder.AddVertex(topVertices[i], uvs[i + numVertices]);
        //}

        //// Create top face
        //for (int i = 0; i < numVertices - 2; i++)
        //{
        //    builder.AddTriangle(topVertexIndices[i + 2], topVertexIndices[i + 1], topVertexIndices[0]);
        //}

        //// Create side faces
        //for (int i = 0; i < numVertices; i++)
        //{
        //    int nextIndex = (i + 1) % numVertices;
        //    builder.AddQuad(bottomVertexIndices[nextIndex], bottomVertexIndices[i], topVertexIndices[i], topVertexIndices[nextIndex]);
        //}

        GetComponent<MeshFilter>().mesh = builder.CreateMesh();
    }
}
