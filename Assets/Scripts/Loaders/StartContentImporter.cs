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
        // Hem de separar en dos passos, awake i start, o alguna cosa no s'executarà. 
        // P. ex. primer posar Cities, i després lo que vagi a dins, com Buildings o Governments. 
        LoadCityData();
        LoadCityInventory();
        LoadStartAgents();
        LoadAgentInventories();
        ImportMineralResources();
    }

    private void Start()
    {
        //ConnectCityAndCityInv();
        //ConnectAgentAndAgentInv();
        ImportBuildings();
        Debug.Log("Acabada fase Start de l'importador! Connectats inventaris a Ciutats i a Agents");
    }

    // #######################################################################################################
    // GEOGRAFIA HUMANA - ciutats, assentaments, inventaris, governs
    // #######################################################################################################
    private void LoadCityData()
    {
        //TextAsset cityDataAsset = Resources.Load<TextAsset>("CityData");
        string path = "Assets/Resources/StartValues/Locations";
        string[] files = Directory.GetFiles(path, "*.json");

        DataManager.Instance.allCityList = new List<CityData>();
        DataManager.Instance.allSettlementList = new List<Settlement>();

        foreach (string file in files)
        {
            string jsonContent = File.ReadAllText(file);
            if (jsonContent.Contains("\"City\""))
            {
            
                CityDataList cityDataList = JsonConvert.DeserializeObject<CityDataList>(jsonContent);
                
                if (cityDataList != null && cityDataList.Cities != null)
                {
                    foreach (var cityWrapper in cityDataList.Cities)
                    {
                        // Convertir el wrapper en una instància de CityData
                        CityData cityData = new CityData(
                            cityWrapper.LocID,
                            cityWrapper.Name,
                            cityWrapper.NodeID,
                            cityWrapper.InventoryID,
                            cityWrapper.PoorPopulation,
                            cityWrapper.MidPopulation,
                            cityWrapper.RichPopulation,
                            cityWrapper.PoorLifestyleID,
                            cityWrapper.MidLifestyleID,
                            cityWrapper.RichLifestyleID,
                            cityWrapper.OwnerID,    
                            cityWrapper.PoliticalStatus,
                            cityWrapper.BuildPoints
                        );

                        DataManager.Instance.allCityList.Add(cityData);

                    }

                    Debug.Log($"S'han carregat {cityDataList.Cities.Count} ciutats des de {file}.");
                }
                else { Debug.LogError($"No s'ha pogut deserialitzar el contingut de {file}"); }
            }
            else if (jsonContent.Contains("\"Settlement\""))
            {
                SettlementDataList settlementDataList = JsonConvert.DeserializeObject<SettlementDataList>(jsonContent);
                
                if (settlementDataList != null && settlementDataList.Settlements != null)
                {
                    foreach (var settlementWrapper in settlementDataList.Settlements)
                    {
                        Settlement settlementData = new Settlement(
                            settlementWrapper.Name,
                            settlementWrapper.LocID,
                            settlementWrapper.NodeID,
                            settlementWrapper.InventoryID,
                            settlementWrapper.SettlActivity,
                            settlementWrapper.Population,
                            settlementWrapper.SettlLifestyleID,
                            settlementWrapper.OwnerID,
                            settlementWrapper.PoliticalStatus,
                            settlementWrapper.BuildPoints
                        );

                        DataManager.Instance.allSettlementList.Add(settlementData);
                    }

                    Debug.Log($"S'han carregat {settlementDataList.Settlements.Count} settlements des de {file}.");
                }
                else { Debug.LogError($"No s'ha pogut deserialitzar el contingut de {file} com a SettlementData"); }
            }
            else { Debug.LogWarning($"El fitxer {file} no conté ni ciutats ni settlements reconeguts."); }
        }

        if (DataManager.Instance.allCityList.Count == 0)
                { Debug.LogError("No s'ha pogut carregar cap ciutat des dels fitxers JSON."); }
        else    { Debug.Log($"S'han carregat un total de {DataManager.Instance.allCityList.Count} ciutats."); }

        if (DataManager.Instance.allSettlementList.Count == 0)
                { Debug.LogError("No s'ha pogut carregar cap settlement des dels fitxers JSON."); }
        else    { Debug.Log($"S'han carregat un total de {DataManager.Instance.allSettlementList.Count} settlements."); }


    }
    
    
    

    private void LoadCityInventory()
    {
        string path = "Assets/Resources/StartValues/CityInventories";
        string[] files = Directory.GetFiles(path, "*.json");
        
        DataManager.Instance.allCityInvs = new List<CityInventory>();
    
        foreach (string file in files)
        {   
            string jsonContent = File.ReadAllText(file);
            CityInventoryWrapper wrapper = JsonConvert.DeserializeObject<CityInventoryWrapper>(jsonContent);
            
            if (wrapper != null && wrapper.Items != null)
            {
                foreach (CityInventory cityInventory in wrapper.Items)
                {
                    DataManager.Instance.allCityInvs.Add(cityInventory); // Afegeix cada CityInventory a la llista
                    
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
    
    
    // #######################################################################################################
    // POBLACIÓ DEL MÓN - agents, characters, vehicles, items, etc. 
    // #######################################################################################################
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
        DataManager.Instance.allAgentsList = loadedAgents;
        Debug.Log($"Total d'agents carregats: {DataManager.Instance.allAgentsList.Count}");
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
        DataManager.Instance.allAgentInvs = loadedAgentInventories;
        Debug.Log($"Total d'inventaris d'agents carregats: {DataManager.Instance.allAgentInvs.Count}");
    }

    
    // #######################################################################################################
    // EDIFICIS
    // #######################################################################################################
    
    // Funció principal per gestionar la importació d'edificis
    public void ImportBuildings()
    {
        string path = "Assets/Resources/StartValues/Buildings";
        foreach (string file in Directory.GetFiles(path, "*.json"))
        {
            string jsonContent = File.ReadAllText(file);

            // Intentem importar segons el tipus de paquet
            var wrapper = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, object>>>>(jsonContent);

            // Si el paquet és de CityBuildings
            if (wrapper.ContainsKey("CityBuildings"))
            {
                foreach (var buildingData in wrapper["CityBuildings"])
                {
                    string buildingType = buildingData["BuildingType"] as string;

                    // Si és un edifici productiu
                    if (buildingType == "Productive")
                    {
                        CityData city = FindCityByID(buildingData["BuildingLocation"].ToString());
                        if (city != null)
                        {
                            ProductiveBuilding building = CreateProductiveBuilding(buildingData, city);
                            city.Buildings.Add(building);
                            Debug.Log($"Nou edifici productiu a {city.Name}: {building.BuildingName}, ID {building.BuildingID}");
                        }
                    }
                    else if (buildingType == "Civic")
                    {
                        CityData city = FindCityByID(buildingData["BuildingLocation"].ToString());
                        if (city != null)
                        {
                            CivicBuilding building = CreateCivicBuilding(buildingData, city);
                            city.Buildings.Add(building);
                            Debug.Log($"Nou edifici cívic a {city.Name}: {building.BuildingName}, ID {building.BuildingID}");
                        }
                    }
                }
            }
            // Si el paquet és de SettlementBuildings
            else if (wrapper.ContainsKey("SettlementBuildings"))
            {
                foreach (var buildingData in wrapper["SettlementBuildings"])
                {
                    string buildingType = buildingData["BuildingType"] as string;

                    // Si és un edifici productiu
                    if (buildingType == "Productive")
                    {
                        /* Settlement settlement = FindSettlementByID(buildingData["BuildingLocation"].ToString());
                        if (settlement != null)
                        {
                            ProductiveBuilding building = CreateProductiveBuilding(buildingData, settlement);
                            settlement.SettlementBuildings.Add(building);
                            Debug.Log($"Nou edifici productiu a {settlement.settlementName}: {building.BuildingName}");
                        } */
                    }
                    else if (buildingType == "Civic")
                    {
                        // Placeholder per Civic buildings
                        // Implementarem més endavant
                    }
                }
            }
        }
    }

    private ProductiveBuilding CreateProductiveBuilding(Dictionary<string, object> buildingData, CityData city)
    {
        string buildingName = buildingData["BuildingName"].ToString();
        string buildingTemplateID = buildingData["BuildingTemplateID"].ToString(); // Recuperem el template ID

        // Buscar el ProductiveTemplate associat al BuildingTemplateID
        ProductiveTemplate template = DataManager.Instance.productiveTemplates
            .FirstOrDefault(t => t.TemplateID == buildingTemplateID);

        if (template == null)
        {
            Debug.LogError($"No s'ha trobat el ProductiveTemplate amb ID: {buildingTemplateID}");
            return null;
        }

        // Crear una llista de factors des del template
        List<ProductiveFactor> factors = new List<ProductiveFactor>();
        foreach (var factorID in template.Factors)
        {
            TemplateFactor factorTemplate = DataManager.Instance.GetFactorById(factorID);
            if (factorTemplate != null)
            {
                // Afegir el factor adequat a la llista
                factors.Add(new ProductiveFactor(factorTemplate, template.TemplateID));
            }
            else
            {
                Debug.LogWarning($"No s'ha trobat el factor amb ID: {factorID} al template {template.TemplateID}");
            }
        }

        // Crear un nou ProductiveBuilding amb les propietats transferides
        ProductiveBuilding building = new ProductiveBuilding(
            id: DataManager.Instance.GenerateBuildingID(),
            name: buildingName,
            templateID: buildingTemplateID,
            location: city.LocID,
            ownerID: buildingData["BuildingOwnerID"].ToString(),
            inventoryID: city.InventoryID,
            activity: "Idle", // Establir l'estat inicial
            size: int.Parse(buildingData["BuildingSize"].ToString()),
            hpCurrent: 100,
            hpMax: 100,
            productionTempID: template.TemplateID,
            currentFactors: factors, // Factors transferits
            methodsAvailable: new List<string>(template.PossibleMethods), // Copiar els possibles mètodes
            methodActive: null, // No hi ha cap mètode actiu inicialment
            methodDefault: template.DefaultMethod, // Assignar el mètode per defecte
            batchCurrent: null, // Inicialitzar sense batch
            batchBacklog: new List<Batch>(), // Inicialitzar sense backlog
            linearOutput: 1f, // Inicialitzar amb els valors predeterminats
            inputEfficiency: 1f,
            outputEfficiency: 1f,
            cycleEfficiency: 1f,
            salaryEfficiency: 1f,
            jobsPoor: 0, // Inicialitzar llocs de treball
            jobsMid: 0,
            jobsRich: 0
        );
        // Log general
        //Debug.Log($"ProductiveBuilding creat: {building.BuildingName}, Template: {template.TemplateID}");

        return building;
    }

    
    private CivicBuilding CreateCivicBuilding(Dictionary<string, object> buildingData, CityData city)
    {
        string buildingName = buildingData["BuildingName"].ToString();
        string buildingTemplateID = buildingData["BuildingTemplateID"].ToString(); // Recuperem el template ID

        // Buscar el CivicTemplate associat al BuildingTemplateID
        CivicTemplate template = DataManager.Instance.civicTemplates
            .FirstOrDefault(t => t.TemplateID == buildingTemplateID);

        if (template == null)
        {
            Debug.LogError($"No s'ha trobat el CivicTemplate amb ID: {buildingTemplateID}");
            return null;
        }

        // Crear el CivicBuilding i transferir propietats del CivicTemplate
        CivicBuilding civicBuilding = new CivicBuilding(
            id: DataManager.Instance.GenerateBuildingID(),
            name: buildingName,
            templateID: buildingTemplateID,
            location: city.LocID,
            ownerID: buildingData["BuildingOwnerID"].ToString(),
            inventoryID: city.InventoryID,  // Si és necessari associar un inventari
            activity: "Idle",  // Estat inicial
            size: int.Parse(buildingData["BuildingSize"].ToString()),
            hpCurrent: 100,
            hpMax: 100,
            function: template.Function,
            jobsPoor: template.JobsPoor,
            jobsMid: template.JobsMid,
            jobsRich: template.JobsRich,
            servOffered: new List<Service>(template.ServOffered),  
            servNeeded: new List<Service>(template.ServNeeded)     
        );

        Debug.Log($"CivicBuilding creat: {civicBuilding.BuildingName}, Template: {template.TemplateID}");

        return civicBuilding;
    }

    

    private CityData FindCityByID(string id)
    {
        return DataManager.Instance.allCityList.FirstOrDefault(c => c.LocID == id);
    }

    private Settlement FindSettlementByID(string id)
    {
        return DataManager.Instance.allSettlementList.FirstOrDefault(s => s.LocID == id);
    }


    // #######################################################################################################
    // BLOC DE MINERIA
    // #######################################################################################################

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
                            /* Debug.Log($"Mineral ID {deposit.MineralID}, Slot {slotIndex + 1}: ");
                            for (int depth = 0; depth < reservesByDepth.Length; depth++)
                            {
                                Debug.Log($"Depth {depth + 1}: {reservesByDepth[depth]} reserves, {purityLevels[depth]}% purity");
                            }         */      
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

        //Debug.Log($"Iniciant la distribució de reserves: totalReserves = {totalReserves}, slotQuantity = {slotQuantity}");

        for (int i = 0; i < slotQuantity - 1; i++)
        {
            int maxAssignable = Math.Min(maxPerSlot, totalReserves - totalAssigned - (slotQuantity - i - 1) * minPerSlot);
            int minAssignable = Math.Max(minPerSlot, totalReserves - totalAssigned - (slotQuantity - i - 1) * maxPerSlot);
            int reserve = UnityEngine.Random.Range(minAssignable, maxAssignable + 1);

            
            reserves[i] = reserve;
            totalAssigned += reserve;
            //Debug.Log($"Slot {i + 1}: Assignat {reserve} reserves. MinAssignable = {minAssignable}, MaxAssignable = {maxAssignable}");
        }

        // Assigna la resta al darrer slot, garantint que la suma total és igual a TotalReserves
        reserves[slotQuantity - 1] = totalReserves - totalAssigned;
        //Debug.Log($"Slot {slotQuantity}: Assignat {reserves[slotQuantity - 1]} reserves (darrer slot).");

        return reserves;
    }

    private int[] DistributeDepths(float averageDepth, int totalReserves)
    {
        int[] reservesByDepth = new int[4];
        
        float modifiedDepth = averageDepth + UnityEngine.Random.Range(-0.5f, 0.5f);
        modifiedDepth = Mathf.Clamp(modifiedDepth, 1f, 4f); // Assegurar-se que està dins els límits de profunditat
        //Debug.Log($"Original averageDepth: {averageDepth}, Modified averageDepth: {modifiedDepth}");

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
            //Debug.Log($"Depth {i + 1}: Assigned {assigned} reserves with weight {weights[i]}");
        }
        // Asegura que la suma total de reserves és correcta
        reservesByDepth[3] = totalReserves - assignedReserves;
        //Debug.Log($"Depth 4: Assigned {reservesByDepth[3]} reserves (last depth).");

        return reservesByDepth;
    }

    private int[] DistributePurities(float averagePurity, float modifiedDepth)
    {
        int depths = 4;
        float adjustedPurity = Mathf.Clamp(averagePurity + UnityEngine.Random.Range(-25.0f, 25.0f), 10f, 100f);
        int[] purityLevels = new int[4];
        //Debug.Log($"Adjusted Purity based on averagePurity {averagePurity}: {adjustedPurity}");

        // Calcular la puresa a cada profunditat
        for (int i = 0; i < depths; i++)
        {
            // Ponderació basada en la proximitat al modifiedDepth ajustat
            float distance = Mathf.Abs(modifiedDepth - (i + 1));
            purityLevels[i] = Mathf.RoundToInt(adjustedPurity - (distance / 6.0f * adjustedPurity)); // disminuit effecte
            purityLevels[i] = Mathf.Clamp(purityLevels[i], 10, 100); // Repassar el maxim un altre cop
            //Debug.Log($"Depth {i + 1}: {purityLevels[i]}% purity");
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

        //Debug.Log($"Modificadors: AvgDepth: {averageDepth}, Modified: {modifiedDepth}; AvgPurity:{averagePurity}, Modified: {adjustedPurity}");
        
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

// WRAPPERS

[System.Serializable]
public class CityDataList
{
    [JsonProperty("City")]
    public List<CityWrapper> Cities { get; set; }
}

public class CityWrapper
{
    [JsonProperty("LocID")]
    public string LocID { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("NodeID")]
    public string NodeID { get; set; }

    [JsonProperty("InvID")]
    public string InventoryID { get; set; }
    
    [JsonProperty("PoorPop")]
    public int PoorPopulation { get; set; }

    [JsonProperty("MidPop")]
    public int MidPopulation { get; set; }

    [JsonProperty("RichPop")]
    public int RichPopulation { get; set; }

    [JsonProperty("PoorID")]
    public string PoorLifestyleID { get; set; }

    [JsonProperty("MidID")]
    public string MidLifestyleID { get; set; }

    [JsonProperty("RichID")]
    public string RichLifestyleID { get; set; }
    
    [JsonProperty("OwnerID")]
    public string OwnerID { get; set; }
    
    [JsonProperty("Political")]
    public string PoliticalStatus { get; set; }

    [JsonProperty("BuildPoints")]
    public float BuildPoints { get; set; }

    
    
}

[System.Serializable]
public class SettlementDataList
{
    [JsonProperty("Settlement")]
    public List<SettlementWrapper> Settlements { get; set; }
}

public class SettlementWrapper
{
    [JsonProperty("LocID")]
    public string LocID { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("NodeID")]
    public string NodeID { get; set; }

    [JsonProperty("InvID")]
    public string InventoryID { get; set; }

    [JsonProperty("SettlementType")]
    public string SettlActivity { get; set; }

    [JsonProperty("Population")]
    public int Population { get; set; }

    [JsonProperty("Lifestyle")]
    public string SettlLifestyleID { get; set; }

    [JsonProperty("OwnerID")]
    public string OwnerID { get; set; }

    [JsonProperty("PoliticalStatus")]
    public string PoliticalStatus { get; set; }

    [JsonProperty("BuildPoints")]
    public float BuildPoints { get; set; }

    

    
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

