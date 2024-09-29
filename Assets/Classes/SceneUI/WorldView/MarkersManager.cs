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
        
        // Utilitza dades del DataManager per afegir marcadors per a cada node
        foreach (WorldMapNode node in DataManager.worldMapNodes)
        {
            AddMarker(node);
        } 
    }

    // PART GRAFICA
    public void AddMarker(WorldMapNode node) // Afegeix nodes al mapa
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
    
    // CREADOR DE RUTA, optimitza la linia entre dos punts del mapa
    public void OnNewRouteSelected(string startNodeId, string endNodeId)
    {
        ResetMarkersToDefaultMaterial(); // Restaura marcadors a l'estat per defecte

        Route routeManager = FindObjectOfType<Route>();
        if (routeManager != null)
        {
            routeManager.ClearAllRoutes();
            routeManager.ClearAllLines();
        }

        var routePaths = WorldMapUtils.DijkstraAlgorithm(startNodeId, endNodeId, DataManager.worldMapNodes, DataManager.worldMapLandPaths);
        
        // Representacio grafica al mapa
        if (routePaths != null && routePaths.Count > 0)
        {
            // Assignar la ruta a l'agent
            AssignRouteToAgent(routePaths);

            // Actualitza els marcadors de la nova ruta
            UpdateMarkerMaterialForRoute(routePaths); 

            // Converteix els IDs dels nodes de la ruta en marcadors
            List<Marker> markersInRoute = routePaths.SelectMany(path => new[] { path.startNode, path.endNode })
                                                    .Distinct()
                                                    .Select(id => allMarkers.FirstOrDefault(marker => marker.id == id))
                                                    .Where(marker => marker != null)
                                                    .ToList();

            // Aquest bucle ha estat modificat per utilitzar correctament les variables dins de l'àmbit
            foreach (var path in routePaths)
            {
                List<Vector3> pathPoints = path.pathArray.Select(marker => LatLongToPosition(marker.Latitude, marker.Longitude)).ToList();
                routeManager.DrawLineOnEarth(pathPoints);
            }
        }
    }

    private void AssignRouteToAgent(List<WorldMapLandPath> routePaths)
    {
        var currentAgent = GameManager.Instance.currentAgent;
        if (currentAgent != null)
        {
            if (routePaths.Count > 0)
            {
                // Converteix el primer path a AgentTravel i l'assigna a Travel
                currentAgent.Travel = ConvertPathToTravel(routePaths[0]);
                currentAgent.Travel.Started = true; // Començar el viatge

                // Converteix la resta de paths a AgentTravel i els afegeix a NextTravelSteps
                currentAgent.NextTravelSteps = routePaths.Skip(1).Select(path => ConvertPathToTravel(path)).ToList();
                
                // Debug log per confirmar la ruta de viatge
                Debug.Log("Confirmada ruta de viatge");

                float totalLength = 0;
                float totalDays = 0;

                Debug.Log($"Node inicial: {routePaths[0].startNode}, node destí: {routePaths[0].endNode}, distància: {currentAgent.Travel.LengthTotal}, dies: {currentAgent.Travel.DaysTotal}");
                totalLength += currentAgent.Travel.LengthTotal;
                totalDays += currentAgent.Travel.DaysTotal;

                foreach (var travel in currentAgent.NextTravelSteps)
                {
                    Debug.Log($"Node inicial: {travel.Current}, node destí: {travel.Destination}, distància: {travel.LengthTotal}, dies: {travel.DaysTotal}");
                    totalLength += travel.LengthTotal;
                    totalDays += travel.DaysTotal;
                }

                Debug.Log($"Distància total de la ruta: {totalLength}, dies totals: {totalDays}");
            }
        }
        else
        {
            Debug.LogWarning("No hi ha cap agent seleccionat actualment.");
        }
    }

    private AgentTravel ConvertPathToTravel(WorldMapLandPath path)
    {
        float lengthTotal = 0.0f;
        for (int i = 0; i < path.pathArray.Count - 1; i++)
        {
            WorldMapMarker marker1 = path.pathArray[i];
            WorldMapMarker marker2 = path.pathArray[i + 1];
            lengthTotal += (float)WorldMapUtils.HaversineDistance(marker1, marker2);
        }
        
        var travel = new AgentTravel(path.startNode, path.endNode)
        {
            PathID = path.landpathId,
            LengthTotal = lengthTotal,
            DaysTotal = lengthTotal / GameManager.Instance.currentAgent.speed
        };
        return travel;
    }

    public void MoveAllAgents()
    {
        var agents = DataManager.Instance.GetAgents();
        foreach (var agent in agents)
        {
            if (agent.Travel != null && agent.Travel.Started)
            {
                agent.Travel.LengthDone += agent.speed;
                agent.Travel.DaysDone += 1;

                if (agent.Travel.LengthDone >= agent.Travel.LengthTotal)
                {
                    FinishAgentTravel(agent, agent.Travel);
                }
            }
        }
    }
    public void FinishAgentTravel(Agent agent, AgentTravel travel)
    {
        agent.LocationNode = travel.Destination;

        // Elimina el AgentTravel actual
        agent.Travel = null;

        // Trasllada el primer de NextTravelSteps a Travel
        if (agent.NextTravelSteps.Count > 0)
        {
            agent.Travel = agent.NextTravelSteps[0];
            agent.Travel.Started = true;
            agent.NextTravelSteps.RemoveAt(0);
        }

        // Obté els noms dels nodes per al debug.log
        var currentNodeName = DataManager.Instance.NodeNameByID(travel.Current);
        var destinationNodeName = DataManager.Instance.NodeNameByID(travel.Destination);

        Debug.Log($"Viatge fet entre {currentNodeName} i {destinationNodeName}");
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

    // La part de la interficie gràfica en el mapa
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
