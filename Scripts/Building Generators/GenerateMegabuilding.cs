using System.Collections.Generic;
using UnityEngine;

public class GenerateMegabuilding : GenerateBuilding
{
    [Header("The bottom of the building - entrance")]
    [SerializeField] private GameObject buildingBase;
    [SerializeField] private float baseHeight;

    [Header("The default building block that is created above the base")]
    [SerializeField] private GameObject buildingCenter;
    [SerializeField] private float centerHeight;

    [Header("The block that divides each floor")]
    [SerializeField] private GameObject dividerPiece;
    [SerializeField] private float dividerPieceHeight;

    [Header("Pieces that can be created on the building's front")]
    [SerializeField] private List<GameObject> frontVariationPieces;

    [Header("Pieces that can be created on the building's side")]
    [SerializeField] private List<GameObject> sideVariationPieces;

    [Header("Pieces that can be created on the building's corner")]
    [SerializeField] private List<GameObject> cornerVariationPieces;

    private GameObject baseObject;

    [Header("The maximum amount of segments the building can have")]
    [SerializeField] private int maxBuildingHeight = 5;

    [SerializeField] private Vector2 baseScaling;

    public override void Generate(int height)
    {
        // Clear the building if it already exists

        PreserveTransform();

        buildingHeight = height;

        ClearBuilding();

        if (buildingHeight > maxBuildingHeight)
        {
            buildingHeight = maxBuildingHeight;
        }

        float currentY = 0;

        // Add the base
        baseObject = Instantiate(buildingBase, transform.position, Quaternion.identity, transform);
        addedBlocks.Add(baseObject);

        // Add the first height
        currentY += (baseHeight / 2) + (centerHeight / 2);


        for (int i = 0; i < buildingHeight; i++)
        {
            // Add the center
            GameObject newCenterObject = Instantiate(buildingCenter, transform.position + (transform.up * currentY * transform.localScale.y), Quaternion.identity, transform);
            addedBlocks.Add(newCenterObject);

            // Randomly add a front piece
            if (Random.Range(0, 2) == 0)
            {
                GameObject frontPieceVariation = frontVariationPieces[Random.Range(0, frontVariationPieces.Count)];
                GameObject frontPiece = Instantiate(frontPieceVariation, frontPieceVariation.transform.position + transform.position + (transform.up * currentY * transform.localScale.y), Quaternion.identity, parent: newCenterObject.transform);
                addedBlocks.Add(frontPiece);
            }

            // Randomly add a side piece
            if (Random.Range(0, 2) == 0)
            {
                GameObject sidePieceVariation = sideVariationPieces[Random.Range(0, sideVariationPieces.Count)];
                GameObject sidePiece = Instantiate(sidePieceVariation, sidePieceVariation.transform.position + transform.position + (transform.up * currentY * transform.localScale.y), Quaternion.identity, parent: newCenterObject.transform);
                addedBlocks.Add(sidePiece);
            }

            GameObject newDivier = Instantiate(dividerPiece, transform.position + (transform.up * (currentY + (centerHeight / 2) + (dividerPieceHeight / 2)) * transform.localScale.y), Quaternion.identity, transform);
            currentY += centerHeight;

            addedBlocks.Add(newDivier);

            currentY += dividerPieceHeight;
        }

        float minScale;
        
        if (baseScaling.x < baseScaling.y)
        {
            minScale = baseScaling.x;
        }
        else
        {
            minScale = baseScaling.y;
        }

        if (minScale > 0)
        transform.localScale = new Vector3(minScale, minScale, minScale);

        ReapplyTransform();
    }

    public override void SetScaling(Vector3 scaling)
    {
        transform.localScale = new Vector3(scaling.x, scaling.y, scaling.z);
    }

    public override void SetRotation(Vector3 rotation)
    {
        transform.rotation = Quaternion.Euler(rotation);
        foreach (Transform child in transform)
        {
            child.localRotation = Quaternion.identity;
        }
    }
}
