using System.Collections.Generic;

public interface IWorldMapPath
{
    string Id { get; set; }
    string StartNodeId { get; set; }
    string EndNodeId { get; set; }
    float Speed { get; set; }
    string Direction { get; set; }
    float MaxDraft { get; set; }
    float MinFreeboard { get; set; }
    List<IWorldMapMarker> Path { get; set; }
}

[System.Serializable]
public class WorldMapPath : IWorldMapPath
{
    public string Id { get; set; }
    public string StartNodeId { get; set; }
    public string EndNodeId { get; set; }
    public float Speed { get; set; }
    public string Direction { get; set; }
    public float MaxDraft { get; set; }
    public float MinFreeboard { get; set; }
    public List<IWorldMapMarker> Path { get; set; }

    public WorldMapPath(string id = "", string startNodeId = "", string endNodeId = "", float speed = 0, string direction = "", float maxDraft = 0, float minFreeboard = 0, List<IWorldMapMarker> path = null)
    {
        Id = id;
        StartNodeId = startNodeId;
        EndNodeId = endNodeId;
        Speed = speed;
        Direction = direction;
        MaxDraft = maxDraft;
        MinFreeboard = minFreeboard;
        Path = path ?? new List<IWorldMapMarker>();
    }
}

[System.Serializable]
public class WorldMapWaterPath : WorldMapPath
{
    public PathType TypeOfPath { get; set; }  // Tipus de ruta (Port o Coastal)

    public enum PathType
    {
        Port,
        Coastal
    }

    public WorldMapWaterPath(string id = "", string startNodeId = "", string endNodeId = "", float speed = 0, string direction = "", float maxDraft = 0, float minFreeboard = 0, List<IWorldMapMarker> path = null, PathType typeOfPath = PathType.Port)
        : base(id, startNodeId, endNodeId, speed, direction, maxDraft, minFreeboard, path)
    {
        TypeOfPath = typeOfPath;
    }
}

[System.Serializable]
public class WaterPathDataWrapper
{
    public List<WorldMapWaterPath> waterpath_jsonfile;
}

