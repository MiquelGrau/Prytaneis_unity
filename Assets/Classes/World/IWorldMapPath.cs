using System.Collections.Generic;

[System.Serializable]
public class WorldMapWaterPath
{
    public string waterpathId;
    public string startNode;
    public string endNode;
    public string pathType;
    public float pathSpeed; 
    public string pathDirection;
    public List<WorldMapMarker> pathArray; // S'ha definit com una llista de WorldMapMarker

    public override string ToString()
    {
        return $"WaterpathId: {waterpathId}, StartNode: {startNode}, EndNode: {endNode}, PathType: {pathType}, PathSpeed: {pathSpeed}, PathDirection: {pathDirection}, PathArray: {string.Join(", ", pathArray)}";
    }
}

[System.Serializable]
public class WaterPathDataWrapper
{
    public List<WorldMapWaterPath> waterpath_jsonfile;
}

[System.Serializable]
public class WorldMapLandPath
{
    public string landpathId;
    public string startNode;
    public string endNode;
    public float pathDifficulty; 
    public float pathLevel; 
    public string pathType; 
    public List<WorldMapMarker> pathArray; 

    public override string ToString()
    {
        return $"PathId: {landpathId}, StartNode: {startNode}, EndNode: {endNode}, Difficulty: {pathDifficulty}, Road level: {pathLevel}, path type: {pathType}, PathArray: {string.Join(", ", pathArray)}";
    }
}

[System.Serializable]
public class LandPathDataWrapper
{
    public List<WorldMapLandPath> landpath_jsonfile;
}