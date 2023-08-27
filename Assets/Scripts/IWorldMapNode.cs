using System.Collections.Generic;
public interface IWorldMapNode
{
    string Id { get; set; }
    string CityId { get; set; }
    IWorldMapMarker Marker { get; set; }
    string Name { get; set; }
    string RegionId { get; set; }
    bool IsSuperNode { get; set; }
    string NodeType { get; set; }
}

[System.Serializable]
public class WorldMapNode : IWorldMapNode
{
    public string Id { get; set; }
    public string CityId { get; set; }
    public IWorldMapMarker Marker { get; set; }
    public string Name { get; set; }
    public string RegionId { get; set; }
    public bool IsSuperNode { get; set; }
    public string NodeType { get; set; }

    public WorldMapNode(string id, string cityId, IWorldMapMarker marker, string name = "", string regionId = "", string nodeType = "", bool isSuperNode = false)
    {
        Id = id;
        CityId = cityId;
        Marker = marker;
        Name = name;
        RegionId = regionId;
        NodeType = nodeType;
        IsSuperNode = isSuperNode;
    }
}
