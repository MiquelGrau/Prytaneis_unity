using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Settlement : Location
{
    // Fet a la classe Location
    // public string LocID { get; set; }
    // public string Name { get; set; }
    // public string NodeID { get; set; }
    // public float Latitude { get; set; }
    // public float Longitude { get; set; }
    // public List<Building> SettlBuildings { get; set; } = new List<Building>(); 
    // Demografia i politica
    public int Population { get; set; }
    public string SettlLifestyleID { get; set; }
    public string SettlActivity { get; set; } // Main activity. Castle, Bishop, Trading, Mining, Port...
    //public string OwnerID { get; set; } 
    //public string PoliticalStatus { get; set; }   
    
    // Econòmic
    //public string SettlInventoryID { get; set; } 
    //public float BuildPoints { get; set; } 
    
    
    // Constructor per a Settlement
    public Settlement(string name, string locID, string nodeID, string inventoryID, 
                      string settlActivity, int population, string settlLifestyleID, 
                      string ownerID, string politicalStatus, float buildPoints )
        : base(name, locID, nodeID, inventoryID, ownerID, politicalStatus, buildPoints)  // Crida al constructor de Location
    {
        Population = population;
        SettlLifestyleID = settlLifestyleID;
        OwnerID = ownerID;
        PoliticalStatus = politicalStatus;
        SettlActivity = settlActivity;
        BuildPoints = buildPoints;

        Latitude = 0f;
        Longitude = 0f;
        Buildings = new List<Building>();
    }
    
}






