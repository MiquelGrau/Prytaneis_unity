using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class DatabaseImporter : MonoBehaviour
{
    private const string LifestyleDataPath = "Statics/LifestyleData";
    public List<LifestyleTier> lifestyleTiers;
    private const string ResourceDataPath = "Statics/ResourceData";
    public static List<Resource> resources;
    public List<CityData> cities;
    public List<CityInventory> cityInventories; 

    private void Awake()
    {
        LoadLifestyleData();
        LoadResourceData();
        cityInventories = new List<CityInventory>(); 
        LoadCityInventory();

        // Obté la referència de DataManager i carrega les ciutats
        DataManager dataManager = FindObjectOfType<DataManager>();
        if (dataManager != null)
        {
            cities = dataManager.GetCities();
            // Afegim el Debug.Log aquí per mostrar tot el contingut de la llista cities
            string citiesJson = JsonConvert.SerializeObject(cities, Formatting.Indented);
            Debug.Log("Ciutats carregades: " + citiesJson);
        }
        else
        {
            Debug.LogError("No s'ha trobat DataManager.");
        }

        Debug.Log("Acabada fase Awake de l'importador! Lifestyle, Resource, CityInventory");
    }

    private void Start()
    {
        ConnectCityAndCityInv();
        Debug.Log("Acabada fase Start de l'importador! Connectar City i CityInventory");
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
                    cityInventories.Add(cityInventory); // Afegeix cada CityInventory a la llista
                    
                    int totalResLines = cityInventory.InventoryResources.Count;
                    float totalQuantity = 0;

                    //Debug.Log($"CityInvID: {cityInventory.CityInvID}, CityID: {cityInventory.CityID}, CityInvMoney: {cityInventory.CityInvMoney}");
                    foreach (var resline in cityInventory.InventoryResources)
                    //foreach (CityInventoryItem resline in cityInventory.InventoryResources)
                    {
                        totalQuantity += resline.Quantity;
                        //Debug.Log($"ResourceID: {resline.ResourceID}, Quantity: {resline.Quantity}, CurrentValue: {resline.CurrentValue}");
                    }
                    Debug.Log($"CityInvID: {cityInventory.CityInvID}, CityID: {cityInventory.CityID}, CityInvMoney: {cityInventory.CityInvMoney}, {totalQuantity} unitats de recursos en  {totalResLines} línies. ");
                    
                }
            }

            
           
        }
        Debug.Log("Llistat d'inventaris de ciutat carregats");
    }

    private void ConnectCityAndCityInv()
    {
        Debug.Log("ConnectCityAndCityInv: Començant a connectar ciutats");
        
        if (cities == null || cities.Count == 0)
        {
            Debug.LogError("ConnectCityAndCityInv: La llista de ciutats és nul·la o buida.");
            return;
        }
        foreach (var cityData in cities)
        {
            Debug.Log($"ConnectCityAndCityInv: Processant la ciutat {cityData.cityName} amb ID d'inventari {cityData.cityInventoryID}");

            // Troba l'objecte CityInventory que coincideix amb la cityInventoryID de CityData
            var matchingInventory = cityInventories.FirstOrDefault(ci => ci.CityInvID == cityData.cityInventoryID);
            
            if (matchingInventory != null)
            {
                // Estableix la referència de CityData a CityInventory
                cityData.CityInventory = matchingInventory;

                // Mostra un missatge indicant que la connexió ha estat exitosa
                Debug.Log($"Connexió ciutat-inventari: {cityData.cityID} {cityData.cityName}, Inventari: {matchingInventory.CityInvID}");
            }
            else
            {
                Debug.LogError($"No s'ha trobat Inventari amb ID {cityData.cityInventoryID} per la ciutat {cityData.cityName}");
            }
        }
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

