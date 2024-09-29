using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ús intern a moltes funcions
    public CityData currentCity { get; private set; }
    public CityInventory currentCityInventory { get; private set; }
    public Agent currentAgent { get; private set; }
    public AgentInventory currentAgentInventory { get; private set; }

    // Referencies a managers
    public MarkersManager markersManager;

    
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        AssignCurrentCity("C0001");
        
        
    }

    void Start()
    {
        AssignCurrentCity("C0001");
        AssignCurrentAgent("AG0003");
        Debug.Log($"Començant fase del GameManager");
        
        // Subscripció als esdeveniments de GlobalTime
        GlobalTime.Instance.OnDayChanged += HandleDayChanged;
        GlobalTime.Instance.OnMonthChanged += HandleMonthChanged;
        GlobalTime.Instance.OnYearChanged += HandleYearChanged;

        // Iterar sobre cada edifici de la ciutat actual
        foreach (var building in currentCity.Buildings)
        {
            if (building is ProductiveBuilding productiveBuilding)
            {
                ProductionManager.Instance.KickstartProductives(productiveBuilding);
            }
        }
        
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
        ProductionManager.Instance.DebugUpdateProduction();

        // Iterar sobre cada edifici de la ciutat actual
        foreach (var building in currentCity.Buildings)
        {
            if (building is ProductiveBuilding productiveBuilding)
            {
                // Calcular els cicles disponibles per a aquest edifici productiu
                int availableCycles = ProductionManager.Instance.CalculateAvailableProductionCycles(productiveBuilding);

                // Mostrar el resultat en el log
                Debug.Log($"Edifici: {productiveBuilding.BuildingName}, Cicles disponibles: {availableCycles}");
            }
        }
        
        //markersManager.MoveAllAgents();
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
        currentCity = DataManager.Instance.GetCityDataByID(cityID);
        if (currentCity != null)
        {
            Debug.Log($"La ciutat assignada és '{currentCity.Name}'");
        }
        else
        {
            Debug.LogError($"No s'ha trobat cap ciutat amb l'ID inicial");
        }
    }
    public void AssignCurrentAgent(string agentID)
    {
        currentAgent = DataManager.Instance.GetAgentByID(agentID);
        if (currentAgent != null)
        {
            currentAgentInventory = currentAgent.Inventory;
            //Debug.Log($"Agent assignat és '{CurrentAgent.agentName}'");
            
            // Log llarg, per veure que es carrega bé tot. 
            float totalResourcesQuantity = currentAgentInventory.InventoryResources.Sum(resource => resource.Quantity);
            int resourceLinesCount = currentAgentInventory.InventoryResources.Count;

            Debug.Log($"Agent assignat és '{currentAgent.agentName}'. " +
                    $"Diners: {currentAgentInventory.InventoryMoney}, " +
                    $"Línies de recursos: {resourceLinesCount}, " +
                    $"Suma de quantitat de recursos: {totalResourcesQuantity}");
            }
        else
        {
            Debug.LogError($"No s'ha trobat cap agent amb l'ID '{agentID}'");
        }
    }

    
}
