using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<InventoryList> inventories;

    [System.Serializable]
    public class JsonInventories
    {
        public List<InventoryListString> inventory_jsonfile;
    }
    
    void Start() {
        LoadInventories();
    }

    public void LoadInventories()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("DDBB_Inventory/Inventory_initial");
        if (jsonFile == null)
        {
            Debug.LogError("No s'ha trobat el fitxer JSON d'inventari.");
            return;
        }
        
        Debug.Log($"Processant fitxer {jsonFile.name}...");
        JsonInventories jsonInventories = JsonUtility.FromJson<JsonInventories>(jsonFile.text);
        if (jsonInventories == null || jsonInventories.inventory_jsonfile == null)
        {
            Debug.LogError("Error en la deserialització del JSON. La llista de InventoryListString és nul·la.");
            return;
        }
        else
        {
            Debug.Log("Fitxer Json carregat, diu això:" + jsonInventories.inventory_jsonfile);
        }
        

        inventories = new List<InventoryList>();    // a zero, sense res. Li afegirem ara
        
        //foreach (var inventoryListString in allInventories.inventory_jsonfile)
        foreach (var inventoryListString in jsonInventories.inventory_jsonfile)
        {
            InventoryList inventoryList = new InventoryList(int.Parse(inventoryListString.inventoryID));
            Debug.Log("Inventory ID: " + inventoryListString.inventoryID);
            foreach (var inventoryItemString in inventoryListString.inventoryitems)
            {
                InventoryList.InventoryItem inventoryItem = new InventoryList.InventoryItem(
                    int.Parse(inventoryItemString.resourceID),
                    int.Parse(inventoryItemString.quantity),
                    int.Parse(inventoryItemString.currentPrice)
                    
                );
                inventoryList.inventoryitems.Add(inventoryItem);
                
                Debug.Log("Resource ID: " + inventoryItemString.resourceID);
                Debug.Log("Quantity: " + inventoryItemString.quantity);
                Debug.Log("Current Price: " + inventoryItemString.currentPrice);
            }
            inventories.Add(inventoryList);
        }

        
    }

    

    // Funcions amb Inventaris

    public InventoryList GetCityInventory(CityData city)
    {
        return GetInventoryById(city.inventoryID);
    }

    public InventoryList GetInventoryById(int id)
    {
        return inventories.Find(inventory => inventory.inventoryID == id);
        
    }

    public void DebugPrintAllInventories()
    {
        if (inventories == null || inventories.Count == 0)
        {
            Debug.LogError("No s'ha carregat cap inventari");
            return;
        }

        Debug.Log("Total d'inventaris carregats: " + inventories.Count);
        
        foreach (var inventory in inventories)
        {
            if (inventory == null || inventory.inventoryitems == null)
            {
                Debug.LogError("Inventari nul o sense items");
                continue;  // passa a l'pròxim element en el bucle
            }
            Debug.Log($"Inventari ID: {inventory.inventoryID}, Nombre d'items: {inventory.inventoryitems.Count}");
            foreach (var item in inventory.inventoryitems)
            {
                Debug.Log($"Resource ID: {item.resourceID}, Quantity: {item.quantity}, Price: {item.currentPrice}");
            }
        }

    }

}
