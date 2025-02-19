using System.Collections;
using UnityEngine;

public class EmissionCurve : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float flashSpeed = 1f;

    private float redValue = 0f;

    [SerializeField] private float emissionIntensity = 1f;

    private void Start()
    {
        // Start the coroutine for pulsating emission color
        StartCoroutine(DoPulsateEmission());
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        // Start the coroutine for pulsating emission color
        StartCoroutine(DoPulsateEmission());
    }

    // Raise the red value of the emission color
    private IEnumerator DoPulsateEmission()
    {
        //Disable the emission of the material
        meshRenderer.material.DisableKeyword("_EMISSION");

        yield return new WaitForSeconds(1);

        //Enable the emission of the material
        meshRenderer.material.EnableKeyword("_EMISSION");

        while (true)
        {
            redValue = Mathf.PingPong(Time.time * flashSpeed, 1);
            meshRenderer.material.SetVector("_EmissionColor", new Vector4(redValue, 0f, 0f, 1f) * emissionIntensity);
     
            yield return null;
        }
    }
}
