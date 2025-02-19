using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    public static ControlPanel Instance;

    [Header("Main Source of Light")]
    [SerializeField] private Light mainDirectionalLight;

    [Header("Light Brighness Slider component")]
    [SerializeField] private Slider lightIntensitySlider;

    [Header("Light Angle Slider component")]
    [SerializeField] private Slider lightAngleSlider;

    [Header("Fog Toggle Component")]
    [SerializeField] private Toggle fogToggle;
    [Header("Fog Object")]
    [SerializeField] private GameObject fogObject;

    [Header("Rain Toggle Component")]
    [SerializeField] private Toggle rainToggle;
    [Header("Rain Object")]
    [SerializeField] private GameObject rainObject;

    [Header("Building Toggle Component")]
    [SerializeField] private Toggle buildingToggle;
    [Header("Building Parent")]
    [SerializeField] private GameObject buildingParent;

    [Header("Plot Toggle Component")]
    [SerializeField] private Toggle plotToggle;
    public Toggle PlotToggle => plotToggle;
    [Header("Plot Parent")]
    [SerializeField] private GameObject plotParent;

    [Header("Quality Slider")]
    [SerializeField] private Slider gameQualitySlider;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        lightAngleSlider.onValueChanged.AddListener(delegate { ChangeLightAngle(); });
        lightAngleSlider.value = mainDirectionalLight.transform.rotation.eulerAngles.x / 180;

        lightIntensitySlider.onValueChanged.AddListener(delegate { ChangeLightIntensity(); });
        lightIntensitySlider.value = mainDirectionalLight.intensity;

        fogToggle.onValueChanged.AddListener(delegate { ToggleFog(); });
        fogToggle.isOn = fogObject.activeSelf;

        rainToggle.onValueChanged.AddListener(delegate { ToggleRain(); });
        rainToggle.isOn = rainObject.activeSelf;

        if (buildingToggle != null && buildingParent != null)
        {
            buildingToggle.onValueChanged.AddListener(delegate { ToggleBuilding(); });
            buildingToggle.isOn = buildingParent.activeInHierarchy;
        }

        if (plotToggle != null && plotParent != null)
        {
            plotToggle.onValueChanged.AddListener(delegate { TogglePlot(); });

            // Disable all the plot objects initially
            foreach (Transform child in plotParent.transform)
            {
                child.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        gameQualitySlider.onValueChanged.AddListener(delegate { SetQuality((int)gameQualitySlider.value); });
    }

    private void ChangeLightAngle()
    {
        // Rotate the light based on the slider value, limit it to 0-180 so that the light doesn't go below the horizon
        mainDirectionalLight.transform.rotation = Quaternion.Euler(new Vector3(lightAngleSlider.value * 180, 0, 0));
    }

    private void ChangeLightIntensity()
    {
        // Change the light intensity based on the slider value
        mainDirectionalLight.intensity = lightIntensitySlider.value;
    }  

    public void ToggleFog()
    {
        // Toggle the fog object based on the toggle value
        fogObject.SetActive(fogToggle.isOn);
    }

    public void ToggleRain()
    {
        // Toggle the rain object based on the toggle value
        rainObject.SetActive(rainToggle.isOn);
    }

    public void ToggleBuilding()
    {
        // Toggle the building parent object based on the toggle value
        buildingParent.SetActive(buildingToggle.isOn);
    }

    public void TogglePlot()
    {
        // Toggle the plot parent object based on the toggle value
        foreach (Transform child in plotParent.transform)
        {
            child.GetComponent<MeshRenderer>().enabled = plotToggle.isOn;
        }
    }

    public void SetQuality(int quality)
    {
        if (quality == 1)
        {
            QualitySettings.SetQualityLevel(4);
        }
        else
        {
            QualitySettings.SetQualityLevel(1);
        }
    }
}
