using UnityEngine;

public class VoronoiDiagram : MonoBehaviour
{
    public Vector3Int imageDim;
    public int regionAmount;

    [SerializeField] private bool reset;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(GetDiagram(), new Rect(0, 0, imageDim.x, imageDim.y), Vector3.one * 0.5f);
    }

    void Update()
    {
        if (reset)
        {
            reset = false;
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(GetDiagram(), new Rect(0, 0, imageDim.x, imageDim.y), Vector3.one * 0.5f);
        }
    }

    Texture2D GetDiagram()
    {
        Vector3Int[] centroids = new Vector3Int[regionAmount];
        Color[] regions = new Color[regionAmount];
        for (int i = 0; i < regionAmount; i++)
        {
            centroids[i] = new Vector3Int(Random.Range(0, imageDim.x), Random.Range(0, imageDim.y));
            regions[i] = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1f);
        }

        Color[] pixelColors = new Color[imageDim.x * imageDim.y];
        for (int x = 0; x < imageDim.x; x++)
        {
            for (int y = 0; y < imageDim.y; y++)
            {
                int index = x * imageDim.x + y;
                pixelColors[index] = regions[GetCentroidIndex(new Vector3Int(x, y), centroids)];
            }
        }

        return GetImageFromColorArray(pixelColors);
    }

    int GetCentroidIndex(Vector3Int point, Vector3Int[] centroids)
    {
        int index = 0;
        float minDistance = float.MaxValue;
        for (int i = 0; i < centroids.Length; i++)
        {
            float distance = Vector3Int.Distance(centroids[i], point);
            if (distance < minDistance)
            {
                minDistance = distance;
                index = i;
            }
        }
        return index;
    }

    Texture2D GetImageFromColorArray(Color[] colors)
    {
        Texture2D texture = new Texture2D(imageDim.x, imageDim.y);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }
}