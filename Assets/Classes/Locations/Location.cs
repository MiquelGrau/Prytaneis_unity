using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Location
{
    // Propietats comunes
    public string Name { get; set; }
    public string LocID { get; set; }
    // Geographic reference
    public string NodeID { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    // Content
    public List<Building> Buildings { get; set; }
    public string InventoryID { get; set; }  
    
    // Startup values
    public float BuildPoints { get; set; } 
    

    // Constructor per a Location
    public Location(string name, string locID, string nodeID, string inventoryID, 
                    string ownerID, string politicalStatus, float buildPoints)
    {
        Name = name;
        LocID = locID;
        NodeID = nodeID;
        InventoryID = inventoryID;  
        BuildPoints = buildPoints;
        
        Latitude = 0f;
        Longitude = 0f;
        Buildings = new List<Building>(); 
        
    }
}
