

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateSkyscraper1 : GenerateBuilding
{
    [Header("The base of the building")]
    [SerializeField] private GameObject buildingBase;
    [SerializeField] private float baseHeight;

    [Header("The block that will be stacked")]
    [SerializeField] private List<GameObject> floorBlockPrefabs;
    [SerializeField] private float floorBlockHeight;
    private GameObject floorBlock;

    [Header("The top piece")]
    [SerializeField] private GameObject topPiece;

    [Header("Overhang corner piece")]
    [SerializeField] private GameObject overhangCornerPiece;

    [Header("Overhang side piece")]
    [SerializeField] private GameObject overhangSidePiece;

    [Header("Spawn Chance (higher = less frequent)")]
    [SerializeField] private int overhangChance;

    [Header("An ad that can be spawned onto the front of the building")]
    [SerializeField] private List<GameObject> NarrowAds;

    [Header("Advert Chance (higher = less frequent)")]
    [SerializeField] private int advertChance;

    [Header("Possible roof prop configurations")]
    [SerializeField] private List<GameObject> roofArrangements;

    public override void Generate(int buildingHeight)
    {
        floorBlock = floorBlockPrefabs[Random.Range(0, floorBlockPrefabs.Count)];

        PreserveTransform();

        this.buildingHeight = buildingHeight;

        ClearBuilding();

        //GameObject baseBlock = Instantiate(buildingBase, transform.position, Quaternion.identity, transform);
        //addedBlocks.Add(baseBlock);

        for (int i = 0; i < buildingHeight; i++)
        {
            GameObject block = Instantiate(floorBlock, transform.position + new Vector3(0, baseHeight + (i * floorBlockHeight), 0), Quaternion.identity, transform);
            addedBlocks.Add(block);

            // Try for a corner piece on each side of the building
            for (int y = 0; y < 4; y++)
            {
                if (Random.Range(0, overhangChance) == 0)
                {
                    GameObject cornerPiece = Instantiate(overhangCornerPiece, transform.position + new Vector3(0, baseHeight + (i * floorBlockHeight), 0), Quaternion.identity, block.transform);
                    addedBlocks.Add(cornerPiece);

                    float randomRot = y * 90;
                    cornerPiece.transform.Rotate(0, randomRot, 0);
                }
            }

            for (int x = 0; x < 4; x++)
            {
                if (Random.Range(0, overhangChance) == 0)
                {
                    GameObject sidePiece = Instantiate(overhangSidePiece, transform.position + new Vector3(0, baseHeight + (i * floorBlockHeight), 0), Quaternion.identity, block.transform);
                    addedBlocks.Add(sidePiece);

                    float randomRot = x * 90;
                    sidePiece.transform.Rotate(0, randomRot, 0);
                }
            }

            // Try for a narrow ad on the narrow sides of the building
            for (int j = 0; j < 2; j++)
            {
                if (Random.Range(0, advertChance) == 0)
                {
                    GameObject advertPiece = Instantiate(NarrowAds[Random.Range(0, NarrowAds.Count)], transform.position + new Vector3(0, baseHeight + (i * floorBlockHeight), 0), Quaternion.identity, block.transform);
                    addedBlocks.Add(advertPiece);

                    float randomRot = j * 180;
                    advertPiece.transform.Rotate(0, randomRot, 0);
                }
            }
        }

        GameObject topBlock = Instantiate(topPiece, transform.position + new Vector3(0, buildingHeight * floorBlockHeight - 1, 0), Quaternion.identity, transform);
        addedBlocks.Add(topBlock);

        GameObject roofArrangement = Instantiate(roofArrangements[Random.Range(0, roofArrangements.Count)], transform.position + new Vector3(0, buildingHeight * floorBlockHeight - 1, 0), Quaternion.identity, transform);
        addedBlocks.Add(roofArrangement);

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
