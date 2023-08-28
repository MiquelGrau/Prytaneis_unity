using System.Collections.Generic;

[System.Serializable]
public class WorldMapWaterPath
{
    public string waterpathId;
    public string startNode;
    public string endNode;
    public string pathType;
    public float pathSpeed; // He canviat a float ja que pathSpeed és "1", però si en realitat hauria de ser una string, torna a canviar-lo
    public string pathDirection;
    public List<WorldMapMarker> pathArray; // S'ha definit com una llista de WorldMapMarker

    public override string ToString()
    {
        return $"WaterpathId: {waterpathId}, StartNode: {startNode}, EndNode: {endNode}, PathType: {pathType}, PathSpeed: {pathSpeed}, PathDirection: {pathDirection}, PathArray: {string.Join(", ", pathArray)}";
    }
}

[System.Serializable]
public class PathDataWrapper
{
    public List<WorldMapWaterPath> waterpath_jsonfile;
}
