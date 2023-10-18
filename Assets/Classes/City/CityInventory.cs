using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CityInventory
{
    public string CityInvID { get; set; }
    public string CityID { get; set; }  // double referenced, just in case
    public int CityInvMoney { get; set; }
    public List<CityInventoryItem> InventoryItems { get; set; }
    
    
    // Constructor
    public CityInventory(string cityInvID, string cityID, int cityInvMoney, List<CityInventoryItem> items)
    {
        CityInvID = cityInvID;
        CityID = cityID;
        CityInvMoney = cityInvMoney;
        InventoryItems = items ?? new List<CityInventoryItem>(); // Assigna una nova llista buida si items és null
    }
    
}



[System.Serializable]
public class CityInventoryItem
{
    public string ResourceID { get; set; }
    public string ResourceType { get; set; }
    public float Quantity { get; set; }
    public float DemandConsume { get; set; }
    public float DemandCritical { get; set; }
    public float DemandTotal { get; set; }
    public int VarietyAssigned { get; set; }
    public int BuyPrice { get; set; }
    public int SellPrice { get; set; }
    public int CurrentValue { get; set; }

    // Constructor
    public CityInventoryItem(string resourceId, float quantity, int currentValue)
    {
        ResourceID = resourceId;
        Quantity = quantity;
        CurrentValue = currentValue;
        
        // Inicialitzar la resta de propietats a zero o null
        ResourceType = null;
        DemandConsume = 0;
        DemandCritical = 0;
        DemandTotal = 0;
        VarietyAssigned = 0;
        BuyPrice = 0;
        SellPrice = 0;
    }
    
}
