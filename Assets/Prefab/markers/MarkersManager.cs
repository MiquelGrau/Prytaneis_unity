using UnityEngine;
using System.Collections.Generic;

public class Marker : MonoBehaviour
{
    public string cityName;
    public Vector3 position;
}

public class MarkersManager : MonoBehaviour
{
    public Planet planet;
    public GameObject markerPrefab;
    private List<Marker> allMarkers = new List<Marker>();

    private void Start()
    {
        // Afegeix marcadors per defecte
        AddMarker(41.3851f, 2.1734f, "Barcelona");
        AddMarker(41.9028f, 12.4964f, "Roma");
        AddMarker(43.7228f, 10.4017f, "Pisa");
        AddMarker(43.7102f, 7.2620f, "Nissa");
        AddMarker(43.2965f, 5.3698f, "Marsella");
    }

    public void AddMarker(float lat, float lon, string name)
    {
        Vector3 position = LatLongToPosition(lat, lon);
        GameObject markerObj = Instantiate(markerPrefab, position, Quaternion.identity, this.transform);
        markerObj.name = "Marker_" + name; // Canviar el nom per a una fàcil identificació
        Marker marker = markerObj.AddComponent<Marker>();
        marker.cityName = name;
        marker.position = position;
        allMarkers.Add(marker);
    }

    private Vector3 LatLongToPosition(float lat, float lon)
    {
        float baseRadius = planet.transform.localScale.x;
        float heightOffset = planet.GetHeightAtLatLon(lat, lon) * planet.heightMultiplier; // obtenim l'altura i l'escalim amb heightMultiplier
        float radius = baseRadius + heightOffset + 0.01f; // Afegeixo un petit desplaçament de 0.05

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
