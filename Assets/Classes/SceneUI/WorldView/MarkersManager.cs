using UnityEngine;
using System.Collections.Generic;

public class MarkersManager : MonoBehaviour
{
    public Planet planet;
    public GameObject markerPrefab;
    public DataManager<CityDataList> dataManager;
    public GameObject contextMenuPrefab;

    private List<Marker> allMarkers = new List<Marker>();

    private void Start()
    {
        if (dataManager.dataItems == null)
        {
            Debug.LogError("dataItems és nul. No s'han carregat les dades correctament des de DataManager.");
            return;
        }

        // Utilitza dades del DataManager per afegir marcadors
        foreach (CityData city in dataManager.dataItems.cities)
        {
            AddMarker(city);
        }
    }

    public void AddMarker(CityData city)
    {
        Vector3 position = LatLongToPosition(city.latitude, city.longitude);
        GameObject markerObj = Instantiate(markerPrefab, position, Quaternion.identity, this.transform);
        markerObj.name = "Marker_" + city.cityName;
        Marker marker = markerObj.AddComponent<Marker>();
        marker.cityName = city.cityName;
        marker.position = position;
        marker.contextMenuPrefab = contextMenuPrefab;
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
