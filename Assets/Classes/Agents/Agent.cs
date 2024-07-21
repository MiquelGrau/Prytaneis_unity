using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Agent
{
    public string agentID;
    public string agentName;   // wil get the main character's name, probably
    
    // Propietats relacionals
    public string LocationNode; 
    // Owner
    
    // Propietats de contingut
    public string AgentInventoryID; 
    public AgentInventory Inventory; 
    public string MainCharID; 
    public Character MainCharacter; 
    //public List<Character> CompanionCharacters; 
    //public List<Vehicle> VehicleList; 
    
    // Propietats de viatge
    public string TravelMode; // "land", "water", "messenger", "star"
    public float speed; 
    public AgentTravel Travel; 
    public List<AgentTravel> NextTravelSteps; 



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
        this.speed = 20.0f; // Velocitat fixa inicial
        this.Travel = null; // Inicialitza la propietat Travel
        this.NextTravelSteps = new List<AgentTravel>(); // Inicialitza la llista de Travel
    }

    

    // Pots afegir aquí mètodes específics d'Agent si és necessari
}

[System.Serializable]
public class AgentTravel
{
    public string TravelID;
    public string Current;
    public string Destination;
    public string PathID;
    public float LengthDone;
    public float LengthTotal;
    public float DaysTotal;
    public float DaysDone;
    public bool Started;

    // Constructor per inicialitzar les propietats de AgentTravel amb current i destination
    public AgentTravel(string current, string destination)
    {
        this.TravelID = ""; 
        this.Current = current;
        this.Destination = destination;
        this.PathID = "";
        this.LengthDone = 0.0f;
        this.LengthTotal = 0.0f;
        this.DaysTotal = 0.0f;
        this.DaysDone = 0.0f;
        this.Started = false;
    }

    // Constructor per inicialitzar les propietats de AgentTravel amb pathID
    public AgentTravel(string pathID)
    {
        this.TravelID = ""; 
        this.Current = "";
        this.Destination = "";
        this.PathID = pathID;
        this.LengthDone = 0.0f;
        this.LengthTotal = 0.0f;
        this.DaysTotal = 0.0f;
        this.DaysDone = 0.0f;
        this.Started = false;
    }
}

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
public class AgentList
{
    //public List<Agent> agents;
    public List<Agent> agents = new List<Agent>();
}




