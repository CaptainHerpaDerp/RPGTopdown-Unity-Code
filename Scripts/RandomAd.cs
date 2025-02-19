using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAd : MonoBehaviour
{
    // Reference the renderer
    [SerializeField] private List<MeshRenderer> renderers = new();

    [SerializeField] private List<Material> possibleMaterial = new();

    [SerializeField] private bool adChanges;

    [Header("Time between changing the ad")]    
    [SerializeField] private float adChangeTime = 5f;

    [SerializeField] private bool randomizeChangeTime;
    private float baseChangeTime;

    void Start()
    {
        baseChangeTime = adChangeTime;

        StartCoroutine(AssignRandomMaterials());

        if (adChanges)
            StartCoroutine(ChangeAds());
    }

    private void OnEnable()
    {
        // Set the material to a random material
        StartCoroutine(AssignRandomMaterials());

        if (adChanges)
            StartCoroutine(ChangeAds());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator AssignRandomMaterials()
    {
        foreach (MeshRenderer renderer in renderers)
        {
            float delay = Random.Range(0.01f, 0.1f);
            yield return new WaitForSeconds(delay);

            renderer.material = possibleMaterial[Random.Range(0, possibleMaterial.Count)];
        }

        yield return null;
    }

    private IEnumerator ChangeAds()
    {
        while(true)
        {
            if (randomizeChangeTime)
                adChangeTime = baseChangeTime + Random.Range(2, 5);

            yield return new WaitForSeconds(adChangeTime);

            StartCoroutine(AssignRandomMaterials());
        }
    }
}
