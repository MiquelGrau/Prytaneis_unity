using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class DatabaseImporter : MonoBehaviour
{
    // Fitxers de estàtics
    private const string LifestyleDataPath = "Statics/LifestyleData";
    private const string ResourceDataPath = "Statics/ResourceData";
    private const string NodeDataPath = "Statics/NodeData"; 
    
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
        LoadNodeData();
        LoadLandPaths();
        LoadLifestyleData();
        LoadResourceData();
        LoadCityInventory();
        LoadStartAgents();
        LoadAgentInventories();
        LoadProductionMethods();
        LoadFactorTemplates();
        LoadBuildingTemplates();    // Després de ProductionMethods i de FactorTemplates, o no carregarà

        //cityInventories = new List<CityInventory>(); 
        
        // Obté la referència de DataManager i carrega les ciutats
        /* DataManager dataManager = FindObjectOfType<DataManager>();
        if (dataManager != null)
        {
            cities = dataManager.GetCities();
            // Afegim el Debug.Log aquí per mostrar tot el contingut de la llista cities
            string citiesJson = JsonConvert.SerializeObject(cities, Formatting.Indented);
            Debug.Log("Ciutats carregades: " + citiesJson);
        }
        else
        {
            Debug.LogError("No s'ha trobat DataManager.");
        } */

        Debug.Log("Acabada fase Awake de l'importador! Ciutats, Lifestyle, Resources, Inventaris, etc");
    }

    private void Start()
    {
        ConnectCityAndCityInv();
        ConnectAgentAndAgentInv();
        Debug.Log("Acabada fase Start de l'importador! Connectats inventaris a Ciutats i a Agents");
    }

    private void LoadNodeData()
    {
        TextAsset jsonData = Resources.Load<TextAsset>(NodeDataPath);
        if (jsonData == null)
        {
            Debug.LogError("No es pot trobar el fitxer NodeData.json a la ruta especificada.");
            return;
        }

        // Deserialitza el JSON al format del teu NodeDataWrapper
        NodeDataWrapper nodeData = JsonUtility.FromJson<NodeDataWrapper>(jsonData.text);
        /* foreach (var node in nodeData.nodes_jsonfile)
        {
            // Aquí pots processar cada node com necessitis, per exemple, afegint-los a una llista dins de DataManager
            Debug.Log($"Node carregat: {node.id}, {node.name}");
        } */

        DataManager.worldMapNodes = nodeData.nodes_jsonfile;
        Debug.Log($"Total de nodes carregats: {nodeData.nodes_jsonfile.Count}");
    }

    private void LoadLandPaths()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("Statics/LandPaths");
        if (jsonData == null)
        {
            Debug.LogError("No es pot trobar el fitxer LandPaths.json a la ruta especificada.");
            return;
        }

        // Deserialitza el JSON directament al format del teu LandPathDataWrapper
        LandPathDataWrapper landPathData = JsonUtility.FromJson<LandPathDataWrapper>(jsonData.text);
        /* foreach (var path in landPathData.landpath_jsonfile)
        {
            Debug.Log(path.ToString());
        } */

        DataManager.worldMapLandPaths = landPathData.landpath_jsonfile;
        Debug.Log($"Total de land paths carregats: {landPathData.landpath_jsonfile.Count}");
    }


    private void LoadLifestyleData()
    {
        TextAsset jsonData = Resources.Load<TextAsset>(LifestyleDataPath);
        if (jsonData == null)
        {
            Debug.LogError("No es pot trobar el fitxer LifestyleData.json a la ruta especificada.");
            return;
        }
        LifestyleTier[] tempArray = JsonUtility.FromJson<LifestyleWrapper>(jsonData.text).Items;
        DataManager.lifestyleTiers = new List<LifestyleTier>(tempArray);
        // Logs
            // Linia a linia
        /* foreach (var tier in lifestyleTiers)
        {
            Debug.Log($"Carregat lifestyle Tier {tier.TierID}, {tier.TierName}");
            foreach (var demand in tier.LifestyleDemands)
            {
                Debug.Log($"{demand.resourceType}, {demand.quantityPerThousand} {demand.monthsCritical} {demand.monthsTotal} {demand.resourceVariety}");
            }
        } */
        Debug.Log($"Llistats de LifestyleTier i LifestyleData carregats. "+
            $"Total de LifestyleTiers: {DataManager.lifestyleTiers.Count}");

    }

    private void LoadResourceData()
    {
        TextAsset jsonData = Resources.Load<TextAsset>(ResourceDataPath);
        if (jsonData == null)
        {
            Debug.LogError("No es pot trobar el fitxer ResourceData.json a la ruta especificada.");
            return;
        }
        //DataManager.resources = JsonUtility.FromJson<ListWrapper<Resource>>(jsonData.text).Items;
        var resourceList = JsonUtility.FromJson<ListWrapper<Resource>>(jsonData.text).Items;
        DataManager.resourcemasterlist = resourceList;

        // Crear HashSets per emmagatzemar tipus i subtipus únics
        HashSet<string> uniqueResourceTypes = new HashSet<string>();
        HashSet<string> uniqueResourceSubtypes = new HashSet<string>();
        
        // Log de linia a linia de recursos
        /* foreach (var resource in resources)
        {
            Debug.Log($"Carregat recurs: {resource.resourceID}, {resource.resourceName}, {resource.resourceType}, {resource.resourceSubtype}, {resource.basePrice}, {resource.baseWeight}");
        } */
        foreach (var resource in resourceList)
        {
            uniqueResourceTypes.Add(resource.ResourceType);
            uniqueResourceSubtypes.Add(resource.ResourceSubtype);
        }
        //Debug.Log("Llistat de recursos carregats");
        Debug.Log($"Llistat de recursos carregats. Total de recursos: {resourceList.Count}, "+
            $"Resource Types: {uniqueResourceTypes.Count}, Resource Subtypes: {uniqueResourceSubtypes.Count}");
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

    private void LoadBuildingTemplates()
    {
        string jsonContent = Resources.Load<TextAsset>("Statics/BuildingTemplates").text;
        BuildingTemplateListWrapper wrapper = JsonConvert.DeserializeObject<BuildingTemplateListWrapper>(jsonContent);
        
        
        foreach (var templateData in wrapper.Templates)
        {
            string individualJsonContent = JsonConvert.SerializeObject(templateData);

            if (templateData.TemplateType == "Productive")
            {
                var productiveTemplate = JsonConvert.DeserializeObject<ProductiveTemplate>(individualJsonContent);
                
                /* // Assigna els factors corresponents des de les IDs especificades
                foreach (var factorId in templateData.Factors.Select(f => f.Factor))
                {
                    TemplateFactor foundFactor = FindFactorById(factorId);
                    if (foundFactor != null)
                    {
                        productiveTemplate.Factors.Add(foundFactor);
                        Debug.Log($"-- Factor afegit: ID={foundFactor.FactorID}, Name={foundFactor.FactorName}");
                    }
                    else
                    {
                        Debug.LogWarning($"No s'ha trobat Factor amb ID: {factorId}");
                    }
                } */
                
                // Verifica si hi ha factors a assignar
                if (templateData.Factors != null && templateData.Factors.Count > 0)
                {
                    foreach (var factorReference in templateData.Factors)
                    {
                        TemplateFactor foundFactor = FindFactorById(factorReference.Factor);
                        if (foundFactor != null)
                        {
                            productiveTemplate.Factors.Add(foundFactor);
                            Debug.Log($"-- Factor afegit: ID={foundFactor.FactorID}, Name={foundFactor.FactorName}");
                        }
                        else
                        {
                            Debug.LogWarning($"No s'ha trobat Factor amb ID: {factorReference.Factor}");
                        }
                    }
                }
                
                dataManager.productiveTemplates.Add(productiveTemplate);
                Debug.Log($"Afegit ProductiveTemplate: {productiveTemplate.ClassName}, ID: {productiveTemplate.TemplateID}, Factors: {productiveTemplate.Factors.Count}");
                foreach (var factor in productiveTemplate.Factors)
                {
                    Debug.Log($"-- Factor: ID={factor.FactorID}, Name={factor.FactorName}, Type={factor.FactorType}, Effect={factor.FactorEffect}");
                }
            }
            else if (templateData.TemplateType == "Civic")
            {
                var civicTemplate = JsonConvert.DeserializeObject<CivicTemplate>(individualJsonContent);
                dataManager.civicTemplates.Add(civicTemplate);
            }

            
        }

        Debug.Log($"Total de Plantilles Productives carregades: {dataManager.productiveTemplates.Count}");
        Debug.Log($"Total de Plantilles Cíviques carregades: {dataManager.civicTemplates.Count}");
    }

    private TemplateFactor FindFactorById(string factorId)
    {
        return dataManager.employeeFactors.FirstOrDefault(f => f.FactorID == factorId) ??
            dataManager.resourceFactors.FirstOrDefault(f => f.FactorID == factorId) as TemplateFactor;
    }

    private void LoadProductionMethods()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("Statics/ProductionMethods");
        if (jsonData == null)
        {
            Debug.LogError("No es pot trobar el fitxer ProductionMethods.json a la ruta especificada.");
            return;
        }

        ProductionMethodWrapper wrapper = JsonConvert.DeserializeObject<ProductionMethodWrapper>(jsonData.text);
        if (wrapper.ProductionMethods != null)
        {
            foreach (var method in wrapper.ProductionMethods)
            {
                // Aquí pots processar cada mètode si és necessari, per exemple, convertir ResourceID a objectes Resource.
                dataManager.productionMethods.Add(method);

                // Afegim un log per a cada mètode carregat
                Debug.Log($"Carregat mètode de producció: {method.MethodName}, ID: {method.MethodID}, Tipus: {method.MethodType}, Temps de cicle: {method.CycleTime}, Inputs: {method.Inputs.Count}, Outputs: {method.Outputs.Count}");

                /* foreach(var input in method.Inputs)  // Logs de recursos, quantitats, etc. 
                {
                    Debug.Log($"Input: {input.ResourceID}, Quantitat: {input.Amount}");
                }

                foreach(var output in method.Outputs)
                {
                    Debug.Log($"Output: {output.ResourceID}, Quantitat: {output.Amount}, Probabilitat: {output.Chance}%");
                } */
            }
        }

        Debug.Log($"Mètodes de producció carregats: {dataManager.productionMethods.Count}");
    }

    private void LoadFactorTemplates()
    {
        // Carrega el text JSON com a TextAsset des de la ruta especificada
        string jsonContent = Resources.Load<TextAsset>("Statics/FactorTemplates").text;
        
        // Deserialitza el JSON a l'objecte FactorTemplateListWrapper
        FactorTemplateListWrapper wrapper = JsonConvert.DeserializeObject<FactorTemplateListWrapper>(jsonContent);
        
        // Comprova si l'envoltori i la llista de Factors no són nuls
        if (wrapper != null && wrapper.Factors != null)
        {
            // Recorre cada Factor del JSON
            foreach (var factorJSON in wrapper.Factors)
            {
                // Comprova el tipus de Factor i deserialitza al tipus de classe correcte
                if (factorJSON.FactorType == "Employee")
                {
                    var factor = JsonConvert.DeserializeObject<EmployeeFT>(JsonConvert.SerializeObject(factorJSON));
                    
                    dataManager.employeeFactors.Add(factor);
                }
                else if (factorJSON.FactorType == "Resource")
                {
                    var factor = JsonConvert.DeserializeObject<ResourceFT>(JsonConvert.SerializeObject(factorJSON));
                    dataManager.resourceFactors.Add(factor);
                }
            }
        }

        // Logs per confirmar la càrrega
        Debug.Log($"Total de Factors d'Empleat carregats: {dataManager.employeeFactors.Count}");
        Debug.Log($"Total de Factors de Recurs carregats: {dataManager.resourceFactors.Count}");
    }



    [System.Serializable]
    private class ListWrapper<T>
    {
        public List<T> Items;
    }
}

// Els collons de wrappers, els necessita per desempaquetar els jsons

[System.Serializable]
public class LifestyleWrapper
{
    public LifestyleTier[] Items;
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
public class BuildingTemplateListWrapper
{
    public List<BuildingTemplateJSON> Templates;
}

[System.Serializable]
public class BuildingTemplateJSON
{
    public string TemplateID;
    public string ClassName;
    public string TemplateType;
    public string TemplateSubtype;
    public List<FactorReference> Factors; 
    
}

[System.Serializable]
public class FactorReference
{
    public string Factor; 
}

[System.Serializable]
public class ProductionMethodWrapper
{
    public List<ProductionMethod> ProductionMethods;
}

[System.Serializable]
public class FactorTemplateListWrapper
{
    public List<FactorJSON> Factors;
}
[System.Serializable]
public class FactorJSON
{
    public string FactorID;
    public string FactorName;
    public string FactorType;
    public string FactorEffect;
    public int EffectSize;
    
}



