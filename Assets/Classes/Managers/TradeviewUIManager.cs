using System.Collections.Generic;
using TMPro; 
using UnityEngine;
using UnityEngine.UI;

public class TradeviewUIManager : MonoBehaviour
{
    // Textos basics
    public TMP_Text barcelonaNameText;
    public TMP_Text barcelonaMoneyText;
    
    // Temporal mentre estem en proves
    public TMP_Text cityInventoryText;
    public TMP_Text allResourcesText;
    public TMP_Text citiesListText;
    public TMP_Text agentsListText;
    public TMP_Dropdown agentDropdown;
    public TMP_Dropdown cityDropdown;
    private List<Agent> allAgents = new List<Agent>(); 

    // Crides a les classes involucrades
    private InventoryList inventoryList;
    private InventoryManager inventoryManager;
    private GameManager gameManager;
    private CityDataManager cityDataManager;
    private AgentManager agentManager;
    
    // Visualitzacions d'inventaris de ciutat i agent
    public RectTransform cityResourceListContainer;
    public RectTransform agentResourceListContainer;

    private void Start()
    {
        cityDataManager = FindObjectOfType<CityDataManager>();
        agentManager = FindObjectOfType<AgentManager>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        inventoryManager.DebugPrintAllInventories();
        gameManager = FindObjectOfType<GameManager>();
        PopulateCityDropdown();
        PopulateAgentDropdown();
        
        UpdateUI();  // Actualitza la UI al començar
        
    }
    
    private void Update()
    {
        // Mostrar les llistes existents
        allResourcesText.text = AllResourcesToString();
        Debug.Log("Després de definir allResourcesText");  
        UpdateUI(); 
        Debug.Log("Després de cridar UpdateUI");  
    }
    
    private void UpdateUI()
    {
        CityData barcelona = cityDataManager.dataItems.cities.Find(city => city.cityName == "Barcelona"); // borrarem
        CityData currentCity = GetCurrentCity();
        Debug.Log("Després de definir barcelona"); 

        citiesListText.text = AllCitiesToString();
        agentsListText.text = AllAgentsToString();

        barcelonaNameText.text = currentCity.cityName;
        barcelonaMoneyText.text = "Money: " + currentCity.money.ToString();
        UpdateInventoryText(currentCity);
        
        Agent currentAgent = GetCurrentAgent();
        Debug.Log("Després de definir currentAgent");  
        
        var cityInventory = inventoryManager.GetCityInventory(currentCity);
        if (cityInventory == null)
        {
            Debug.LogError("cityInventory és null");
            return; // Retorna per evitar l'error NullReferenceException
        }
        if (cityInventory.inventoryitems == null)
        {
            Debug.LogError("cityInventory.inventoryitems és null");
            return; // Retorna per evitar l'error NullReferenceException
        }
        Debug.Log("Després de processar cityInventory");
        
        
        var agentInventory = inventoryManager.GetInventoryById(currentAgent.inventoryID);
        Debug.Log("Després de processar agentInventory"); 
        

    }

    // Convert agents - cities to text
    private string AllResourcesToString()
    {
        string result = "Recursos:\n";
        foreach(var resource in ResourceManager.AllResources)
        {
            result += resource.ToString() + "\n"; 
        }
        return result;
    }

    private string AllCitiesToString()
    {
        string result = "Ciutats:\n";
        if(cityDataManager != null && cityDataManager.dataItems != null && cityDataManager.dataItems.cities != null)
        {
            foreach (var city in cityDataManager.dataItems.cities)
            {
                result += city.cityName + "\n";
            }
        }
        return result;
    }

    private string AllAgentsToString()
    {
        string result = "Agents:\n";
        foreach (var agent in agentManager.agents)
        {
            CityData agentCity = cityDataManager.dataItems.cities.Find(c => c.cityID == agent.currentCityID);
            if(agentCity != null)
                result += agent.agentName + ", a " + agentCity.cityName + "\n";
        }
        //Debug.Log("Començant a generar la cadena d'agents. Nombre d'agents: " + agentManager.agents.Count);
        return result;
        
    }

    private void UpdateInventoryText(CityData city)
    {
        InventoryList inventory = inventoryManager.GetCityInventory(city);
        string inventoryString = "Inventory:\n";

        if (inventory == null || inventory.inventoryitems == null || inventory.inventoryitems.Count == 0)
        {
            inventoryString += "No items in inventory.";
        }
        else
        {
            foreach (var item in inventory.inventoryitems)
            {
                inventoryString += $"Resource {item.resourceID}, Quantity: {item.quantity}, Price: {item.currentPrice}\n";
            }
        }

        cityInventoryText.text = inventoryString;
    }


    // Information about cities, agents and inventories
    private void PopulateCityDropdown()
    {
        cityDropdown.ClearOptions();

        List<string> cityNames = new List<string>();
        foreach (var city in cityDataManager.dataItems.cities)
        {
            cityNames.Add(city.cityName);
        }

        cityDropdown.AddOptions(cityNames);
    }
    private CityData GetCurrentCity()
    {
        int index = cityDropdown.value;
        return cityDataManager.dataItems.cities[index];
    }
    
    private void PopulateAgentDropdown()
    {
        Debug.Log("AgentManager: " + agentManager);
        if (agentManager == null)
        {
            Debug.LogError("agentManager és null!");
            return;
        }

        allAgents = agentManager.GetAgents(); 
        
        if (allAgents == null)
        {
            Debug.LogError("allAgents és null després de cridar GetAgents!");
            return;
        }
        
        Debug.Log("Nombre d'agents: " + allAgents.Count);
        
        agentDropdown.ClearOptions();

        List<string> agentNames = new List<string>();
        foreach (Agent agent in allAgents)
        {
            agentNames.Add(agent.agentName); 
        }

        agentDropdown.AddOptions(agentNames);
    }
    private Agent GetCurrentAgent()
    {
        int index = agentDropdown.value;
        return agentManager.agents[index];
    }

    
    

}
