using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<InventoryList> inventories;

    void Start() {
        LoadInventories();
    }

    public void LoadInventories()
    {
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("DDBB_Inventory");
        if (jsonFiles.Length == 0)
        {
            Debug.Log("No s'han trobat fitxers JSON dins de DDBB_Inventory.");
            return;
        }
        foreach (TextAsset jsonFile in jsonFiles)
        {
            Debug.Log($"Processant fitxer {jsonFile.name}...");
            InventoryList inventory = JsonUtility.FromJson<InventoryList>(jsonFile.text);
            inventories.Add(inventory);
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

}
