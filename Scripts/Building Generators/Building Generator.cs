using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class BuildingGenerator : MonoBehaviour
{
    [SerializeField] private GameObject roadPrefab, roadCylinderPrefab, lampPostPrefab;

    [SerializeField] private float lampPostSpacing, lampPostRoadDistance, lampPostNodeDistance;

    //[Header("Add all building generators that will be created throughout the city")]
    [HideInInspector] public List<GenerateBuilding> BuildingGenerators;
    [HideInInspector] public List<float> BuildingGeneratorScalePreferences;

    private Dictionary<GenerateBuilding, float> BuildingGeneratorPreferencePairs;

    [Header("The prefab that will be used to visualize plot areas")]
    [SerializeField] private GameObject plotPrefab;

    [SerializeField] private int maxScale;
    [SerializeField] private float spawnDistance;

    private Dictionary<Vector3, List<Vector3>> nodeTriangleGroups;

    [SerializeField] private Transform plotParent;
    [SerializeField] private Transform buildingParent;
    [SerializeField] private Transform roadParent;

    private Dictionary<Vector3, GameObject> roadVerticesGOGroups;
    Dictionary<Vector3, float> nodeHeatMapPairs;

    // Debug
    [SerializeField] private int startDistModifier;
    [SerializeField] private bool destroyOverlaps;
    [SerializeField] private float roadWidth;

    private List<Block> blocks;

    [SerializeField] private bool recheckOverlaps;

    private List<GameObject> lampObjects;
    private List<BoxCollider> roadColliders;
    private List<BoxCollider> plotColliders;

    private Dictionary<GameObject, int> blockObjectHeightPairs;

    // The value that multiplies the heatmap value to give the building height
    public float heatMapHeightLow;
    public float heatMapHeightPeak;

    [SerializeField] private float heatmapIntensity;

    [SerializeField] private bool GenerateBuildings;

    [SerializeField] private float buildingScaleMin, buildingScaleMax, buildingScaleHeatMapMultiplier;
    public float BuildingScaleMin { get => buildingScaleMin; }

    [Header("Post-heatmap scaling to add some variation to the scale")]
    [SerializeField] private float buildingScaleModifierMin;
    [SerializeField] private float buildingScaleModifierMax;

    [Header("Post-heatmap scaling to add some variation to the height")]
    [SerializeField] private float buildingHeightModifierMin;
    [SerializeField] private float buildingHeightModifierMax;

    [Header("Building Scaling Modifiers")]
    [SerializeField] private float buildingWidthModifierMin;
    [SerializeField] private float buildingWidthModifierMax;

    [SerializeField] private float buildingLengthModifierMin, buildingLengthModifierMax;

    // Debugging
    private System.Diagnostics.Stopwatch coroutineStopwatch;

    [SerializeField] private float roadDistanceCheckRange, buildingDistanceCheckRange;

    [SerializeField] private bool destoryRoadOverlaps, destroyBuildingOverlaps;

    public void ClearBuildings()
    {
        foreach (Transform child in buildingParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in roadParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in plotParent)
        {
            Destroy(child.gameObject);
        }

        foreach (GameObject lamp in lampObjects)
        {
            Destroy(lamp);
        }

        if (blockObjectHeightPairs != null)
        foreach (GameObject block in blockObjectHeightPairs.Keys)
        {
            Destroy(block);
        }
    }

    public void Generate(Dictionary<Vector3, List<Vector3>> groupDictionary, Dictionary<Vector3, float> heatmapPairDictionary)
    {
        nodeHeatMapPairs = heatmapPairDictionary;
        coroutineStopwatch = System.Diagnostics.Stopwatch.StartNew();
        StartCoroutine(GenerateCR(groupDictionary));
    }

    private IEnumerator GenerateCR(Dictionary<Vector3, List<Vector3>> groupDictionary)
    {
        // Enable the plot parent to allow the plots to be visible
        nodeTriangleGroups = groupDictionary;

        GenerateRoads();

        // Wait for a fixed update to ensure the roads have been generated and their colliders can be accessed
        yield return new WaitForFixedUpdate();

        StartCoroutine(GenerateBlockBuildings());

        coroutineStopwatch.Stop();

        float actualTime = coroutineStopwatch.ElapsedMilliseconds;

        Debug.Log("Coroutine duration: " + actualTime / 100);

        // Disable the plot parent as the new plots have been generated, and are hidden
        ControlPanel.Instance.PlotToggle.isOn = false;

        yield return null;
    }

    private void GenerateRoads()
    {
        roadColliders = new();
        plotColliders = new();
        lampObjects = new();
        blocks = new();

        roadVerticesGOGroups = new();

        // Iterate through each group in the dictionary
        foreach (var nodeTriangleGroup in nodeTriangleGroups)
        {
            Block newBlock = new();

            // Set the block center to be the key of the group
            newBlock.blockCenter = nodeTriangleGroup.Key;

            // Match the block center with a heatmap key
            if (nodeHeatMapPairs.ContainsKey(nodeTriangleGroup.Key))
            {
                newBlock.heatValue = nodeHeatMapPairs[nodeTriangleGroup.Key];
            }
            else
            {
                Debug.LogWarning("Block center does not have a corresponding heatmap value");
            }

            blocks.Add(newBlock);

            // Iterate through each node that surrounds their main node
            for (int i = 0; i < nodeTriangleGroup.Value.Count - 1; i++)
            {
                Vector3 nodeA, nodeB;

                nodeA = nodeTriangleGroup.Value[i];
                nodeB = nodeTriangleGroup.Value[i + 1];

                GameObject roadObject = null;
                Vector3 direction = (nodeB - nodeA).normalized;

                // Check to see if a road has already been generated between the two nodes
                if (!roadVerticesGOGroups.Keys.ToList().Contains(nodeA + nodeB))
                {
                    // Create a road object between the two nodes
                    float distance = Vector3.Distance(nodeA, nodeB);

                    Vector3 spawnLocation = nodeA + (direction * (distance / 2));

                    // Set the rotation of the road to be the same as the direction between the two nodes
                    Vector3 rotation = new Vector3(0, Vector3.SignedAngle(Vector3.forward, direction, Vector3.up), 0);

                    //// Ensure all road rotations are positive to keep building rotations consistent
                    //if (rotation.y < 0)
                    //{
                    //    rotation = new Vector3(rotation.x, rotation.y + 180, rotation.z);
                    //}   

                    roadObject = Instantiate(roadPrefab, spawnLocation, Quaternion.Euler(rotation), parent: roadParent);

                    // Set the tiling of the road texture to be the same as the distance between the two nodes
                     
                    roadObject.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(1, distance);                    

                    Vector3 currScale = roadObject.transform.localScale;
                    roadObject.transform.localScale = new Vector3(currScale.x, currScale.y, distance);

                    roadVerticesGOGroups.Add(nodeA + nodeB, roadObject);
                    roadColliders.Add(roadObject.GetComponent<BoxCollider>());

                    // Create a road Cylinder at both ends of the road
                    GameObject roadCylinderStart = Instantiate(roadCylinderPrefab, nodeA, Quaternion.identity, roadParent);
                    roadColliders.Add(roadCylinderStart.GetComponent<BoxCollider>());

                    GameObject roadCylinderEnd = Instantiate(roadCylinderPrefab, nodeB, Quaternion.identity, roadParent);
                    roadColliders.Add(roadCylinderEnd.GetComponent<BoxCollider>());

                    Vector3 normal = new Vector3(-direction.z, 0, direction.x);

                    int lampPostCount = Mathf.FloorToInt((distance / lampPostSpacing) - lampPostNodeDistance);
                    Vector3 currentPosition = nodeA + (normal * lampPostNodeDistance);

                    for (int j = 0; j < lampPostCount; j++)
                    {
                        currentPosition += direction * lampPostSpacing;

                        // Create lamp post at the center of the road
                        GameObject lampPost = Instantiate(lampPostPrefab, currentPosition + (normal * lampPostRoadDistance), Quaternion.Euler(rotation), roadParent);
                        lampObjects.Add(lampPost);
                    }
                }

                // Otherwise, retrieve the road object from the dictionary
                else
                {
                    roadObject = roadVerticesGOGroups[nodeA + nodeB];
                }

                // If the road object is null, log an error
                if (roadObject == null)
                {
                    Debug.LogError("Road object is still null!");
                }

                // Add the road to the block             
                Road newRoad = new(nodeA, nodeB, direction, roadObject);
                newBlock.AddRoad(newRoad);
            }
        }
    }

    private IEnumerator GenerateBlockBuildings()
    {
        if (blocks == null)
        {
            Debug.LogError("Blocks have not been generated yet");
            yield break;
        }

        blockObjectHeightPairs = new();

        // Start a coroutine to generate plots along roads for each block
        yield return StartCoroutine(GeneratePlotsAndBuildings());

        yield return new WaitForSeconds(0.5f);

        if (GenerateBuildings)
        {
            GenerateBuildingsOnPlots();
        }
    }

    private IEnumerator GeneratePlotsAndBuildings()
    {
        foreach (Block block in blocks)
        {
            foreach (var road in block.Roads)
            {
                // Generate plots along the road and wait for it to finish
                StartCoroutine(GeneratePlotsAlongRoad(road.start, road.end, road.direction, block));
            }
        }

        yield break;
    }

    /// <summary>
    /// Creates "Plots" along the road, whose areas will determine the type and size of the building that will be generated there
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <param name="direction"></param>
    /// <param name="parentBock"></param>
    /// <returns></returns>
    private IEnumerator GeneratePlotsAlongRoad(Vector3 nodeA, Vector3 nodeB, Vector3 direction, Block parentBock)
    {
        float distance = Vector3.Distance(nodeA, nodeB);

        // Get the normal of the direction vector
        Vector3 normal = new Vector3(-direction.z, 0, direction.x);

        // Set the rotation of the road to be the same as the direction between the two nodes
        Vector3 rotation = new Vector3(0, Vector3.SignedAngle(Vector3.forward, direction, Vector3.up), 0);

        Vector3 plotRotation = Vector3.zero;

        // Use dot product to determine the rotation of the plot
        if (Vector3.Dot(normal, Vector3.forward) > 0)
        {
            plotRotation = new Vector3(0, 90, 0);
        }
        else
        {
            plotRotation = new Vector3(0, -90, 0);
        }
                
        plotRotation += rotation;

        Vector3 startPoint = nodeA + direction;

        float currentDistance = 0;

        while (currentDistance < distance)
        {
            float buildingScale = Mathf.Lerp(buildingScaleMax, buildingScaleMin, (parentBock.heatValue / nodeHeatMapPairs.Values.Max()) * buildingScaleHeatMapMultiplier);
            float buildingHeight = Mathf.Lerp(heatMapHeightPeak, heatMapHeightLow, (parentBock.heatValue / nodeHeatMapPairs.Values.Max()) * heatmapIntensity);

            //// Add some variation to the building scale and height
            buildingScale *= Random.Range(buildingScaleModifierMin, buildingScaleModifierMax);
            //buildingHeight *= Random.Range(buildingHeightModifierMin, buildingHeightModifierMax);

            // Calculate the spawn position
            Vector3 spawnPosition = startPoint + (direction * currentDistance);

            // Calculate the perpendicular offset based on the road width and building size
            float halfRoadWidth = roadWidth / 2.0f;
            float buildingOffset = (1 * buildingScale) / 2.0f;

            // Calculate the offset from the road center
            Vector3 sideOffset = normal * (halfRoadWidth + buildingOffset);

            Vector3 blockCenter = parentBock.blockCenter;

            // Ensure the building is spawned on the correct side of the road, towards the block center
            if (Vector3.Dot(spawnPosition - blockCenter, normal) < 0)
            {
                sideOffset *= -1;
            }

            // Offset the spawn position
            Vector3 finalSpawnPosition = ((spawnPosition + sideOffset));

            GameObject newPlot = Instantiate(plotPrefab, finalSpawnPosition, Quaternion.identity, plotParent);
            newPlot.transform.localScale = new Vector3(1 * buildingScale, 0.1f, 1 * buildingScale);

            newPlot.transform.rotation = Quaternion.Euler(0, plotRotation.y, 0);

            yield return new WaitForFixedUpdate();

            currentDistance += spawnDistance * buildingScale;

            BoxCollider plotCollider = newPlot.GetComponent<BoxCollider>();

            // if the new building overlaps with any road, destroy it
            if (OverlapsWithRoad(plotCollider))
            {
                if (buildingScale > buildingScaleMin)
                {
                    float newScaling = 0.5f * buildingScale;

                    // If the new scaling is less than the minimum building scale, set it to 0.5f
                    if (newScaling < buildingScaleMin)
                    {
                        newScaling = buildingScaleMin;
                    }

                    newPlot.transform.localScale = new Vector3(newScaling, 0.1f, newScaling);
                }
                else
                {
                    Destroy(newPlot);
                    continue;
                }

                // Move the plot away from the road
                newPlot.transform.position += new Vector3(1, 0, 1);

                // Let the collider adjust to the new scale
                yield return new WaitForFixedUpdate();

                if (OverlapsWithRoad(plotCollider))
                {
                    if (destoryRoadOverlaps)
                    {
                        Destroy(newPlot);
                        continue;
                    }
                }
            }

            if (OverlapsWithBuilding(plotCollider))
            {
                if (buildingScale > buildingScaleMin)
                {
                    float newScaling = 0.5f * buildingScale;

                    // If the new scaling is less than the minimum building scale, set it to 0.5f
                    if (newScaling < buildingScaleMin)
                    {
                        newScaling = buildingScaleMin;
                    }

                    newPlot.transform.localScale = new Vector3(newScaling, 0.1f, newScaling);
                }
                else
                {
                    Destroy(newPlot);
                    continue;
                }

                // Let the collider adjust to the new scale
                yield return new WaitForFixedUpdate();

                if (OverlapsWithBuilding(plotCollider))
                {
                    if (destroyBuildingOverlaps)
                    {
                        Destroy(newPlot);
                        continue;
                    }
                }
            }

            newPlot.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = Mathf.RoundToInt(buildingHeight).ToString();

            blockObjectHeightPairs.Add(newPlot, Mathf.RoundToInt(buildingHeight));
            plotColliders.Add(plotCollider);
            parentBock.Buildings.Add(plotCollider);

            //GameObject newBuilding = Instantiate(GetBestBuilding(buildingScale).gameObject, finalSpawnPosition, Quaternion.identity, buildingParent);

            //GenerateBuilding generatedBuilding = newBuilding.GetComponent<GenerateBuilding>();

            //// Scale the building height
            //Vector3 scale = new Vector3(newBuilding.transform.localScale.x * buildingScale, buildingHeight, newBuilding.transform.localScale.z * buildingScale);

            //// Move the building up by half its height
            //if (generatedBuilding is GenerateOfficeBuilding1)
            //newBuilding.transform.position += new Vector3(0, buildingHeight / 2, 0);

            //generatedBuilding.SetScaling(scale);

            //generatedBuilding.Generate();

            //generatedBuilding.SetRotation(rotation);

        }

        yield break;
    }

    private void GenerateBuildingsOnPlots()
    {
        // Instantiate the "preffered building" dictionary
        BuildingGeneratorPreferencePairs = new();
        foreach (var generator in BuildingGenerators)
        {
            BuildingGeneratorPreferencePairs.Add(generator, BuildingGeneratorScalePreferences[BuildingGenerators.IndexOf(generator)]);
        }

        if (blockObjectHeightPairs.Count == 0)
        {
            Debug.LogError("No plots have been generated yet");
        }

        foreach(var plot in blockObjectHeightPairs)
        {
            if (plot.Key == null)
            {
                Debug.LogWarning("Plot is null");
                continue;
            }

            GameObject plotObject = plot.Key;
            int buildingHeight = plot.Value;
            Vector3 rotation = plotObject.transform.eulerAngles;

            if (plotObject == null)
            {
                Debug.LogError("Plot is null");
                continue;
            }

           
            // Get the area of the plot (all blocks are square)
            float plotArea = plotObject.transform.localScale.x;

            GenerateBuilding building = Instantiate(GetBestBuilding(plotArea), plotObject.transform.position, Quaternion.identity, buildingParent);

            building.transform.position = plotObject.transform.position;
            building.Generate(buildingHeight);
            building.SetScaling(new Vector3(plotArea, plotArea, plotArea));
            building.SetRotation(rotation);
        }
    }

    private bool OverlapsWithRoad(BoxCollider boxCollider)
    {
        if (roadColliders.Count == 0)
        {
            Debug.LogError("No road colliders have been generated yet");
        }

        foreach (var road in roadColliders)
        {
            if (Vector3.Distance(road.transform.position, boxCollider.transform.position) < roadDistanceCheckRange)
            {
                if (Utils.CollidersOverlap(road, boxCollider))
                {
                    return true;
                }
            }
        }

        return false;

        // return roadColliders.Any(roadCollider => Utils.CollidersOverlap(roadCollider, boxCollider));
    }

    private bool OverlapsWithBuilding(BoxCollider boxCollider)
    {
        if (plotColliders.Count == 0)
        {
            Debug.LogWarning("No building colliders have been generated yet");
        }

        foreach (BoxCollider buildingCollider in plotColliders)
        {
            if (buildingCollider == boxCollider)
            {
                continue;
            }

            if (Vector2.Distance(new Vector3(buildingCollider.transform.position.x, 0, buildingCollider.transform.position.z), new Vector3(boxCollider.transform.position.x, 0, boxCollider.transform.position.z)) < buildingDistanceCheckRange)
            {
                if (Utils.CollidersOverlap(buildingCollider, boxCollider))
                {
                    return true;
                }
            }
        }

        return false;

        // Ensure the collider doesnt check collision with itself

        foreach (BoxCollider buildingCollider in plotColliders)
        {
            if (buildingCollider == boxCollider)
            {
                continue;
            }

            if (Utils.CollidersOverlap(buildingCollider, boxCollider))
            {
                return true;
            }
        }

        return false;
    }

    private class Road
    {
        public Vector3 start, end;
        public Vector3 direction;
        public GameObject roadObject;
        public BoxCollider roadCollider;

        public Road(Vector3 start, Vector3 end, Vector3 direction, GameObject roadObject)
        {
            this.start = start;
            this.end = end;
            this.direction = direction;
            this.roadObject = roadObject;

            if (!roadObject.TryGetComponent(out roadCollider))
            {
                Debug.LogError("Road object does not have a BoxCollider component");
            }
        }
    }

    private class Block
    {
        public Vector3 blockCenter;
        public List<Road> Roads = new();
        public List<BoxCollider> Buildings = new();

        // Distance from the city center, determines building size
        public float heatValue;

        public void AddRoad(Road road)
        {
            foreach (Road existingRoad in Roads)
            {
                if (existingRoad.start + existingRoad.end == road.start + road.end)
                {
                    Debug.LogWarning("Road already exists in block");
                    return;
                }
            }

            Roads.Add(road);
        }

        public bool OverlapsWithElement(BoxCollider collider)
        {
            foreach (BoxCollider building in Buildings)
            {
                if (Utils.CollidersOverlap(building, collider))
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Get the most suitable building for the given area
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    private GenerateBuilding GetBestBuilding(float area)
    {
        float closestArea = float.MaxValue;
        List<int> suitableBuildings = new();

        for (int i = 0; i < BuildingGeneratorPreferencePairs.Count; i++)
        {
            if (Mathf.Abs(BuildingGeneratorPreferencePairs.ElementAt(i).Value - area) < closestArea)
            {
                closestArea = Mathf.Abs(BuildingGeneratorPreferencePairs.ElementAt(i).Value - area);
                suitableBuildings.Clear();
                suitableBuildings.Add(i);
            }
            else if (Mathf.Abs(BuildingGeneratorPreferencePairs.ElementAt(i).Value - area) == closestArea)
            {
                suitableBuildings.Add(i);
            }
        }

        // If there are multiple suitable buildings, choose one at random
        int bestIndex = suitableBuildings[Random.Range(0, suitableBuildings.Count)];
        return BuildingGeneratorPreferencePairs.ElementAt(bestIndex).Key;
    }

    //private void OnDrawGizmos()
    //{
    //    if (blocks == null)
    //    {
    //        return;
    //    }

    //    foreach (Block block in blocks)
    //    {
    //        foreach (Road road in block.Roads)
    //        {
    //            Bounds bound = road.roadCollider.bounds;

    //            Gizmos.color = Color.red;
    //            Gizmos.DrawWireCube(bound.center, bound.size);
    //        }
    //    }
    //}
}
