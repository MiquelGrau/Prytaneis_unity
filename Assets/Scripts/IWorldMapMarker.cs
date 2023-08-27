public interface IWorldMapMarker
{
    float Latitude { get; set; }
    float Longitude { get; set; }
}

[System.Serializable]
public class WorldMapMarker : IWorldMapMarker
{
    public float Latitude { get; set; }
    public float Longitude { get; set; }

    public WorldMapMarker(float latitude = 0, float longitude = 0)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}
