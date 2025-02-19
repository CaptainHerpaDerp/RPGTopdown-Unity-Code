using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSmallBuilding2 : GenerateBuilding
{

    [Header("The base of the building - first block that will always spawn")]
    [SerializeField] private GameObject baseBlock;
    [SerializeField] private float baseBlockHeight;

    [Header("Window pieces - stack blocks")]
    [SerializeField] private List<GameObject> segmentBlocks;
    [SerializeField] private float segmentBlockHeight;

    [Header("Top block")]
    [SerializeField] private GameObject topBlock;

    [Header("Neon sign groups - Parents with all children activated")]
    [SerializeField] private GameObject baseNeonSigns;
    [SerializeField] private GameObject blockNeonSigns;
    [Header("Sign activation chance (Less = more common)")]
    [SerializeField] private int signChance;

    [Header("Roof prop variants")]
    [SerializeField] private List<GameObject> roofPropVariants;


    public override void Generate(int buildingHeight)
    {
        PreserveTransform();

        this.buildingHeight = buildingHeight;

        ClearBuilding();

        transform.position = new Vector3(transform.position.x, heightPivot, transform.position.z);

        GameObject baseInstance = Instantiate(baseBlock, transform.position, Quaternion.identity, transform);
        addedBlocks.Add(baseInstance);
        
        GameObject baseNeonSignsParent = Instantiate(baseNeonSigns, transform.position, Quaternion.identity, transform);
        addedBlocks.Add(baseNeonSignsParent);

        foreach (Transform child in baseNeonSignsParent.transform)
        {
            if (Random.Range(0, signChance) == 0)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < buildingHeight; i++)
        {
            GameObject segmentPiece = Instantiate(segmentBlocks[Random.Range(0, segmentBlocks.Count)], transform.position + new Vector3(0, i * segmentBlockHeight + baseBlockHeight, 0), Quaternion.identity, transform);
            addedBlocks.Add(segmentPiece);

            GameObject blockNeonSignsParent = Instantiate(blockNeonSigns, transform.position + new Vector3(0, i * segmentBlockHeight + baseBlockHeight, 0), Quaternion.identity, transform);
            addedBlocks.Add(blockNeonSignsParent);

            foreach (Transform child in blockNeonSignsParent.transform)
            {
                if (Random.Range(0, signChance) == 0)
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }

            if (i == buildingHeight - 1)
            {
                GameObject roofPropVariant = Instantiate(roofPropVariants[Random.Range(0, roofPropVariants.Count)], transform.position + new Vector3(0, i * segmentBlockHeight + baseBlockHeight, 0), Quaternion.identity, segmentPiece.transform);
                addedBlocks.Add(roofPropVariant);

                // GameObject topPiece = Instantiate(topBlock, transform.position + new Vector3(0, (i * segmentBlockHeight) + (0.5f), 0), Quaternion.identity, transform);
                // addedBlocks.Add(topPiece);
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
