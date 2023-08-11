using System.Collections.Generic;

[System.Serializable]
public class RootObject
{
    public List<CityDataAux> cities;
}

[System.Serializable]
public class CityDataAux
{
    public string cityName;
    public float latitude;
    public float longitude;
    public List<BuildingInstanceData> buildings;
    public List<List<string>> grid;
}
