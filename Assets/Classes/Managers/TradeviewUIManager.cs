using System.Collections.Generic;
using TMPro; 
using UnityEngine;
using UnityEngine.UI;

public class TradeviewUIManager : MonoBehaviour
{
    public TMP_Text barcelonaNameText;
    public TMP_Text barcelonaMoneyText;
    
    public TMP_Text allResourcesText;
    public TMP_Text citiesListText;
    public TMP_Text agentsListText;

    private InventoryList inventoryList;
    private InventoryManager inventoryManager;
    private GameManager gameManager;
    private CityDataManager cityDataManager;
    private AgentManager agentManager;
    
    


    private void Start()
    {
        cityDataManager = FindObjectOfType<CityDataManager>();
        agentManager = FindObjectOfType<AgentManager>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        UpdateUI();  // Actualitza la UI al començar
        
        gameManager = FindObjectOfType<GameManager>();

        
    }
    
    private void Update()
    {
        // Mostrar les llistes existents
        allResourcesText.text = AllResourcesToString();
        
        UpdateUI(); 
        
    }
    
    private void UpdateUI()
    {
        CityData barcelona = cityDataManager.dataItems.cities.Find(city => city.cityName == "Barcelona");
        
        citiesListText.text = AllCitiesToString();
        agentsListText.text = AllAgentsToString();

        barcelonaNameText.text = barcelona.cityName;
        barcelonaMoneyText.text = "Money: " + barcelona.money.ToString();
        
        
    }

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

}
