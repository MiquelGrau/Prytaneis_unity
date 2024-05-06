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
        LoadProductionMethods();
        LoadFactorTemplates();
        LoadBuildingTemplates();    
        
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
        ListWrapper<Resource> resourceListWrapper = JsonConvert.DeserializeObject<ListWrapper<Resource>>(jsonData.text);
        if (resourceListWrapper == null || resourceListWrapper.Items == null)
        {
            Debug.LogError("Error en deserialitzar el JSON. Verifica que el format sigui correcte.");
            return;
        }
        DataManager.resourcemasterlist = resourceListWrapper.Items;

        //DataManager.resources = JsonUtility.FromJson<ListWrapper<Resource>>(jsonData.text).Items;
        //var resourceList = JsonUtility.FromJson<ListWrapper<Resource>>(jsonData.text).Items;
        //DataManager.resourcemasterlist = resourceList;

        // Crear HashSets per emmagatzemar tipus i subtipus únics
        HashSet<string> uniqueResourceTypes = new HashSet<string>();
        HashSet<string> uniqueResourceSubtypes = new HashSet<string>();
        
        // Log de linia a linia de recursos
        foreach (var resource in DataManager.resourcemasterlist)
        {
            uniqueResourceTypes.Add(resource.ResourceType);
            uniqueResourceSubtypes.Add(resource.ResourceSubtype);
            /* Debug.Log($"Carregat recurs: {resource.resourceID}, {resource.resourceName}, {resource.resourceType}, "+
                    $"{resource.resourceSubtype}, {resource.basePrice}, {resource.baseWeight}"); */
            Debug.Log($"Carregat recurs: {resource.ResourceID}, {resource.ResourceName}, {resource.ResourceType}, " +
                    $"{resource.ResourceSubtype}, {resource.BasePrice}, {resource.BaseWeight}");
        }
        //Debug.Log("Llistat de recursos carregats");
        //Debug.Log($"Llistat de recursos carregats. Total de recursos: {resourceList.Count}, "+
        //    $"Resource Types: {uniqueResourceTypes.Count}, Resource Subtypes: {uniqueResourceSubtypes.Count}");
        Debug.Log($"Llistat de recursos carregats. Total de recursos: {DataManager.resourcemasterlist.Count}, " +
            $"Resource Types: {uniqueResourceTypes.Count}, Resource Subtypes: {uniqueResourceSubtypes.Count}");
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
                
                
                // Verifica si hi ha factors a assignar
                if (templateData.Factors != null && templateData.Factors.Count > 0)
                {
                    foreach (var factorReference in templateData.Factors)
                    {
                        TemplateFactor foundFactor = FindFactorById(factorReference.Factor);
                        if (foundFactor != null)
                        {
                            productiveTemplate.Factors.Add(foundFactor);
                            //Debug.Log($"-- Factor afegit: ID={foundFactor.FactorID}, Name={foundFactor.FactorName}");
                        }
                        else
                        {
                            Debug.LogWarning($"No s'ha trobat Factor amb ID: {factorReference.Factor}");
                        }
                    }
                }
                
                dataManager.productiveTemplates.Add(productiveTemplate);
                // Logs
                /* Debug.Log($"Afegit ProductiveTemplate: {productiveTemplate.ClassName}, ID: {productiveTemplate.TemplateID}, Factors: {productiveTemplate.Factors.Count}");
                foreach (var factor in productiveTemplate.Factors)
                {
                    Debug.Log($"-- Factor: ID={factor.FactorID}, Name={factor.FactorName}, Type={factor.FactorType}, Effect={factor.FactorEffect}");
                } */
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

                // Logs
                //Debug.Log($"Carregat mètode de producció: {method.MethodName}, ID: {method.MethodID}, Tipus: {method.MethodType}, Temps de cicle: {method.CycleTime}, Inputs: {method.Inputs.Count}, Outputs: {method.Outputs.Count}");

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
        Debug.Log($"Total de Factors de Treballaors carregats: {dataManager.employeeFactors.Count}");
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



