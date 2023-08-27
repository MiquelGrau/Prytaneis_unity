using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Agent
{
    public int agentID;
    public string agentName;
    public string currentCityID; 
    public int inventoryID; 
    public int money;
    
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
public class AgentListString
{
    public List<AgentString> agents;
}




