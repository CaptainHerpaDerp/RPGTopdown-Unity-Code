using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningBlock : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1f;

    private Coroutine spinCoroutine;

    [SerializeField] private List<Material> possibleMaterial;

    void OnEnable()
    {
        // Set a random material
        GetComponent<MeshRenderer>().material = possibleMaterial[Random.Range(0, possibleMaterial.Count)];

        if (spinCoroutine == null)
            spinCoroutine = StartCoroutine(SpinBlock());
    }

    void OnDisable()
    {
        if (spinCoroutine != null)
        {
            StopCoroutine(spinCoroutine);
            spinCoroutine = null;
        }
    }

    private IEnumerator SpinBlock()
    {
        // Set a random rotation to start with
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 180), 0);

        while (true)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            if (transform.rotation.eulerAngles.y >= 180)
                transform.rotation = Quaternion.Euler(0, 0, 0);

            yield return null; // Wait for the next frame
        }
    }
}
