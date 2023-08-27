using System.Collections.Generic;
using TMPro; 
using UnityEngine;
using UnityEngine.UI;

public class TradeviewUIManager : MonoBehaviour
{
    // Textos basics
    public TMP_Text currCityNameText;
    public TMP_Text currCityMoneyText;
    public TMP_Text cityInventoryText;
    private float updateInterval = 5f; // Interval d'actualització en segons
    private float timer = 0f; // Comptador de temps

    // Temporal mentre estem en proves
    public TMP_Text allResourcesText;
    public TMP_Text citiesListText;
    public TMP_Text agentsListText;
    public TMP_Dropdown agentDropdown;
    public TMP_Dropdown cityDropdown;
    private List<Agent> allAgents = new List<Agent>(); 

    // Crides a les classes involucrades
    private InventoryList inventoryList;
    private InventoryManager inventoryManager;
    private CityInventoryList cityInventoryList;
    private CityInventoryManager cityInventoryManager;
    private GameManager gameManager;
    private CityDataManager cityDataManager;
    private AgentManager agentManager;
    
    // Visualitzacions d'inventaris de ciutat i agent
    public GameObject tradeLinePrefab;
    public RectTransform cityResourceListContainer;
    public RectTransform agentResourceListContainer;

    private void Start()
    {
        cityDataManager = FindObjectOfType<CityDataManager>();
        agentManager = FindObjectOfType<AgentManager>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        cityInventoryManager = FindObjectOfType<CityInventoryManager>();
        //inventoryManager.DebugPrintAllInventories();
        gameManager = FindObjectOfType<GameManager>();
        PopulateCityDropdown();
        cityDropdown.onValueChanged.AddListener(delegate { OnCityDropdownChange(); });
        PopulateAgentDropdown();
        agentDropdown.onValueChanged.AddListener(delegate { OnAgentDropdownChange(); });
        
        UpdateUI();  // Actualitza la UI al començar
        
    }
    
    private void Update()
    {
        allResourcesText.text = AllResourcesToString();
        
        timer += Time.deltaTime; // Incrementa el comptador amb el temps transcorregut des de l'últim fotograma

        if (timer >= updateInterval) // Comprova si s'ha arribat a l'interval desitjat
        {
            UpdateUI(); // Actualitza la UI
            timer = 0f; // Reinicia el comptador
        }

        //UpdateUI(); 
        
    }
    
    public CityData CurrentCity 
    { 
        get 
        {
            return GetCurrentCity();
        }
    }
        
    private void UpdateUI()
    {

        
        CityData currentCity = GetCurrentCity();
        Agent currentAgent = GetCurrentAgent();
        
        citiesListText.text = AllCitiesToString();
        agentsListText.text = AllAgentsToString();

        currCityNameText.text = currentCity.cityName;
        currCityMoneyText.text = "Money: " + currentCity.money.ToString();
        UpdateInventoryText(currentCity);
        
        foreach (Transform child in cityResourceListContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in agentResourceListContainer)
        {
            Destroy(child.gameObject);
        }
       

        var cityInventory = cityInventoryManager.GetCityInventory(currentCity);
        if (cityInventory == null)
        {
            Debug.LogError("cityInventory nul");
            return; // Retorna per evitar l'error NullReferenceException
        }
        if (cityInventory.cityInventoryItems == null)
        {
            Debug.LogError("Sense existencies en aquest inventari (cityInventory.cityInventoryItems és nul)");
            return; // Retorna per evitar l'error NullReferenceException
        }
        foreach (var item in cityInventory.cityInventoryItems)
        {
            AddResourceLine(item, true);
        }
        //Debug.Log("Després de processar cityInventory");
        
        var agentInventory = inventoryManager.GetInventoryById(currentAgent.inventoryID);
        foreach (var item in agentInventory.inventoryitems)
        {
            AddResourceLine(item, false);
        }
        
        Debug.Log("UI Updated"); 
        
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
        CityInventoryList inventory = cityInventoryManager.GetCityInventory(city);
        string inventoryString = "Inventory:\n";

        if (inventory == null || inventory.cityInventoryItems == null || inventory.cityInventoryItems.Count == 0)
        {
            inventoryString += "No items in inventory.";
        }
        else
        {
            foreach (var item in inventory.cityInventoryItems)
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
    private void OnCityDropdownChange()
    {
        UpdateUI();
    }

    private void PopulateAgentDropdown()
    {
        
        allAgents = agentManager.GetAgents(); 
        
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
    private void OnAgentDropdownChange()
    {
        UpdateUI();
    }
    
    private void AddResourceLine(object item, bool isCity)
    {
        GameObject tradeLine = Instantiate(tradeLinePrefab);
        tradeLine.transform.SetParent(isCity ? cityResourceListContainer.transform : agentResourceListContainer.transform, false);
        TradeLineController tradeLineController = tradeLine.GetComponent<TradeLineController>();
        
        if (item is InventoryList.InventoryItem invItem)
        {
            tradeLineController.Setup(invItem);
        }
        else if (item is CityInventoryList.CityInventoryItem cityItem)
        {
            tradeLineController.Setup(cityItem);
        }
        else
        {
            Debug.LogError("Tipus d'item no reconegut");
        }
        
        
        //tradeLineController.Setup(item);
        //Debug.Log("Prefab instanciat i afegit al contenidor. Nom del recurs: " + tradeLineController.resourceNameText.text);
        
    }
    

}
