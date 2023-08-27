using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RouteManager : MonoBehaviour
{
    public WorldMapSettings worldMapSettings;  // Referència a WorldMapSettings
    public Material lineMaterial;  // Material per les línies de ruta

    private void Awake()
    {
        LoadWorldMapData();
    }

    private void LoadWorldMapData()
    {
        // Carregar les dades
        TextAsset cityData = Resources.Load<TextAsset>("CityData");
        TextAsset nodeData = Resources.Load<TextAsset>("NodeData");
        TextAsset waterPathData = Resources.Load<TextAsset>("WaterPathData");

        // Parsejar les dades
        List<WorldMapCity> citiesList = JsonUtility.FromJson<List<WorldMapCity>>(cityData.text);
        List<WorldMapNode> nodesList = JsonUtility.FromJson<List<WorldMapNode>>(nodeData.text);
        List<WorldMapWaterPath> waterPathsList = JsonUtility.FromJson<List<WorldMapWaterPath>>(waterPathData.text);

        if (citiesList == null || nodesList == null || waterPathsList == null) {
            Debug.Log("Error deserialitzant les dades.");
        } else {
            Debug.Log("Dades deserialitzades amb èxit.");
        }

        // Assignar les dades a WorldMapSettings
        worldMapSettings.cities = citiesList;
        worldMapSettings.nodes = nodesList;
        worldMapSettings.waterPaths = waterPathsList;
    }

    public void CreateRoute(CityData startCity, CityData destinationCity)
    {
        Debug.Log($"Creant ruta des de {startCity.cityName} fins a {destinationCity.cityName}.");

        List<WorldMapCity> cities = worldMapSettings.cities;
        List<IWorldMapNode> nodes = worldMapSettings.nodes.Select(n => (IWorldMapNode)n).ToList();
        List<IWorldMapPath> waterPaths = worldMapSettings.waterPaths.Select(wp => (IWorldMapPath)wp).ToList();

        // Utilitzar l'algoritme de Dijkstra per determinar la ruta entre els dos nodes
        List<IWorldMapPath> route = WorldMapUtils.DijkstraAlgorithm(startCity.nodeID, destinationCity.nodeID, nodes, waterPaths);
        if (route != null && route.Count > 0) {
            Debug.Log($"Ruta creada: {route[0]} ");
            // El codi restant per gestionar la ruta
        } else {
            Debug.Log("No s'ha trobat cap ruta entre les ciutats especificades.");
        }

        
        // Imprimir la ruta (o fer qualsevol cosa que necessitis amb ella)
        foreach (var path in route)
        {
            Debug.Log($"Cami des de {path.StartNodeId} fins a {path.EndNodeId} amb velocitat {path.Speed}.");

            // Per pintar el path basat en l'objecte de ruta actual
            WorldMapPath pathObj = waterPaths.Find(p => p.Id == path.Id) as WorldMapPath;
            if (pathObj != null)
            {
                Vector3[] pathMarkerArray = pathObj.Path.Select(marker => new Vector3(marker.Longitude, marker.Latitude, 0)).ToArray();
                Vector3[] curve = CreatePathFromPoints(pathMarkerArray);
                GameObject line = CreatePathLine(curve);
                line.transform.SetParent(this.transform, false);  // Attach to RouteManager object
            }
        }
    }

    private Vector3[] CreatePathFromPoints(Vector3[] points)
    {
        // En aquesta implementació, simplement retornem els punts originals. 
        // Podries afegir lògica per suavitzar o modificar el camí si ho necessites.
        return points;
    }

    private GameObject CreatePathLine(Vector3[] curve)
    {
        // 1. Creació d'un nou GameObject.
        Debug.Log("Intentant crear l'objecte RoutePath...");
        GameObject lineObject = new GameObject("RoutePath");

        // 2. Afegir LineRenderer al GameObject.
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        // 3. Configuració de punts de la línia.
        lineRenderer.positionCount = curve.Length;
        lineRenderer.SetPositions(curve);

        // 4. Configuració del material i aparença de la línia.
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.1f;  // Pots ajustar aquest valor segons les teves necessitats.
        lineRenderer.endWidth = 0.1f;

        // 5. Retornar l'objecte.
        return lineObject;
    }
}
