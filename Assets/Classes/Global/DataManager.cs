using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
        // Aquesta és la classe per guardar DADES del joc. Tot el que es vagi fent, comerç, moviment, etc, es volcarà aquí. 


public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    private string dataPath;

    // Classes estatiques, definicions
    public static List<LifestyleTier> lifestyleTiers;
    public static List<Resource> resourcemasterlist;
    public List<ProductiveTemplate> productiveTemplates = new List<ProductiveTemplate>();
    public List<CivicTemplate> civicTemplates = new List<CivicTemplate>();
    public List<ProductionMethod> productionMethods = new List<ProductionMethod>();
    public List<EmployeeFT> employeeFactors = new List<EmployeeFT>();
    public List<ResourceFT> resourceFactors = new List<ResourceFT>();
    


    // Geografia
    public static List<WorldMapNode> worldMapNodes = new List<WorldMapNode>();
    public static List<WorldMapLandPath> worldMapLandPaths = new List<WorldMapLandPath>();

    // Classes de city
    public CityDataList dataItems;
    public List<CityInventory> cityInventories;
    
    // BBDD d'edificis
    public int buildingCounter = 0;

    
    // Classes de agents, merchants, etc
    public List<Agent> agents = new List<Agent>();
    public List<AgentInventory> agentInventories;

    

    private void Awake()
    {
        // Bloc basic per a que no es perdi la variable en altres llocs. Crec, això diu el gepeto. 
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Això manté l'objecte viu entre canvis d'escena
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        TextAsset cityDataAsset = Resources.Load<TextAsset>("CityData");
        if(cityDataAsset != null)
        {
            dataItems = JsonConvert.DeserializeObject<CityDataList>(cityDataAsset.text);
            Debug.Log("Dades carregades: " + cityDataAsset.text);

            if(dataItems == null)
            {
                Debug.LogError("La deserialització ha fallat. Es pot que el format JSON no coincideixi amb l'estructura de dades esperada.");
            }
            else
            {
                CityDataList dataList = dataItems as CityDataList;
            }
        }
        else
        {
            dataItems = new CityDataList();
            Debug.LogError("No es pot trobar el fitxer CityData.json a la carpeta Resources.");
        }

        // Depuració per a confirmar la càrrega de dades
        Debug.Log($"Nombre de templates productius carregats: {productiveTemplates.Count}");
        Debug.Log($"Nombre de templates cívics carregats: {civicTemplates.Count}");
        
    }

    public void SaveData()
    {
        string json = JsonConvert.SerializeObject(dataItems, Formatting.Indented); // Use Formatting.Indented for pretty print
        File.WriteAllText(dataPath, json);
    }
    
    public List<CityData> GetCities()
    {
        if (dataItems is CityDataList cityDataList)
        {
            return cityDataList.cities;
        }
        else
        {
            Debug.LogError("DataManager no conté una instància de CityDataList.");
            return new List<CityData>();
        }
    }
    public CityData GetCityDataByID(string cityID)  // Te la demanaran mil vegades, millor tenir això aqui dins
    {
        return dataItems.cities.FirstOrDefault(city => city.cityID == cityID);
    }

    public List<Agent> GetAgents()
    {
        return agents;
    }
    public Agent GetAgentByID(string agentID)
    {
        return agents.FirstOrDefault(agent => agent.agentID == agentID);
    }

    public string GenerateBuildingID()
    {
        buildingCounter++;
        return $"B{buildingCounter.ToString().PadLeft(5, '0')}";
    }

    /* public List<Agent> GetAgents()
    {
        if (agents is AgentList agentList)
        {
            return agentList.agents;
        }
        else
        {
            Debug.LogError("DataManager no conté una instància de Agents.");
            return new List<Agent>();
        }
    } */


}
