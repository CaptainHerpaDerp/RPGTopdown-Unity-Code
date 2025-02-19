using UnityEngine;

public class GenerateCylinderBuilding : GenerateBuilding
{
    [Header("The base of the building - first block that will always spawn")]
    [SerializeField] private GameObject baseBlock;
    [SerializeField] private float baseBlockHeight;

    [Header("Window piece - stack block modifier")]
    [SerializeField] private GameObject windowSegment;
    [SerializeField] private float windowSegmentHeight;

    [Header("Advert piece - stack block modifier")]
    [SerializeField] private GameObject advertSegment;
    [SerializeField] private float advertSegmentHeight;
    [SerializeField] private float advertNegativePadding;

    [SerializeField] private GameObject topBlock;

    [Header("Neon sign group - Parent with all children activated")]
    [SerializeField] private GameObject neonSignGroup;
    [Header("Sign activation chance (Less = more common)")]
    [SerializeField] private int signChance;

    [Header("Scales the set height by the given amount")]
    [SerializeField] private float heightScalingModifier;

    private float currentHeightSpacing = 0;

    private void AddToHeightSpacing(float value)
    {
        currentHeightSpacing += value;
    }

    public override void Generate(int buildingHeight)
    {
        this.buildingHeight = buildingHeight;

        currentHeightSpacing = 0;

        ClearBuilding();       

        GameObject baseBlockInstance = Instantiate(baseBlock, transform.position, Quaternion.identity, transform);
        addedBlocks.Add(baseBlockInstance);

        AddToHeightSpacing(baseBlockHeight);

        for (int i = 0; i < buildingHeight; i++)
        {
            GameObject nextBlock = null;

            if (Random.Range(0, 2) == 0)
            {
                nextBlock = Instantiate(windowSegment, transform.position + new Vector3(0, currentHeightSpacing, 0), Quaternion.identity, transform);
                AddToHeightSpacing(windowSegmentHeight); 
            }
            else
            {
                nextBlock = Instantiate(advertSegment, transform.position + new Vector3(0, currentHeightSpacing - (advertNegativePadding), 0), Quaternion.identity, transform);
                AddToHeightSpacing(advertSegmentHeight + (advertNegativePadding * 2)); 
            }

            addedBlocks.Add(nextBlock);

            // Finish off with a advert segment on the top.
            if (i == buildingHeight - 1)
            {
                nextBlock = Instantiate(advertSegment, transform.position + new Vector3(0, currentHeightSpacing - (advertNegativePadding), 0), Quaternion.identity, transform);
                AddToHeightSpacing(advertSegmentHeight + (advertNegativePadding * 2)); 
                addedBlocks.Add(nextBlock);
            }
        }

        // Sign group pre-positioned to the base block.
        GameObject signGroup = Instantiate(neonSignGroup, transform.position + new Vector3(0, 0, 0), Quaternion.identity, transform);
        addedBlocks.Add(signGroup);

        // Randomly acivate the signs based on the sign chance.
        foreach (Transform child in signGroup.transform)
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
    }

    public override void SetRotation(Vector3 rotation)
    {
        transform.rotation = Quaternion.Euler(rotation);
    }

    public override void SetScaling(Vector3 scaling)
    {
        transform.localScale = new Vector3(scaling.x, scaling.x, scaling.x);
    }
}
