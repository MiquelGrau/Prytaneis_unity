using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Marker : MonoBehaviour
{
    public string cityName;
    public Vector3 position;

    private void OnMouseDown()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        switch (currentScene)
        {
            case "WorldScene":
                PlayerPrefs.SetString("SelectedCity", cityName);
                SceneManager.LoadScene("CityScene");
                break;

            case "RouteScene":
                // Ací posa la nova funció que vols realitzar quan estàs a RouteScene
                break;

            default:
                Debug.LogWarning("Acció no definida per a aquesta escena.");
                break;
        }
    }
}

public class MarkersManager : MonoBehaviour
{
    public Planet planet;
    public GameObject markerPrefab;
    public DataManager<CityDataList> dataManager;

    private List<Marker> allMarkers = new List<Marker>();

    private void Start()
    {
        if(dataManager.dataItems == null)
        {
            Debug.LogError("dataItems és nul. No s'han carregat les dades correctament des de DataManager.");
            return;
        }

        // Utilitza dades del DataManager per afegir marcadors
        foreach (CityData city in dataManager.dataItems.cities)
        {
            AddMarker(city.latitude, city.longitude, city.cityName);
        }
    }

    public void AddMarker(float lat, float lon, string name)
    {
        Vector3 position = LatLongToPosition(lat, lon);
        GameObject markerObj = Instantiate(markerPrefab, position, Quaternion.identity, this.transform);
        markerObj.name = "Marker_" + name;
        Marker marker = markerObj.AddComponent<Marker>();
        marker.cityName = name;
        marker.position = position;
        allMarkers.Add(marker);
    }

    private Vector3 LatLongToPosition(float lat, float lon)
    {
        float baseRadius = planet.transform.localScale.x;
        float heightOffset = planet.GetHeightAtLatLon(lat, lon) * planet.heightMultiplier;
        float radius = baseRadius + heightOffset + 0.01f;

        lat = Mathf.Deg2Rad * lat;
        lon = Mathf.Deg2Rad * lon;

        Vector3 direction = new Vector3(
            Mathf.Cos(lat) * Mathf.Cos(lon),
            Mathf.Sin(lat),
            Mathf.Cos(lat) * Mathf.Sin(lon)
        ).normalized;

        Vector3 position = direction * radius;

        return position;
    }

    public void UpdateMarkerVisibility(Camera cam)
    {
        foreach (var marker in allMarkers)
        {
            Chunk chunk = FindChunkForPosition(marker.position);
            if (chunk != null)
            {
                marker.gameObject.SetActive(chunk.IsVisibleFrom(cam));
            }
        }
    }

    private Chunk FindChunkForPosition(Vector3 position)
    {
        foreach (TerrainFace face in planet.GetTerrainFaces())
        {
            foreach (Chunk chunk in face.GetChunks())
            {
                if (chunk.Renderer.bounds.Contains(position))
                {
                    return chunk;
                }
            }
        }
        return null;
    }
}
