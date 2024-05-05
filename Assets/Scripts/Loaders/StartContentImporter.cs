using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class StartContentImporter : MonoBehaviour
{
    private DataManager dataManager;

    private void Awake()
    {   
        // Obtenir la referència a DataManager
        dataManager = FindObjectOfType<DataManager>();
        if (dataManager == null)
        {
            Debug.LogError("DataManager no trobat en la escena.");
            return;
        }
        LoadCityInventory();
        LoadStartAgents();
        LoadAgentInventories();
        ImportBuildings();
        
    }

    private void Start()
    {
        ConnectCityAndCityInv();
        ConnectAgentAndAgentInv();
        Debug.Log("Acabada fase Start de l'importador! Connectats inventaris a Ciutats i a Agents");
    }

    private void LoadCityInventory()
    {
        string path = "Assets/Resources/StartValues/CityInventories";
        string[] files = Directory.GetFiles(path, "*.json");
        
        dataManager.cityInventories = new List<CityInventory>();
    
        foreach (string file in files)
        {   
            string jsonContent = File.ReadAllText(file);
            CityInventoryWrapper wrapper = JsonConvert.DeserializeObject<CityInventoryWrapper>(jsonContent);
            
            if (wrapper != null && wrapper.Items != null)
            {
                foreach (CityInventory cityInventory in wrapper.Items)
                {
                    dataManager.cityInventories.Add(cityInventory); // Afegeix cada CityInventory a la llista
                    
                    int totalResLines = cityInventory.InventoryResources.Count;
                    float totalQuantity = 0;

                    //Debug.Log($"CityInvID: {cityInventory.CityInvID}, CityID: {cityInventory.CityID}, CityInvMoney: {cityInventory.CityInvMoney}");
                    foreach (var resline in cityInventory.InventoryResources)
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
    
    private void LoadStartAgents()
    {
        //agents = new List<Agent>();
        string path = Path.Combine(Application.dataPath, "Resources/StartValues/Agents");
        List<Agent> loadedAgents = new List<Agent>();

        foreach (string file in Directory.GetFiles(path, "*.json"))
        {
            string jsonContent = File.ReadAllText(file);
            AgentListWrapper wrapper = JsonConvert.DeserializeObject<AgentListWrapper>(jsonContent);

            if (wrapper != null && wrapper.agents != null)
            {
                foreach (var agent in wrapper.agents)
                {
                    loadedAgents.Add(agent); // Afegeix cada agent a la llista
                    // Afegir un log per a cada agent carregat
                    /* Debug.Log($"Agent carregat: ID={agent.agentID}, Nom={agent.agentName}, " +
                            $"LocationNode={agent.LocationNode}, InventoryID={agent.AgentInventoryID}, " +
                            $"MainCharID={agent.MainCharID}"); */
                }
            }
        }
        dataManager.agents = loadedAgents;
        Debug.Log($"Total d'agents carregats: {dataManager.agents.Count}");
    }
    
    private void LoadAgentInventories()
    {
        //agentInventories = new List<AgentInventory>();
        string path = Path.Combine(Application.dataPath, "Resources/StartValues/AgentInventories");
        List<AgentInventory> loadedAgentInventories = new List<AgentInventory>();

        foreach (string file in Directory.GetFiles(path, "*.json"))
        {
            string jsonContent = File.ReadAllText(file);
            AgentInventoryWrapper wrapper = JsonConvert.DeserializeObject<AgentInventoryWrapper>(jsonContent);

            if (wrapper != null && wrapper.Items != null)
            {
                foreach (var agentInventory in wrapper.Items)
                {
                    loadedAgentInventories.Add(agentInventory); // Afegeix cada AgentInventory a la llista

                    // Afegir un log per a cada AgentInventory carregat
                    /* Debug.Log($"AgentInventory carregat: InventoryID={agentInventory.InventoryID}, " +
                              $"AgentID={agentInventory.AgentID}, InventoryMoney={agentInventory.InventoryMoney}, " +
                              $"Recursos={agentInventory.InventoryResources.Count}"); */
                }
            }
        }
        dataManager.agentInventories = loadedAgentInventories;
        Debug.Log($"Total d'inventaris d'agents carregats: {dataManager.agentInventories.Count}");
    }

    private void ConnectCityAndCityInv()
    {
        Debug.Log("ConnectCityAndCityInv: Començant a connectar ciutats");
        
        var cities = dataManager.GetCities();
        if (cities == null || cities.Count == 0)
        {
            Debug.LogError("ConnectCityAndCityInv: La llista de ciutats és nul·la o buida.");
            return;
        }
        foreach (var cityData in cities)
        {
            //Debug.Log($"ConnectCityAndCityInv: Processant la ciutat {cityData.cityName} amb ID d'inventari {cityData.cityInventoryID}");

            // Troba l'objecte CityInventory que coincideix amb la cityInventoryID de CityData
            var matchingInventory = dataManager.cityInventories.FirstOrDefault(ci => ci.CityInvID == cityData.cityInventoryID);
            if (matchingInventory != null)
            {
                // Estableix la referència de CityData a CityInventory
                cityData.CityInventory = matchingInventory;

                // Mostra un missatge indicant que la connexió ha estat exitosa
                //Debug.Log($"Connexió ciutat-inventari: {cityData.cityID} {cityData.cityName}, Inventari: {matchingInventory.CityInvID}");
            }
            else
            {
                Debug.LogError($"No s'ha trobat Inventari amb ID {cityData.cityInventoryID}" +
                    $"per la ciutat {cityData.cityName}");
            }
        }
    }

    private void ConnectAgentAndAgentInv()
    {
        Debug.Log("ConnectAgentAndAgentInv: Començant a connectar agents amb els seus inventaris");
        
        var agents = dataManager.GetAgents(); 
        if (agents == null || agents.Count == 0)
        {
            Debug.LogError("ConnectAgentAndAgentInv: La llista d'agents és nul·la o buida.");
            return;
        }

        foreach (var agent in agents)
        {
            //Debug.Log($"ConnectAgentAndAgentInv: Processant l'agent {agent.agentName} amb ID d'inventari {agent.AgentInventoryID}");

            // Troba l'objecte AgentInventory que coincideix amb l'AgentInventoryID de l'Agent
            var matchingInventory = dataManager.agentInventories.FirstOrDefault(ai => ai.InventoryID == agent.AgentInventoryID);
            if (matchingInventory != null)
            {
                // Estableix la referència d'Agent a AgentInventory
                agent.Inventory = matchingInventory;

                // Mostra un missatge indicant que la connexió ha estat exitosa
                //Debug.Log($"Connexió agent-inventari: AgentID={agent.agentID}, AgentNom={agent.agentName}, InventariID={matchingInventory.InventoryID}");
            }
            else
            {
                Debug.LogError($"No s'ha trobat Inventari amb ID {agent.AgentInventoryID} per l'agent {agent.agentName}");
            }
        }
    }


    private void ImportBuildings()
    {
        string path = "Assets/Resources/StartValues/Buildings";
        foreach (string file in Directory.GetFiles(path, "*.json"))
        {
            string jsonContent = File.ReadAllText(file);
            ImportCityBuildings(jsonContent);
            ImportSettlementBuildings(jsonContent);
        }
    }

    private void ImportCityBuildings(string jsonContent)
    {
        var wrapper = JsonConvert.DeserializeObject<Dictionary<string, List<Building>>>(jsonContent);
        if (wrapper.TryGetValue("CityBuildings", out List<Building> buildings))
        {
            foreach (var building in buildings)
            {
                CityData city = FindCityByID(building.BuildingLocation);
                if (city != null)
                {
                    city.CityBuildings.Add(building);
                    Debug.Log($"Building added to city {city.cityName}: {building.BuildingName}");
                }
            }
        }
    }
    private void ImportSettlementBuildings(string jsonContent)
    {
        var wrapper = JsonConvert.DeserializeObject<Dictionary<string, List<Building>>>(jsonContent);
        if (wrapper.TryGetValue("SettlementBuildings", out List<Building> buildings))
        {
            foreach (var building in buildings)
            {
                Settlement settlement = FindSettlementByID(building.BuildingLocation);
                if (settlement != null)
                {
                    settlement.SettlBuildings.Add(building);
                    Debug.Log($"Building added to settlement {settlement.settlementName}: {building.BuildingName}");
                }
            }
        }
    }

    private CityData FindCityByID(string id)
    {
        return DataManager.Instance.allCityList.FirstOrDefault(c => c.cityID == id);
    }

    private Settlement FindSettlementByID(string id)
    {
        return DataManager.Instance.allSettlementList.FirstOrDefault(s => s.settlementID == id);
    }



}


[System.Serializable]
public class CityInventoryWrapper
{
    public List<CityInventory> Items;
}

[System.Serializable]
public class AgentListWrapper
{
    public List<Agent> agents;
}

[System.Serializable]
public class AgentInventoryWrapper
{
    public List<AgentInventory> Items;
}

