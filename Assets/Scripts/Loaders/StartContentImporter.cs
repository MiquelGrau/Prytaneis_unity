using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System;

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
        ImportMineralResources();
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
                building.BuildingID = DataManager.Instance.GenerateBuildingID();
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
                building.BuildingID = DataManager.Instance.GenerateBuildingID();
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

    public void ImportMineralResources()
    {
        string path = "Assets/Resources/StartValues/MineralDeposits";
        foreach (string file in Directory.GetFiles(path, "*.json"))
        {
            string jsonContent = File.ReadAllText(file);
            MineralNodeWrapper depositData = JsonUtility.FromJson<MineralNodeWrapper>(jsonContent);
            
            foreach (var node in depositData.Nodes)
            {
                WorldMapNode landNode = DataManager.Instance.FindNodeById(node.NodeID);
                if (landNode != null)
                {
                    Debug.Log($"Recursos minerals creats al node ID {landNode.id}, nom {landNode.name}");
                    foreach (var deposit in node.Deposits)
                    {
                        int[] reservesBySlot = DistributeReserves(deposit.TotalReserves, deposit.SlotQty);
                        for (int slotIndex = 0; slotIndex < reservesBySlot.Length; slotIndex++)
                        {
                            int slotReserves = reservesBySlot[slotIndex];
                            var (reservesByDepth, purityLevels) = DistributeDepthsAndPurities(deposit.AvgDepth, deposit.AvgPurity, slotReserves);
                            //int[] reservesByDepth = DistributeDepths(deposit.AvgDepth, slotReserves);
                            //int[] purityLevels = DistributePurities(deposit.AvgPurity, modifiedDepth);
                            
                            MineralResource resource = new MineralResource(
                                deposit.MineralID,
                                slotIndex,
                                slotReserves,
                                reservesByDepth,
                                purityLevels
                            );

                            // Afegir el recurs mineral al node de terra
                            landNode.NodeDeposits.Add(resource);      
                            
                            // Logging for debugging
                            Debug.Log($"Mineral ID {deposit.MineralID}, Slot {slotIndex + 1}: ");
                            for (int depth = 0; depth < reservesByDepth.Length; depth++)
                            {
                                Debug.Log($"Depth {depth + 1}: {reservesByDepth[depth]} reserves, {purityLevels[depth]}% purity");
                            }              
                        }
                    }
                }
            }
        }
    }


    private int[] DistributeReserves(int totalReserves, int slotQuantity)
    {
        int[] reserves = new int[slotQuantity];
        int totalAssigned = 0;

        int minPerSlot = totalReserves / (slotQuantity + 2);    // Facil de canviar les densitats, amb això
        int maxPerSlot = totalReserves / Math.Max(slotQuantity - 1, 1);

        Debug.Log($"Iniciant la distribució de reserves: totalReserves = {totalReserves}, slotQuantity = {slotQuantity}");

        for (int i = 0; i < slotQuantity - 1; i++)
        {
            int maxAssignable = Math.Min(maxPerSlot, totalReserves - totalAssigned - (slotQuantity - i - 1) * minPerSlot);
            int minAssignable = Math.Max(minPerSlot, totalReserves - totalAssigned - (slotQuantity - i - 1) * maxPerSlot);
            int reserve = UnityEngine.Random.Range(minAssignable, maxAssignable + 1);

            
            reserves[i] = reserve;
            totalAssigned += reserve;
            Debug.Log($"Slot {i + 1}: Assignat {reserve} reserves. MinAssignable = {minAssignable}, MaxAssignable = {maxAssignable}");
        }

        // Assigna la resta al darrer slot, garantint que la suma total és igual a TotalReserves
        reserves[slotQuantity - 1] = totalReserves - totalAssigned;
        Debug.Log($"Slot {slotQuantity}: Assignat {reserves[slotQuantity - 1]} reserves (darrer slot).");

        return reserves;
    }

    private int[] DistributeDepths(float averageDepth, int totalReserves)
    {
        int[] reservesByDepth = new int[4];
        
        float modifiedDepth = averageDepth + UnityEngine.Random.Range(-0.5f, 0.5f);
        modifiedDepth = Mathf.Clamp(modifiedDepth, 1f, 4f); // Assegurar-se que està dins els límits de profunditat
        Debug.Log($"Original averageDepth: {averageDepth}, Modified averageDepth: {modifiedDepth}");

        float totalWeight = 0f;
        float[] weights = new float[4];

        for (int i = 0; i < 4; i++)
        {
            // Distribució triangular com a aproximació de la normal
            weights[i] = Mathf.Max(0f, 1f - Mathf.Abs(i + 1 - modifiedDepth));
            totalWeight += weights[i];
        }

        // Normalitzar i assignar reserves
        int assignedReserves = 0;
        for (int i = 0; i < 3; i++) // Assigna a totes menys l'última profunditat
        {
            weights[i] /= totalWeight;
            int assigned = Mathf.RoundToInt(totalReserves * weights[i]);
            reservesByDepth[i] = assigned;
            assignedReserves += assigned;
            Debug.Log($"Depth {i + 1}: Assigned {assigned} reserves with weight {weights[i]}");
        }
        // Asegura que la suma total de reserves és correcta
        reservesByDepth[3] = totalReserves - assignedReserves;
        Debug.Log($"Depth 4: Assigned {reservesByDepth[3]} reserves (last depth).");

        return reservesByDepth;
    }

    private int[] DistributePurities(float averagePurity, float modifiedDepth)
    {
        int depths = 4;
        float adjustedPurity = Mathf.Clamp(averagePurity + UnityEngine.Random.Range(-25.0f, 25.0f), 10f, 100f);
        int[] purityLevels = new int[4];
        Debug.Log($"Adjusted Purity based on averagePurity {averagePurity}: {adjustedPurity}");

        // Calcular la puresa a cada profunditat
        for (int i = 0; i < depths; i++)
        {
            // Ponderació basada en la proximitat al modifiedDepth ajustat
            float distance = Mathf.Abs(modifiedDepth - (i + 1));
            purityLevels[i] = Mathf.RoundToInt(adjustedPurity - (distance / 6.0f * adjustedPurity)); // disminuit effecte
            purityLevels[i] = Mathf.Clamp(purityLevels[i], 10, 100); // Repassar el maxim un altre cop
            Debug.Log($"Depth {i + 1}: {purityLevels[i]}% purity");
        }
        
        return purityLevels;
    }

    private (int[], int[]) DistributeDepthsAndPurities(float averageDepth, float averagePurity, int totalReserves)
    {
        int depths = 4; // Les profunditats marcades, si es vol modificar és aquí. 
        // Els dos arrays amb valors que busquem
        int[] reservesByDepth = new int[depths];
        int[] purityLevels = new int[depths];

        float modifiedDepth = averageDepth + UnityEngine.Random.Range(-0.5f, 0.5f);
        modifiedDepth = Mathf.Clamp(modifiedDepth, 1f, 4f); // Assegurar-se que està dins els límits de profunditat
        float adjustedPurity = Mathf.Clamp(averagePurity + UnityEngine.Random.Range(-25.0f, 25.0f), 10f, 100f);

        Debug.Log($"Modificadors: AvgDepth: {averageDepth}, Modified: {modifiedDepth}; AvgPurity:{averagePurity}, Modified: {adjustedPurity}");
        
        float totalWeight = 0f;
        float[] weights = new float[depths];
        
        float sigma = 1.0f; // Ajust de la desviació estàndard
        for (int i = 0; i < depths; i++)
        {
            float x = i + 1;
            //weights[i] = Mathf.Max(0f, 1f - Mathf.Abs(i + 1 - modifiedDepth));
            weights[i] = Mathf.Exp(-0.5f * Mathf.Pow((x - modifiedDepth) / sigma, 2));  // distribució normal, amb avgdst sobre el centre
            totalWeight += weights[i];
        }

        // Normalitzar i assignar reserves
        int assignedReserves = 0;
        for (int i = 0; i < depths; i++)
        {
            weights[i] /= totalWeight;
            int assigned = Mathf.RoundToInt(totalReserves * weights[i]);
            reservesByDepth[i] = assigned;
            assignedReserves += assigned;

            // Ponderació basada en la proximitat al modifiedDepth ajustat
            float distance = Mathf.Abs(modifiedDepth - (i + 1));
            purityLevels[i] = Mathf.RoundToInt(adjustedPurity - (distance / 6.0f * adjustedPurity));
            purityLevels[i] = Mathf.Clamp(purityLevels[i], 10, 100);

            //Debug.Log($"Depth {i + 1}: Assigned {assigned} reserves with weight {weights[i]}, {purityLevels[i]}% purity");
        }

        // Asegura que la suma total de reserves és correcta
        reservesByDepth[3] = totalReserves - assignedReserves;
        //Debug.Log($"Depth 4: Assigned {reservesByDepth[3]} reserves (last depth), {purityLevels[3]}% purity");

        return (reservesByDepth, purityLevels);
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

[System.Serializable]
public class MineralNodeWrapper
{
    public List<NodeDepositWrapper> Nodes;
}

[System.Serializable]
public class NodeDepositWrapper
{
    public string NodeID;
    public List<DepositWrapper> Deposits;
}

[System.Serializable]
public class DepositWrapper
{
    public string MineralID;
    public int SlotQty;
    public int TotalReserves;
    public float AvgDepth;
    public float AvgPurity;
}

