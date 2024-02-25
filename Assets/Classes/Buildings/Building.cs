using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    public string BuildingID { get; set; }
    public string BuildingName { get; set; }
    public string BuildingTemplateID { get; set; }
    public string BuildingLocation { get; set; }
    public string BuildingOwnerID { get; set; }
    public string RelatedInventoryID { get; set; }
    public string ActivityStatus { get; set; }
    public int BuildingSize { get; set; }
    public int HPCurrent { get; set; }
    public int HPMaximum { get; set; }
    public int Capacity { get; set; }

    // Constructor
    public Building(string id, string name, string templateID, string location, string ownerID, string inventoryID,
                    string activity, int size, int hpCurrent, int hpMax, int capacity)
    {
        BuildingID = id;
        BuildingName = name;
        BuildingTemplateID = templateID;
        BuildingLocation = location;
        BuildingOwnerID = ownerID;
        RelatedInventoryID = inventoryID;
        ActivityStatus = activity;
        BuildingSize = size;
        HPCurrent = hpCurrent;
        HPMaximum = hpMax;
        Capacity = capacity;
    }

    // Basic functions
    public void IdentifyTemplate()
    {
        // Logic to identify the building template using the BuildingTemplateID
    }

    public void IdentifyOwner()
    {
        // Logic to identify the owner using the BuildingOwnerID
    }

    
    public void UpgradeBuilding(string newTemplateID)
    {
        // Logic to upgrade the building
        BuildingTemplateID = newTemplateID;
        // Update other properties if necessary...
    }

}

