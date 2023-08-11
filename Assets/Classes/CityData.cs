using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CityData
{
    public string cityName;
    public float latitude;
    public float longitude;
    public List<BuildingInstanceData> buildings;
    public string[][] grid;
}

[System.Serializable]
public class CityDataList
{
    public List<CityData> cities;
}


[System.Serializable]
public class BuildingInstanceData
{
    public string buildingType; // per exemple: "Hospital1", "SmallHouse2", etc.
    public Vector2Int position; // posició de l'edifici dins de la matriu/grid de la ciutat
}
