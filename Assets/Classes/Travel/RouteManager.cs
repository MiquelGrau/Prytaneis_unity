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
}
