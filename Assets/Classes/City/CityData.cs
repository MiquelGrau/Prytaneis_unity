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
    public CityInventory CityInventory { get; set; } // Referència directa a CityInventory
    
    public int PoorPopulation { get; set; }
    public int MidPopulation { get; set; }
    public int RichPopulation { get; set; }

    public int Population
    {
        get { return PoorPopulation + MidPopulation + RichPopulation; }
    }

    public int PoorLifestyleID; 
    public int MidLifestyleID; 
    public int RichLifestyleID; 
    
    //public List<BuildingInstanceData> buildings;
    public string[][] grid;
    
}


[System.Serializable]
public class CityDataList
{
    public List<CityData> cities;
}
