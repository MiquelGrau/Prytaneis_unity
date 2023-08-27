using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class CityInventoryManager : MonoBehaviour
{
    public List<CityInventoryList> cityInventories;

    void Start() {
        LoadCityInventoriesFromJSON();
    }


    public void LoadCityInventoriesFromJSON()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("DDBB_Inventory/cityinventoryfile"); 
        if (jsonText == null)
        {
            Debug.LogError("Error: No s'ha trobat el fitxer JSON cityinventoryfile.");
            return;
        }

        CityInventoryImport importedData = JsonUtility.FromJson<CityInventoryImport>(jsonText.text);
        if (importedData == null || importedData.inventory_jsonfile == null)
        {
            Debug.LogError("Error en processar les dades JSON.");
            return;
        }
        Debug.Log($"Inventaris a carregar: {importedData.inventory_jsonfile.Count}");

        
        foreach (var importedList in importedData.inventory_jsonfile)
        {
            CityInventoryList newInventoryList = new CityInventoryList(importedList.inventoryID);

            foreach (var importedItem in importedList.inventoryitems)
            {
                CityInventoryList.CityInventoryItem newItem = new CityInventoryList.CityInventoryItem(
                    int.Parse(importedItem.resourceID),
                    "",
                    0,
                    int.Parse(importedItem.quantity),
                    int.Parse(importedItem.currentPrice),
                    0, 0, 0, 0
                );
                newInventoryList.cityInventoryItems.Add(newItem);
            }

            cityInventories.Add(newInventoryList);
        }
    }

    public CityInventoryList GetCityInventory(CityData city)
    {
        return GetCityInventoryById(city.cityInventoryID);
    }
    public CityInventoryList GetCityInventoryById(string id)
    {
        return cityInventories.Find(inventory => inventory.cityInventoryID == id);
    }

}


