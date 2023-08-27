using System.Collections.Generic;
public interface IWorldMapNode
{
    string id { get; set; }
    string CityId { get; set; }
    string NodeId { get; set; }
    IWorldMapMarker Marker { get; set; }
    string Name { get; set; }
    string RegionId { get; set; }
    bool IsSuperNode { get; set; }
    string NodeType { get; set; }
}

[System.Serializable]
public class WorldMapNode : IWorldMapNode
{
    public string id { get; set; }
    public string CityId { get; set; }
    public string NodeId { get; set; }
    public IWorldMapMarker Marker { get; set; }  // Assegura't que IWorldMapMarker estigui definit adequadament en un altre fitxer.
    public string Name { get; set; }
    public string RegionId { get; set; }
    public bool IsSuperNode { get; set; }
    public string NodeType { get; set; }

    // Constructor amb 8 arguments
    public WorldMapNode(string id, string CityId, string NodeId, IWorldMapMarker Marker, string Name, string RegionId, string NodeType, bool IsSuperNode)
    {
        this.id = id;
        this.CityId = CityId;
        this.NodeId = NodeId;
        this.Marker = Marker;
        this.Name = Name;
        this.RegionId = RegionId;
        this.NodeType = NodeType;
        this.IsSuperNode = IsSuperNode;
    }

    public override string ToString()
    {
        return $"ID: {id}, CityId: {CityId}, NodeId: {NodeId}, Marker: {Marker}, Name: {Name}, RegionId: {RegionId}, IsSuperNode: {IsSuperNode}, NodeType: {NodeType}";
    }
}

[System.Serializable]
public class NodeDataWrapper
{
    public List<WorldMapNode> nodes_jsonfile;
}
