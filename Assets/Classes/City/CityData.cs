using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CityData
{
    public string cityID;
    public string nodeID;
    public string cityName;
    public float latitude;
    public float longitude;
    public string cityInventoryID;
    //public int money; //money inside inventory, now
    
    public int PoorPopulation { get; set; }
    public int MidPopulation { get; set; }
    public int RichPopulation { get; set; }

    public int Population
    {
        get { return PoorPopulation + MidPopulation + RichPopulation; }
    }

    public int poorLifestyleID; 
    public int midLifestyleID; 
    public int richLifestyleID; 
    
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
