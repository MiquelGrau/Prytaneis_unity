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
    public string OwnerID { get; set; } // relacionat amb el jugador que ho té. 
    public string PoliticalStatus { get; set; }   // republic, fief, peasant commune, etc
    public string SettlActivity { get; set; } // Main activity. Castle, Bishop, Trading, Mining, Port...
    // Econòmic
    //public string SettlInventoryID { get; set; } 
    //public float BuildPoints { get; set; } 
    
    
    // Constructor per a Settlement
    public Settlement(string name, string locID, string nodeID, string inventoryID, 
                      string settlActivity, int population, string settlLifestyleID, 
                      float buildPoints, string politicalStatus, string ownerID )
        : base(name, locID, nodeID, inventoryID, buildPoints)  // Crida al constructor de Location
    {
        Population = population;
        SettlLifestyleID = settlLifestyleID;
        OwnerID = ownerID;
        PoliticalStatus = politicalStatus;
        SettlActivity = settlActivity;
        //SettlInventoryID = settlInventoryID;
        BuildPoints = buildPoints;

        Latitude = 0f;
        Longitude = 0f;
        Buildings = new List<Building>();
    }
    
}






