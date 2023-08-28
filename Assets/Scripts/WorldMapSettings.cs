using System.Collections.Generic;
using UnityEngine;

public class WorldMapSettings : MonoBehaviour
{
    public List<WorldMapCity> cities;
    public List<WorldMapNode> nodes;  // Assegura't que això és una List<WorldMapNode>
    public List<WorldMapWaterPath> waterPaths;
}

[System.Serializable]
public class WorldMapCity 
{
    public string id;
    public string nodeId;
    public WorldMapMarker marker;
}