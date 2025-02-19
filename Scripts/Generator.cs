using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [Header("Generation Settings")]
    [SerializeField] private Vector2Int regionDim = new(1, 1);

    [SerializeField] private int regionCount;

    List<Vector3> nodes;

    [SerializeField] private float minNodeDistance;

    [Header("Seed, leave 0 for random")]
    [SerializeField] private int seed = 0;

    Triangulation triangulation;
    PolyGenerator polyGenerator;
    CustomMeshTriangleFiller triangleFiller;
    [SerializeField] BuildingGenerator buildingGenerator;

    Dictionary<Vector3, float> nodeHeatMapPairs;

    [SerializeField] private bool showHeatMap;

    [SerializeField] private GameObject particleGroupParent;

    private void Start()
    {   
        // Set the framerate to 60
        Application.targetFrameRate = 120;

        SetRandomSeed();

        triangulation = GetComponent<Triangulation>();

        // add the CustomMeshTriangleFiller component to the game object
        if (!TryGetComponent(out triangleFiller))
            triangleFiller = gameObject.AddComponent<CustomMeshTriangleFiller>();

        // add the PolyGenerator component to the game object
        if (!TryGetComponent(out polyGenerator))
            polyGenerator = gameObject.AddComponent<PolyGenerator>();

        Generate();
    }

    private void SetRandomSeed()
    {
        if (seed != 0)
        {
            Random.InitState(seed);
            Debug.Log("Seed: " + seed);
        }
        else
        {
            seed = Random.Range(0, 1000000);
            Random.InitState(seed);
            Debug.Log("Seed: " + seed);
            seed = 0;
        }
    }

    private void Generate()
    {
        //regionDim = new Vector2Int(1, 1);

        // regionDim *= mapSize;

        GenerateNodes();

        if (polyGenerator.enabled)
        {
            polyGenerator.Generate();
        }
        else
        {
            Debug.LogError("No generation method enabled!");
            return;
        }

        if (polyGenerator.NodeTriangleGroups() == null)
        {
            Debug.LogError("Poly generator component wasn't able to return a valid NodeTriangleGroups dictionary!");
        }

        nodeHeatMapPairs = GenerateHeatMap(polyGenerator.NodeTriangleGroups().Keys.ToList());
        buildingGenerator.Generate(polyGenerator.NodeTriangleGroups(), nodeHeatMapPairs);


        if (triangleFiller.enabled)
        {
            triangleFiller.FillTriangles();
        }

        InitializeParticles();
    }

    /// <summary>
    /// Set the shape of the particle system to a box and scale it to the region dimensions based on the regionDim variable
    /// </summary>
    private void InitializeParticles()
    {
        particleGroupParent.transform.position = new Vector3(regionDim.x / 2, 0, regionDim.y / 2);

        ParticleSystemShapeType boxShape = ParticleSystemShapeType.Box;
        Vector3 boxSize = new Vector3(regionDim.x, 20.0f, regionDim.y);

        foreach (Transform child in particleGroupParent.transform)
        {
            ParticleSystem ps = child.GetComponent<ParticleSystem>();
            var shape = ps.shape;
            shape.shapeType = boxShape;
            shape.scale = boxSize;
        }
    }

    public void Regenerate()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        triangleFiller.ClearMeshes();
        buildingGenerator.ClearBuildings();
        ClearNodes();
        Generate();
    }

    private void ClearNodes()
    {
        if (triangulation != null)
        triangulation.ClearPoints();

        if (polyGenerator != null) 
        polyGenerator.ClearPoints();

        if (nodeHeatMapPairs != null)
        nodeHeatMapPairs.Clear();
    }

    private void GenerateNodes()
    {
        SetRandomSeed();

        nodes = new List<Vector3>();

        for (int i = 0; i < regionCount; i++)
        {
            Vector3 position = new(Random.Range(0, regionDim.x), 0, Random.Range(0, regionDim.y));
            nodes.Add(position);
        }

        if (minNodeDistance > 0)
        {
            nodes = nodes.Where(node => nodes.Count(otherNode => Vector3.Distance(node, otherNode) < minNodeDistance) == 1).ToList();
        }

        foreach (var node in nodes)
        {
            triangulation.AddPoint(node);
        }
    }

    private void OnDrawGizmos()
    {
        if (showHeatMap)
        {
            foreach (var node in nodeHeatMapPairs)
            {
                Gizmos.color = Color.Lerp(Color.green, Color.red, node.Value / nodeHeatMapPairs.Values.Max());
                Gizmos.DrawSphere(node.Key, 3f);
            }
        }
    }
    
    public Dictionary<Vector3, float> GenerateHeatMap(List<Vector3> nodes)
    {
        Dictionary<Vector3, float> nodeHeatPairs = new();

        foreach (var node in nodes)
        {
            float distanceSum = 0;

            foreach (var otherNode in nodes)
            {
                distanceSum += Vector3.Distance(node, otherNode);
            }

            if (!nodeHeatPairs.ContainsKey(node))
            nodeHeatPairs.Add(node, distanceSum);
        }

        return nodeHeatPairs;
    }
}