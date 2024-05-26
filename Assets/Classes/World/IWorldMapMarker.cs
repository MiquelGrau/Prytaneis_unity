[System.Serializable]
public class WorldMapMarker
{
    public float Latitude;
    public float Longitude;

    public WorldMapMarker(float latitude = 0, float longitude = 0)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public override string ToString()
    {
        return $"Latitude: {Latitude}, Longitude: {Longitude}";
    }
}
