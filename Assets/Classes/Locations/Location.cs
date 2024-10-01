using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Location
{
    // Propietats comunes
    public string Name { get; set; }
    public string LocID { get; set; }
    public string NodeID { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public List<Building> Buildings { get; set; }
    public string InventoryID { get; set; }  
    public float BuildPoints { get; set; } 
    

    // Constructor per a Location
    /* public Location(string name, string locID, string nodeID)
    {
        Name = name;
        LocID = locID;
        NodeID = nodeID;
        Latitude = 0f;
        Longitude = 0f;
        Buildings = new List<Building>(); // Edificis s'inicialitzen després, sempre en blanc al crear-se. 
    } */
    public Location(string name, string locID, string nodeID, string inventoryID, float buildPoints)
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
