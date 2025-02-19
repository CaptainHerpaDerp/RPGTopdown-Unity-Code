using System.Collections.Generic;
using UnityEngine;

public class GenerateOfficeBuilding1 : GenerateBuilding
{
    [Header("The base of the building")]
    [SerializeField] private GameObject buildingBase;
    [SerializeField] private float baseScale;

    [Header("The front window")]
    [SerializeField] private GameObject frontWindow;

    [Header("The side window")]
    [SerializeField] private GameObject sideWindow;

    [Header("The top piece")]
    [SerializeField] private GameObject topPiece;
    [SerializeField] private Vector2 topPieceNeededLW;

    [SerializeField] private GameObject pillarGroup;

    [SerializeField] private float lengthScale;
    [SerializeField] private float widthScale;
    [SerializeField] private float heightScale;

    private Vector3 frontWindowScale, sideWindowScale;

    [SerializeField] private Vector2 frontWindowTextureScaleBase, sideWindowTextureScaleBase;

    public override void SetScaling(Vector3 scaling)
    {
        lengthScale = scaling.x;
        heightScale = scaling.y;
        widthScale = scaling.z;
    }

    public override void Generate(int buildingHeight)
    {
        ClearBuilding();

        this.buildingHeight = buildingHeight;

        frontWindowScale = frontWindow.transform.localScale;
        sideWindowScale = sideWindow.transform.localScale;

        addedBlocks = new List<GameObject>();

        GameObject baseBlock = Instantiate(buildingBase, transform.position, Quaternion.identity, transform);
        addedBlocks.Add(baseBlock);
        baseBlock.transform.localScale =  new Vector3(lengthScale, heightScale, widthScale);

        GameObject windowFront = Instantiate(frontWindow, transform.position, Quaternion.identity, transform);
        addedBlocks.Add(windowFront);
        windowFront.transform.localScale = new Vector3(frontWindowScale.x * lengthScale, frontWindowScale.y * heightScale, frontWindowScale.z * widthScale);
        windowFront.GetComponent<Renderer>().sharedMaterial.mainTextureScale = new Vector2(frontWindowTextureScaleBase.x * lengthScale, frontWindowTextureScaleBase.y * heightScale);

        GameObject windowSide = Instantiate(sideWindow, transform.position, Quaternion.identity, transform);
        addedBlocks.Add(windowSide);
        windowSide.transform.localScale = new Vector3(sideWindowScale.x * lengthScale, sideWindowScale.y * heightScale, sideWindowScale.z * widthScale);
        windowSide.GetComponent<Renderer>().sharedMaterial.mainTextureScale = new Vector2(sideWindowTextureScaleBase.x * widthScale, sideWindowTextureScaleBase.y * heightScale);

        GameObject pillarGroup1 = Instantiate(pillarGroup, transform.position + new Vector3(0, 0, -(baseScale/2) * widthScale), Quaternion.identity, transform);
        addedBlocks.Add(pillarGroup1);
        pillarGroup1.transform.localScale = new Vector3(1 * lengthScale, heightScale, 1);

        GameObject pillarGroup2 = Instantiate(pillarGroup, transform.position + new Vector3(0, 0, (baseScale / 2) * widthScale), Quaternion.identity, transform);
        addedBlocks.Add(pillarGroup2);
        pillarGroup2.transform.localScale = new Vector3(1 * lengthScale, heightScale, 1);

        if (topPieceNeededLW.x == lengthScale && topPieceNeededLW.y == widthScale)
        {
            GameObject topPieceBlock = Instantiate(topPiece, transform.position + new Vector3(0, 0 + (heightScale - 1) / 2, 0), Quaternion.identity, transform);
            addedBlocks.Add(topPieceBlock);
        }
    }

    public override void SetRotation(Vector3 rotation)
    {
        transform.rotation = Quaternion.Euler(rotation);

        //foreach (GameObject block in addedBlocks)
        //{
        //    block.transform.rotation = Quaternion.Euler(rotation);
        //}
    }
}
