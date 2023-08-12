using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Agent
{
    public int agentID;
    public string agentName;
    public int currentCityID; // Canviat a un tipus int per emmagatzemar la ID de la ciutat actual
    public List<InventoryItem> agentInventory = new List<InventoryItem>();
    public int money;


    
}

