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
    public int Population { get { return PoorPopulation + MidPopulation + RichPopulation; } }

    public string PoorLifestyleID; 
    public string MidLifestyleID; 
    public string RichLifestyleID; 
    
    public List<Building> CityBuildings { get; set; } = new List<Building>(); 
    public string[][] grid;
    public float BuildPoints { get; set; } 
    
}



[System.Serializable]
public class Settlement
{
    public string settlementID;
    public string settlementName;
    public string nodeID;
    public float latitude;
    public float longitude;
    
    public List<Building> SettlBuildings { get; set; } = new List<Building>(); 
    public int SettlementMoney { get; set; }
    public List<CityInventoryResource> SettlResources { get; set; }
    
    public string SettlLifestyleID;   
    //public List<CityDemands> Demands { get; set; } = new List<CityDemands>();
    
    public int PoorPopulation { get; set; }
    public int MidPopulation { get; set; }
    public int RichPopulation { get; set; }
    public int Population { get { return PoorPopulation + MidPopulation + RichPopulation; } }
    
    public float BuildPoints { get; set; } 
    
}

