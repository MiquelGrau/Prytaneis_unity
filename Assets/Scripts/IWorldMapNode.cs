using System.Collections.Generic;

[System.Serializable]
public class WorldMapNode
{
    public string id;
    public string cityId;
    public string name;
    public float latitude;
    public float longitude;
    public string RegionId;
    public bool IsSuperNode;
    public string ConnectionsId;
    public string LandNodeType;
    public string WaterNodeType;

    public override string ToString()
    {
        return $"ID: {id}, CityId: {cityId}, Name: {name}, Latitude: {latitude}, Longitude: {longitude}, RegionId: {RegionId}, IsSuperNode: {IsSuperNode}, LandNodeType: {LandNodeType}, WaterNodeType: {WaterNodeType}";
    }

    public override bool Equals(object obj)
    {
        if (obj is WorldMapNode other)
        {
            return other.id == this.id;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}

[System.Serializable]
public class NodeDataWrapper
{
    public List<WorldMapNode> nodes_jsonfile;
}
