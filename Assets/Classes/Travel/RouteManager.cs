using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RouteManager : MonoBehaviour
{
    public WorldMapSettings worldMapSettings;  // Referència a WorldMapSettings
    public Material lineMaterial;  // Material per les línies de ruta
    public Transform earthTransform; // Drag and drop your Earth object here in the inspector

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
        NodeDataWrapper nodeDataWrapper = JsonUtility.FromJson<NodeDataWrapper>(nodeData.text);
        List<WorldMapNode> nodesList = nodeDataWrapper.nodes_jsonfile;
        var waterPathDataWrapper = JsonUtility.FromJson<WaterPathDataWrapper>(waterPathData.text);
        List<WorldMapWaterPath> waterPathsList = waterPathDataWrapper.waterpath_jsonfile;

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
        List<WorldMapNode> nodes = worldMapSettings.nodes;
        List<WorldMapWaterPath> waterPaths = worldMapSettings.waterPaths;

        WorldMapNode startNode = nodes.Find(n => n.id == startCity.nodeID);
        Vector3 startPosition = LatLongToPosition(startNode.latitude, startNode.longitude);

        List<WorldMapWaterPath> route = WorldMapUtils.DijkstraAlgorithm(startCity.nodeID, destinationCity.nodeID, nodes, waterPaths);
        if (route != null && route.Count > 0) {
            Debug.Log($"Ruta creada: {route[0]} ");
        } else {
            Debug.Log("No s'ha trobat cap ruta entre les ciutats especificades.");
        }

        foreach (var path in route)
        {
            WorldMapWaterPath pathObj = waterPaths.Find(p => p.waterpathId == path.waterpathId);
            if (pathObj != null)
            {
                foreach (var marker in pathObj.pathArray)
                {
                    Debug.Log($"Marker: Longitude = {marker.Longitude}, Latitude = {marker.Latitude}");
                }

                Vector3[] pathMarkerArray = pathObj.pathArray.Select(marker => LatLongToPosition(marker.Latitude, marker.Longitude)).ToArray();
                Debug.Log($"Punts de la ruta: {string.Join(", ", pathMarkerArray.Select(p => p.ToString()))}");

                Vector3[] curve = CreatePathFromPoints(pathMarkerArray);
                GameObject line = CreatePathLine(curve, startPosition);
                line.transform.SetParent(earthTransform, false);
            }
        }
    }

    private float GetHeightAtLatLon(float lat, float lon)
    {
        // Si el teu earthTransform no té una funció anomenada 'GetHeightAtLatLon' o similar,
        // llavors hauràs d'implementar la lògica aquí o simplement retornar 0.
        // En aquest exemple, suposarem que la teva terra és plana i no té elevacions, així que retornarem 0.
        return 0f;
    }

    private Vector3 LatLongToPosition(float lat, float lon)
    {
        float baseRadius = earthTransform.localScale.x / 2;  // Agafem el radi a partir de la mitat de l'escala en x.
        float heightOffset = GetHeightAtLatLon(lat, lon);  // Obtenim l'altitud en aquesta latitud/longitud.
        float radius = baseRadius + heightOffset + 0.01f;  // Afegim un petit desplaçament de 0.01 per assegurar-nos que està lleugerament per sobre del terreny.

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

    private Vector3[] CreatePathFromPoints(Vector3[] points)
    {
        return points;
    }

    private GameObject CreatePathLine(Vector3[] curve, Vector3 startPosition)
    {
        Debug.Log("Intentant crear l'objecte RoutePath...");
        GameObject lineObject = new GameObject("RoutePath");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        Vector3[] adjustedCurve = new Vector3[curve.Length + 1];
        adjustedCurve[0] = startPosition;
        for (int i = 0; i < curve.Length; i++)
        {
            adjustedCurve[i + 1] = curve[i];
        }

        lineRenderer.positionCount = adjustedCurve.Length;
        lineRenderer.SetPositions(adjustedCurve);

        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;

        return lineObject;
    }
}
