using System.Collections.Generic;

[System.Serializable]
public class WorldMapNode
{
    public string id;
    public string name;
    public string cityId;
    public float latitude;
    public float longitude;
    public string LandNodeType;
    public string LandContinentId;
    public string LandRegionId;
    public string LandSubregionId;
    public string WaterNodeType;
    public string WaterNodeRegion;
    public string WaterNodeSubregion;
    //public bool IsSuperNode;
    //public string ConnectionsId;
    
    public override string ToString()
    {
        return $"ID: {id}, CityId: {cityId}, Name: {name}, Latitude: {latitude}, Longitude: {longitude}, LandNodeType: {LandNodeType}, RegionId: {LandRegionId}, WaterNodeType: {WaterNodeType}";
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
