using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentInventory : Inventory
{
    //public string InventoryID { get; set; }       // propietats heredades de la generica Inventory
    //public int InventoryMoney { get; set; }
    //public List<InventoryResource> InventoryResources { get; set; }
    //public List<InventoryItem> InventoryItems { get; set; }
    public string AgentID { get; set; }
    public int Food { get; set; }
    public int Water { get; set; }
    public float CurrentCapacity { get; set; }
    public float MaxCapacity { get; set; }

    public AgentInventory()
    {
        // Inicialització de les propietats addicionals
        Food = 0;
        Water = 0;
        CurrentCapacity = 0f;
        MaxCapacity = 100f; // ja ho canviarem a que calculi segons vehicles
    }

    // Aquí pots afegir mètodes específics d'AgentInventory si és necessari
}


