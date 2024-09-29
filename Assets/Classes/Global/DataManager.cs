using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
        // Aquesta és la classe per guardar DADES del joc. Tot el que es vagi fent, comerç, moviment, etc, es volcarà aquí. 


public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    private string dataPath;

    // Classes estatiques, definicions
    public static List<Resource> resourcemasterlist;
    public static List<ResourceType> ResTypesList;
    public static List<ResourceSubtype> ResSubtypesList;
    
    public static List<LifestyleTier> lifestyleTiers;
    public List<ProductiveTemplate> productiveTemplates = new List<ProductiveTemplate>();
    public List<CivicTemplate> civicTemplates = new List<CivicTemplate>();
    public List<ProductionMethod> productionMethods = new List<ProductionMethod>();
    public List<EmployeeFT> employeeFactors = new List<EmployeeFT>();
    public List<ResourceFT> resourceFactors = new List<ResourceFT>();
    


    // Geografia fisica
    public static List<WorldMapNode> worldMapNodes = new List<WorldMapNode>();
    public static List<WorldMapLandPath> worldMapLandPaths = new List<WorldMapLandPath>();
    public static List<Climate> climateList = new List<Climate>();

    // Geografia humana
    public CityDataList dataItems;  // antic, candidat de borrar
    public List<CityData> allCityList = new List<CityData>(); 
    public List<Settlement> allSettlementList;
    public List<CityInventory> cityInventories;
    
    // BBDD d'edificis
    public int buildingCounter = 0;
        // Edificis guardats a dins de les ciutats i settlements directament
    
    
    // Classes de agents, merchants, characters, etc
    public List<Agent> agents = new List<Agent>();
    public List<AgentInventory> agentInventories;

    

    private void Awake()
    {
        // Bloc basic per a que no es perdi la variable en altres llocs. Crec, això diu el gepeto. 
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Això manté l'objecte viu entre canvis d'escena
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        // Depuració per a confirmar la càrrega de dades
        //Debug.Log($"Nombre de templates productius carregats: {productiveTemplates.Count}");
        //Debug.Log($"Nombre de templates cívics carregats: {civicTemplates.Count}");
        
    }

    public void SaveData()
    {
        string json = JsonConvert.SerializeObject(dataItems, Formatting.Indented); // Use Formatting.Indented for pretty print
        File.WriteAllText(dataPath, json);
    }
    
    
    
    //////////////
    // BUSCADORS
    //////////////

    public List<CityData> GetCities()
    {
        return allCityList;
    }

    public CityData GetCityDataByID(string cityID)  // Te la demanaran mil vegades, millor tenir això aqui dins
    {
        //return dataItems.cities.FirstOrDefault(city => city.cityID == cityID);
        return allCityList.FirstOrDefault(city => city.LocID == cityID);
    }

    public CityInventory GetCityInvByID(string invID)
    {
        // Buscar l'inventari associat a una ciutat
        return cityInventories.FirstOrDefault(inv => inv.CityInvID == invID);
    }

    public List<Agent> GetAgents()
    {
        return agents;
    }

    public Resource GetResourceByID(string resourceID)
    {
        return resourcemasterlist.FirstOrDefault(r => r.ResourceID == resourceID);
    }
    
    public Agent GetAgentByID(string agentID)
    {
        return agents.FirstOrDefault(agent => agent.agentID == agentID);
    }

    public string GenerateBuildingID()
    {
        buildingCounter++;
        return $"B{buildingCounter.ToString().PadLeft(5, '0')}";
    }

    public WorldMapNode FindNodeById(string nodeId)
    {
        return worldMapNodes.FirstOrDefault(node => node.id == nodeId);
    }
    public string NodeNameByID(string nodeId)
    {
        var node = worldMapNodes.FirstOrDefault(n => n.id == nodeId);
        return node != null ? node.name : "Unknown Node";
    }

    public ProductionMethod GetProductionMethodByID(string methodID)
    {
        return productionMethods.FirstOrDefault(method => method.MethodID == methodID);
    }

    // Funció per obtenir el nom de la plantilla basant-se en l'ID
    public string GetTemplateNameByID(string templateID)
    {
        var template = productiveTemplates.FirstOrDefault(pt => pt.TemplateID == templateID) 
                    ?? civicTemplates.FirstOrDefault(ct => ct.TemplateID == templateID) as BuildingTemplate;
        
        return template?.ClassName ?? "Unknown Template";
    }
    public CivicTemplate GetCivicTemplateByID(string templateID)
    {
        return civicTemplates.FirstOrDefault(template => template.TemplateID == templateID);
    }


    public TemplateFactor GetFactorById(string factorID)
    {
        // Buscar primer a la llista de EmployeeFT
        var employeeFactor = employeeFactors.FirstOrDefault(factor => factor.FactorID == factorID);
        if (employeeFactor != null)
        {
            return employeeFactor;
        }

        // Si no es troba a EmployeeFT, buscar a la llista de ResourceFT
        var resourceFactor = resourceFactors.FirstOrDefault(factor => factor.FactorID == factorID);
        if (resourceFactor != null)
        {
            return resourceFactor;
        }

        // Si no es troba a cap llista, retornar null
        Debug.LogWarning($"No s'ha trobat cap factor amb ID: {factorID}");
        return null;
    }


    
    /* public List<Agent> GetAgents()
    {
        if (agents is AgentList agentList)
        {
            return agentList.agents;
        }
        else
        {
            Debug.LogError("DataManager no conté una instància de Agents.");
            return new List<Agent>();
        }
    } */


}
