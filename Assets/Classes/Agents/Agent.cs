using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Agent
{
    public string agentID;
    public string agentName;   // wil get the main character's name, probably
    public string LocationNode; 
    public string AgentInventoryID; 
    public AgentInventory Inventory; 
    public string MainCharID; 
    public Character MainCharacter; 
    //public List<Character> CompanionCharacters; 
    //public List<Vehicle> VehicleList; 
    public string TravelMode; // "land", "water", "messenger", "star"

    /* public Agent()
    {
        CompanionCharacters = new List<Character>();
        VehicleList = new List<Vehicle>();
    } */
    public Agent(string agentID, string agentName, string locationNode, string agentInventoryID, string mainCharID) // inputs must agree with JSON definitions
    {
        this.agentID = agentID;
        this.agentName = agentName;
        this.LocationNode = locationNode;
        this.AgentInventoryID = agentInventoryID;
        this.Inventory = null; // Es crea linkant després
        this.MainCharID = mainCharID;
        this.MainCharacter = null; // Es crea linkant després
        //this.CompanionCharacters = companionCharacters ?? new List<Character>(); 
        //this.VehicleList = vehicleList ?? new List<Vehicle>(); 
        this.TravelMode = null; // Es calcula
    }

    

    // Pots afegir aquí mètodes específics d'Agent si és necessari
}



/* [System.Serializable]
public class Agent
{
    public string agentID;
    public string agentName;
    public string locationNodeID; 
    public AgentInventory AgInventoryID { get; set; }
    //public int inventoryID; 
    //public int money;
    
}
 */

[System.Serializable]
public class AgentString
{
    public string agentID;
    public string agentName;
    public string currentCityID;
    public string inventoryID;
    public string money;
    
}

[System.Serializable]
public class AgentListString
{
    public List<AgentString> agents;
}




