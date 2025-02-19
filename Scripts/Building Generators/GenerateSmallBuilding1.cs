using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSmallBuilding1 : GenerateBuilding
{

    [Header("The base of the building - first block that will always spawn")]
    [SerializeField] private GameObject baseBlock;
    [SerializeField] private float baseBlockHeight;

    [Header("Window piece - stack block")]
    [SerializeField] private GameObject segmentBlock;
    [SerializeField] private float segmentBlockHeight;

    [Header("Corner Variation Groups")]
    [SerializeField] private List<GameObject> cornerBlocks;

    [Header("Top block")]
    [SerializeField] private GameObject topBlock;

    [Header("Roof prop variants")]
    [SerializeField] private List<GameObject> roofPropVariants;

    public override void Generate(int buildingHeight)
    {
        PreserveTransform();

        this.buildingHeight = buildingHeight;

        ClearBuilding();

        transform.position = new Vector3(transform.position.x, heightPivot, transform.position.z);

        for (int i = 0; i < buildingHeight; i++)
        {
            GameObject segmentPiece = Instantiate(segmentBlock, transform.position + new Vector3(0, i * segmentBlockHeight, 0), Quaternion.identity, transform);
            addedBlocks.Add(segmentPiece);

            GameObject cornerPiece = Instantiate(cornerBlocks[Random.Range(0, cornerBlocks.Count)], transform.position + new Vector3(0, i * segmentBlockHeight, 0), Quaternion.identity, segmentPiece.transform);
            addedBlocks.Add(cornerPiece);

            if (i == buildingHeight - 1)
            {
                GameObject topPiece = Instantiate(topBlock, transform.position + new Vector3(0, (i * segmentBlockHeight) + (0.5f), 0), Quaternion.identity, transform);
                addedBlocks.Add(topPiece);

                GameObject roofProp = Instantiate(roofPropVariants[Random.Range(0, roofPropVariants.Count)], transform.position + new Vector3(0, (i * segmentBlockHeight) + (0.5f), 0), Quaternion.identity, topPiece.transform);
                addedBlocks.Add(roofProp);
            }
        }

        ReapplyTransform();
    }

    public override void SetRotation(Vector3 rotation)
    {
        transform.rotation = Quaternion.Euler(rotation);
    }


    public override void SetScaling(Vector3 scaling)
    {
        transform.localScale = new Vector3(scaling.x, scaling.x, scaling.x);
        buildingHeight = Mathf.RoundToInt(scaling.y);
    }
}
