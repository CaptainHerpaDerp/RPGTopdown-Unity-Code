using System.Collections.Generic;
using UnityEngine;

public class GenerateOfficeBuilding2 : GenerateBuilding
{
    [Header("The base of the building")]
    [SerializeField] private GameObject buildingBase;
    [SerializeField] private float baseHeight;

    [Header("The block that will be stacked")]
    [SerializeField] private GameObject floorBlock;
    [SerializeField] private float floorBlockHeight;

    [Header("The top piece")]
    [SerializeField] private GameObject topPiece;

    [Header("Roof prop variants")]
    [SerializeField] private List<GameObject> roofPropVariants;

    public override void Generate(int buildingHeight)
    {
        this.buildingHeight = buildingHeight;

        PreserveTransform();

        ClearBuilding();

        GameObject baseBlock = Instantiate(buildingBase, transform.position, Quaternion.identity, transform);
        addedBlocks.Add(baseBlock);

        baseBlock.GetComponentInChildren<CustomMeshTest>().Generate();

        for (int i = 0; i < buildingHeight; i++)
        {
            GameObject block = Instantiate(floorBlock, transform.position + new Vector3(0, baseHeight + (i * floorBlockHeight), 0), Quaternion.identity, transform);
            addedBlocks.Add(block);
        }

        GameObject topBlock = Instantiate(topPiece, transform.position + new Vector3(0, buildingHeight * floorBlockHeight, 0), Quaternion.identity, transform);
        addedBlocks.Add(topBlock);

        GameObject roofPropVariant = Instantiate(roofPropVariants[Random.Range(0, roofPropVariants.Count)], transform.position + new Vector3(0, (buildingHeight * floorBlockHeight) -0.02f, 0), Quaternion.identity, topBlock.transform);
        addedBlocks.Add(roofPropVariant);

        ReapplyTransform();
    }

    public override void SetRotation(Vector3 rotation)
    {
        transform.rotation = Quaternion.Euler(rotation);

        if (flipRandomly)
        {
            if (Random.Range(0, 2) == 0)
            {
                transform.Rotate(0, 180, 0);
            }
        }
    }

    public override void SetScaling(Vector3 scaling)
    {
        transform.localScale = new Vector3(scaling.x * scaleModifier, scaling.y * scaleModifier, scaling.z * scaleModifier);
        transform.position = new Vector3(transform.position.x, transform.localScale.y / 2, transform.position.z);
    }
}
