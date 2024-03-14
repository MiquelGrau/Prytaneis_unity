using UnityEngine;
using System.Collections.Generic;

public class MarkersManager : MonoBehaviour
{
    public Planet planet;
    public GameObject markerPrefab;
    public DataManager dataManager;
    
    private List<Marker> allMarkers = new List<Marker>();

    private void Start()
    {
        if (dataManager.dataItems == null)
        {
            Debug.LogError("dataItems és nul. No s'han carregat les dades correctament des de DataManager.");
            return;
        }

        // Utilitza dades del DataManager per afegir marcadors
        /* foreach (CityData city in dataManager.dataItems.cities)
        {
            AddMarker(city);
        } */
        // Utilitza dades del DataManager per afegir marcadors per a cada node
        foreach (WorldMapNode node in DataManager.worldMapNodes)
        {
            AddMarker(node);
        } 
    }

    //public void AddMarker(CityData city)
    public void AddMarker(WorldMapNode node)
    {
        //Vector3 position = LatLongToPosition(city.latitude, city.longitude);
        Vector3 position = LatLongToPosition(node.latitude, node.longitude);
        GameObject markerObj = Instantiate(markerPrefab, position, Quaternion.identity, this.transform);
        //markerObj.name = "Marker_" + city.cityName;
        markerObj.name = "Marker_" + node.id; 
        Marker marker = markerObj.AddComponent<Marker>();
        //marker.cityName = city.cityName;
        marker.cityName = node.name; 
        marker.id = node.id; 
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
