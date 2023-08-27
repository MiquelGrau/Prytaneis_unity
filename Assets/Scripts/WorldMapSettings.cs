using System.Collections.Generic;
using UnityEngine;

public class WorldMapSettings : MonoBehaviour
{
    public List<WorldMapCity> cities = new List<WorldMapCity>();
    public List<WorldMapNode> nodes = new List<WorldMapNode>();
    public List<WorldMapWaterPath> waterPaths = new List<WorldMapWaterPath>();
    public List<WorldMapPath> landPaths = new List<WorldMapPath>();  // Aquesta es pot omplir quan tinguis el format de LandPathsData.json
}

[System.Serializable]
public class WorldMapCity 
{
    public string id;
    public string nodeId;
    public WorldMapMarker marker;
}