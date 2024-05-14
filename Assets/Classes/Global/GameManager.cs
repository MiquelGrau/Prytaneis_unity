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

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        Debug.Log($"Començant fase del GameManager");
        AssignCurrentCity("C0001");
        
        // Subscripció als esdeveniments de GlobalTime
        GlobalTime.Instance.OnDayChanged += HandleDayChanged;
        GlobalTime.Instance.OnMonthChanged += HandleMonthChanged;
        GlobalTime.Instance.OnYearChanged += HandleYearChanged;
    }

    void OnDestroy()
    {
        // Desubscripció dels esdeveniments de GlobalTime
        if (GlobalTime.Instance != null)
        {
            GlobalTime.Instance.OnDayChanged -= HandleDayChanged;
            GlobalTime.Instance.OnMonthChanged -= HandleMonthChanged;
            GlobalTime.Instance.OnYearChanged -= HandleYearChanged;
        }
    }

    
    private void HandleDayChanged()
    {
        Debug.Log($"Un nou dia ha començat: {GlobalTime.Instance.GetCurrentDate()}");
    }

    private void HandleMonthChanged()
    {
        Debug.Log($"Un nou mes ha començat: {GlobalTime.Instance.GetCurrentDate()}");
    }

    private void HandleYearChanged()
    {
        Debug.Log($"Un nou any ha començat: {GlobalTime.Instance.GetCurrentDate()}");
    }
    
    public void AssignCurrentCity(string cityID)
    {
        CurrentCity = DataManager.Instance.GetCityDataByID(cityID);
        if (CurrentCity != null)
        {
            Debug.Log($"La ciutat assignada és '{CurrentCity.cityName}'");
        }
        else
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
