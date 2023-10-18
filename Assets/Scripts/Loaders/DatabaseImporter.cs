using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DatabaseImporter : MonoBehaviour
{
    private const string LifestyleDataPath = "Statics/LifestyleData";
    private List<LifestyleTier> lifestyleTiers;
    private const string ResourceDataPath = "Statics/ResourceData";
    private List<Resource> resources;

    private void Start()
    {
        LoadLifestyleData();
        LoadResourceData();
        LoadCityInventory();
    }

    private void LoadLifestyleData()
    {
        TextAsset jsonData = Resources.Load<TextAsset>(LifestyleDataPath);
        if (jsonData == null)
        {
            Debug.LogError("No es pot trobar el fitxer LifestyleData.json a la ruta especificada.");
            return;
        }
        LifestyleTier[] tempArray = JsonUtility.FromJson<LifestyleWrapper>(jsonData.text).Items;
        lifestyleTiers = new List<LifestyleTier>(tempArray);

        // Logs
            // Linia a linia
        /* foreach (var tier in lifestyleTiers)
        {
            Debug.Log($"Carregat lifestyle Tier {tier.TierID}, {tier.TierName}");
            foreach (var demand in tier.LifestyleDemands)
            {
                Debug.Log($"{demand.resourceType}, {demand.quantityPerThousand} {demand.monthsCritical} {demand.monthsTotal} {demand.resourceVariety}");
            }
        } */
        //Debug.Log("Llistats de LifestyleTier i LifestyleData carregats");
        Debug.Log($"Llistats de LifestyleTier i LifestyleData carregats. Total de LifestyleTiers: {lifestyleTiers.Count}");

    }

    private void LoadResourceData()
    {
        TextAsset jsonData = Resources.Load<TextAsset>(ResourceDataPath);
        if (jsonData == null)
        {
            Debug.LogError("No es pot trobar el fitxer ResourceData.json a la ruta especificada.");
            return;
        }
        resources = JsonUtility.FromJson<ListWrapper<Resource>>(jsonData.text).Items;

        // Crear HashSets per emmagatzemar tipus i subtipus únics
        HashSet<string> uniqueResourceTypes = new HashSet<string>();
        HashSet<string> uniqueResourceSubtypes = new HashSet<string>();
        
        // Log de linia a linia de recursos
        /* foreach (var resource in resources)
        {
            Debug.Log($"Carregat recurs: {resource.resourceID}, {resource.resourceName}, {resource.resourceType}, {resource.resourceSubtype}, {resource.basePrice}, {resource.baseWeight}");
        } */
        foreach (var resource in resources)
        {
            uniqueResourceTypes.Add(resource.resourceType);
            uniqueResourceSubtypes.Add(resource.resourceSubtype);
        }
        //Debug.Log("Llistat de recursos carregats");
        Debug.Log($"Llistat de recursos carregats. Total de recursos: {resources.Count}, Resource Types: {uniqueResourceTypes.Count}, Resource Subtypes: {uniqueResourceSubtypes.Count}");
    }
    
    private void LoadCityInventory()
    {
        string path = "Assets/Resources/StartValues/CityInventories";
        string[] files = Directory.GetFiles(path, "*.json");
        
        foreach (string file in files)
        {
            string jsonContent = File.ReadAllText(file);
            CityInventoryWrapper wrapper = JsonConvert.DeserializeObject<CityInventoryWrapper>(jsonContent);
            //Debug.Log($"Llegint el fitxer: {Path.GetFileName(file)}");
            //Debug.Log($"Contingut del fitxer: {jsonContent}");
            
            if (wrapper != null && wrapper.Items != null)
            {
                foreach (CityInventory cityInventory in wrapper.Items)
                {
                    int totalItems = cityInventory.InventoryItems.Count;
                    float totalQuantity = 0;

                    //Debug.Log($"CityInvID: {cityInventory.CityInvID}, CityID: {cityInventory.CityID}, CityInvMoney: {cityInventory.CityInvMoney}");
                    foreach (CityInventoryItem item in cityInventory.InventoryItems)
                    {
                        totalQuantity += item.Quantity;
                        //Debug.Log($"ResourceID: {item.ResourceID}, Quantity: {item.Quantity}, CurrentValue: {item.CurrentValue}");
                    }
                    Debug.Log($"CityInvID: {cityInventory.CityInvID}, CityID: {cityInventory.CityID}, CityInvMoney: {cityInventory.CityInvMoney}, {totalQuantity} unitats de recursos en  {totalItems} línies. ");
                    
                }
            }
           
        }
        Debug.Log("Llistat d'inventaris de ciutat carregats");
    }

    
    


    [System.Serializable]
    private class ListWrapper<T>
    {
        public List<T> Items;
    }
}

[System.Serializable]
public class LifestyleWrapper
{
    public LifestyleTier[] Items;
}

[System.Serializable]
public class CityInventoryWrapper
{
    public List<CityInventory> Items;
}

