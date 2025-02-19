using System.Collections.Generic;
using UnityEngine;

public abstract class GenerateBuilding : MonoBehaviour
{
    protected List<GameObject> addedBlocks;
    [SerializeField] protected int buildingHeight;

    [SerializeField] protected bool flipRandomly;

    public int BuildingHeight { get => buildingHeight; set => buildingHeight = value; }

    [Header("The value that will be added initially")]
    [SerializeField] protected float heightPivot;

    [Header("Scales the building by the set amount when scaling normally")]
    [SerializeField] protected float scaleModifier = 1;

    protected Vector3 savedScale;
    protected Vector3 savedRotation;

    public abstract void SetScaling(Vector3 scaling);

    public abstract void Generate(int buildingHeight);

    public abstract void SetRotation(Vector3 rotation);

    public virtual void RotateTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    protected void ReapplyTransform()
    {
        transform.localScale = savedScale;
        transform.rotation = Quaternion.Euler(savedRotation);
    }
    protected void PreserveTransform()
    {
        savedScale = transform.localScale;
        transform.localScale = new Vector3(1, 1, 1);

        savedRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.identity;
    }

    protected void ClearBuilding()
    {
        if (addedBlocks != null)
        {
            foreach (GameObject block in addedBlocks)
            {
                DestroyImmediate(block);
            }

            addedBlocks.Clear();
        }

        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        addedBlocks = new List<GameObject>();
    }
}
