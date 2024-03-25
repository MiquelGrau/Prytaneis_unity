using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MarkersManager : MonoBehaviour
{
    public Planet planet;
    public GameObject markerPrefab;
    public DataManager dataManager;
    
    private List<Marker> allMarkers = new List<Marker>();

    public Material defaultMarkerMaterial;
    public Material routeMarkerMaterial;

    private List<Marker> currentRouteMarkers = new List<Marker>();


    private void Start()
    {
        if (dataManager.dataItems == null)
        {
            Debug.LogError("dataItems és nul. No s'han carregat les dades correctament des de DataManager.");
            return;
        }

        // Utilitza dades del DataManager per afegir marcadors per a cada node
        foreach (WorldMapNode node in DataManager.worldMapNodes)
        {
            AddMarker(node);
        } 
    }

    //public void AddMarker(CityData city)
    public void AddMarker(WorldMapNode node)
    {
        Vector3 position = LatLongToPosition(node.latitude, node.longitude);
        GameObject prefabToUse = markerPrefab;
        GameObject markerObj = Instantiate(prefabToUse, position, Quaternion.identity, this.transform);
        markerObj.name = "Marker_" + node.id; 
        Marker marker = markerObj.AddComponent<Marker>();
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

    public void OnNewRouteSelected(string startNodeId, string endNodeId)
    {
        ResetMarkersToDefaultMaterial(); // Restaura marcadors a l'estat per defecte
        
        var routePaths = WorldMapUtils.DijkstraAlgorithm(startNodeId, endNodeId, DataManager.worldMapNodes, DataManager.worldMapLandPaths);
        if (routePaths != null && routePaths.Count > 0)
        {
            UpdateMarkerMaterialForRoute(routePaths); // Actualitza els marcadors de la nova ruta
            CreateRouteBetweenMarkers(startNodeId, endNodeId);
        }
    }

    // Restaura tots els marcadors al material per defecte
    public void ResetMarkersToDefaultMaterial()
    {
        foreach (var marker in currentRouteMarkers)
        {
            var meshRenderer = marker.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = defaultMarkerMaterial;
            }
        }
        // Neteja la llista una vegada tots els marcadors han estat resetejats
        currentRouteMarkers.Clear();
    }


    // Actualitza els marcadors específics per la ruta al material de ruta
    public void UpdateMarkerMaterialForRoute(List<WorldMapLandPath> routePaths)
    {
        HashSet<string> nodeIdsInRoute = new HashSet<string>(routePaths.SelectMany(path => new[] { path.startNode, path.endNode }));

        foreach (var marker in allMarkers)
        {
            if (nodeIdsInRoute.Contains(marker.id))
            {
                var meshRenderer = marker.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.material = routeMarkerMaterial;
                    currentRouteMarkers.Add(marker); // Afegir a la llista per ràpid accés posterior
                }
            }
        }
    }

    public void CreateRouteBetweenMarkers(string startMarkerId, string endMarkerId)
    {
        Marker startMarker = allMarkers.FirstOrDefault(marker => marker.id == startMarkerId);
        Marker endMarker = allMarkers.FirstOrDefault(marker => marker.id == endMarkerId);

        if (startMarker != null && endMarker != null)
        {
            // Asumim que tenim una referència a un objecte Route ja existent a l'escena o la creem
            Route routeManager = FindObjectOfType<Route>(); // Troba l'instància de Route a l'escena
            if (routeManager != null)
            {
                routeManager.ConnectMarkersWithLine(startMarker, endMarker);
            }
        }
    }
}
