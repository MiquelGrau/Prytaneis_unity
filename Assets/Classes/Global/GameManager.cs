using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ús intern a moltes funcions
    public CityData CurrentCity { get; private set; }
    public CityInventory CurrentCityInventory { get; private set; }
    public Agent CurrentAgent { get; private set; }
    public AgentInventory CurrentAgentInventory { get; private set; }

    
    void Start()
    {
        Debug.Log($"Començant fase del GameManager");
        AssignCurrentCity("C0001");
        
        
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        //AssignCurrentCity("C0001");
    }

    
    public void AssignCurrentCity(string cityID)
    {
        CurrentCity = DataManager.Instance.GetCityDataByID(cityID);
        Debug.Log($"La ciutat assignada és '{CurrentCity.cityName}'");
        if (CurrentCity == null)
        {
            Debug.LogError($"No s'ha trobat cap ciutat amb l'ID '{cityID}'");
        }
    }
    public void AssignCurrentAgent(string agentID)
    {
        CurrentAgent = DataManager.Instance.GetAgentByID(agentID);
        if (CurrentAgent != null)
        {
            CurrentAgentInventory = CurrentAgent.Inventory;
            //Debug.Log($"Agent assignat és '{CurrentAgent.agentName}'");
            
            // Log llarg, per veure que es carrega bé tot. 
            float totalResourcesQuantity = CurrentAgentInventory.InventoryResources.Sum(resource => resource.Quantity);
            int resourceLinesCount = CurrentAgentInventory.InventoryResources.Count;

            Debug.Log($"Agent assignat és '{CurrentAgent.agentName}'. " +
                    $"Diners: {CurrentAgentInventory.InventoryMoney}, " +
                    $"Línies de recursos: {resourceLinesCount}, " +
                    $"Suma de quantitat de recursos: {totalResourcesQuantity}");
            }
        else
        {
            Debug.LogError($"No s'ha trobat cap agent amb l'ID '{agentID}'");
        }
    }
}
