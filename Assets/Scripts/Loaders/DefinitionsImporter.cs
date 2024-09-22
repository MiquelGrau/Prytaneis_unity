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
        LoadClimateData();
        LoadNodeData();
        LoadLandPaths();
        LoadLifestyleData();
        LoadResourceData();
        LoadProductionMethods();
        LoadFactorTemplates();
        LoadProductiveTemplates();
        LoadCivicTemplates();    
        
        Debug.Log("Acabada fase Awake de l'importador! Ciutats, Lifestyle, Resources, Inventaris, etc");
    }

    
    private void LoadClimateData()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("Statics/Climates");
        if (jsonData == null)
        {
            Debug.LogError("No es pot trobar el fitxer Climates.json a la ruta especificada.");
            return;
        }

        // Deserialitza el JSON al format del teu ClimateDataWrapper
        ClimateDataWrapper climateData = JsonConvert.DeserializeObject<ClimateDataWrapper>(jsonData.text);

        if (climateData?.Climates != null)
        {
            foreach (var newclimate in climateData.Climates)
            {
                DataManager.climateList.Add(newclimate);
                //Debug.Log($"Clima carregat: {newclimate.ClimateID}, {newclimate.ClimateName}. Estacions:");
                //foreach (var season in newclimate.Seasons)
                //{
                //    Debug.Log($"- {season.SeasonName}: Temp Màx: {season.AvgMaxTemp}, Temp Mín: {season.AvgMinTemp}, Precipitació: {season.Precipitation}");
                //}
            }
        }
        else
        {
            Debug.LogError("No s'ha pogut deserialitzar correctament el fitxer Climates.json.");
        }

        Debug.Log($"Total de climes carregats: {climateData?.Climates.Count}");
    }

    
    private void LoadNodeData()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("Statics/NodeData");
        if (jsonData == null)
        {
            Debug.LogError("No es pot trobar el fitxer NodeData.json a la ruta especificada.");
            return;
        }

        // Deserialitza el JSON al format del teu NodeDataWrapper
        NodeDataWrapper nodeData = JsonUtility.FromJson<NodeDataWrapper>(jsonData.text);
        
        // Processar els nodes per assignar les noves propietats
        foreach (var node in nodeData.nodes_jsonfile)
        {
            // Assignar el Climate a partir del nom del Climate en el JSON
            var climateinnode = GetClimateByName(node.Climate);
            if (climateinnode == null)
            {
                Debug.LogError($"No s'ha trobat cap clima amb el nom '{node.Climate}' per al node {node.name}");
                continue;
            }
            var currentSeason = climateinnode.Seasons.FirstOrDefault(); // Assumeix la primera estació com a inicial
            
            // Crear una nova instància de WorldMapNode amb les noves propietats
            var newNode = new WorldMapNode
            {
                id = node.id,
                name = node.name,
                cityId = node.cityId,
                latitude = node.latitude,
                longitude = node.longitude,
                LandNodeType = node.LandNodeType,
                LandContinentId = node.LandContinentId,
                LandRegionId = node.LandRegionId,
                LandSubregionId = node.LandSubregionId,
                WaterNodeType = node.WaterNodeType,
                WaterNodeRegion = node.WaterNodeRegion,
                WaterNodeSubregion = node.WaterNodeSubregion,
                NodeDeposits = node.NodeDeposits,
                NodeClimate = climateinnode,
                CurrentSeason = currentSeason,
                ExtraMinTemp = node.ExtraMinTemp,
                ExtraMaxTemp = node.ExtraMaxTemp
            };

            // Afegir el nou node processat a la llista
            DataManager.worldMapNodes.Add(newNode);
            //Debug.Log($"Node carregat: {newNode.id}, {newNode.name}");
        }

        Debug.Log($"Total de nodes carregats: {nodeData.nodes_jsonfile.Count}");
    }
    public Climate GetClimateByName(string climateName)
    {
        return DataManager.climateList.FirstOrDefault(c => c.ClimateName == climateName);
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
        // Deserialitzar el JSON, en el wrapper
        LifestyleWrapper lifestyleWrapper = JsonConvert.DeserializeObject<LifestyleWrapper>(jsonData.text);
        if (lifestyleWrapper != null && lifestyleWrapper.Items != null)
        
        {
            DataManager.lifestyleTiers = new List<LifestyleTier>(lifestyleWrapper.Items);
            Debug.Log($"Llistats de LifestyleTier i LifestyleData carregats. Total de LifestyleTiers: {DataManager.lifestyleTiers.Count}");
            
            // Debugs
            foreach (var tier in DataManager.lifestyleTiers)
            {
                //Debug.Log($"Loaded LifestyleTier: {tier.TierID}, {tier.TierName}, NextTierID: {tier.NextTierID}, Demands: {tier.LifestyleDemands.Count}");
                Debug.Log($"Loaded LifestyleTier: {tier.TierID}, {tier.TierName}, NextTierID: {tier.NextTierID}, LifestyleDemands: {tier.LifestyleDemands.Count}, ServiceDemands: {tier.ServiceDemands.Count}");
                
                /* foreach (var demand in tier.LifestyleDemands)
                {
                    Debug.Log($"  LifestyleDemand - ResType: {demand.ResType}, DemType: {demand.DemType}, Position: {demand.Position}, MonthlyQty: {demand.MonthlyQty}, MonthsCrit: {demand.MonthsCrit}, MonthsTotal: {demand.MonthsTotal}");
                }
                
                foreach (var service in tier.ServiceDemands)
                {
                    Debug.Log($"  ServiceDemand - ResType: {service.ResType}, DemType: {service.DemType}, Position: {service.Position}, MonthlyQty: {service.MonthlyQty}, Minimum: {service.Minimum}, Optimum: {service.Optimum}");
                } */
                
            }
        }
        else
        {
            Debug.LogError("Error en deserialitzar les dades del JSON.");
        }
        
    }
    [System.Serializable]
    public class LifestyleWrapper
    {
        public LifestyleTier[] Items;
    }



    private void LoadResourceData()
    {
        TextAsset jsonData = Resources.Load<TextAsset>(ResourceDataPath);
        if (jsonData == null)
        {
            Debug.LogError("No es pot trobar el fitxer ResourceData.json a la ruta especificada.");
            return;
        }

        // Deserialitzar el JSON en WrapperResource
        ListWrapper<WrapperResource> resourceListWrapper = JsonConvert.DeserializeObject<ListWrapper<WrapperResource>>(jsonData.text);
        if (resourceListWrapper == null || resourceListWrapper.Items == null)
        {
            Debug.LogError("Error en deserialitzar el JSON. Verifica que el format sigui correcte.");
            return;
        }
        
        // Inicialitzar llistes a DataManager, i HashSets per garantir que no hi hagi duplicats
        DataManager.resourcemasterlist = new List<Resource>();
        DataManager.ResTypesList = new List<ResourceType>();
        DataManager.ResSubtypesList = new List<ResourceSubtype>();
        HashSet<string> uniqueResourceTypes = new HashSet<string>();
        HashSet<string> uniqueResourceSubtypes = new HashSet<string>();
        
        // Log de linia a linia de recursos
        foreach (var wrapperResource  in resourceListWrapper.Items)
        {
            bool isPerishable = wrapperResource.Per.ToLower() == "yes";
            bool isService = wrapperResource.Serv.ToLower() == "yes";
            

            // Afegir o obtenir el ResourceType corresponent
            if (!uniqueResourceTypes.Contains(wrapperResource.Type))
            {
                // Només afegim el ResourceType si no existeix ja
                ResourceType resourceType = new ResourceType(wrapperResource.Type, isService);
                DataManager.ResTypesList.Add(resourceType);
                uniqueResourceTypes.Add(wrapperResource.Type);
            }

            // Afegir o obtenir el ResourceSubtype corresponent
            if (!uniqueResourceSubtypes.Contains(wrapperResource.Subtype))
            {
                // Només afegim el ResourceSubtype si no existeix ja
                ResourceSubtype resourceSubtype = new ResourceSubtype(wrapperResource.Subtype);
                DataManager.ResSubtypesList.Add(resourceSubtype);
                uniqueResourceSubtypes.Add(wrapperResource.Subtype);
            }
            
            
            // Crear una instància de Resource amb les dades convertides
            Resource resource = new Resource(
                wrapperResource.ID,
                wrapperResource.Name,
                wrapperResource.Type,
                wrapperResource.Subtype,
                wrapperResource.Price,
                wrapperResource.Weight,
                isPerishable,
                isService
            );
            // Entrega el que acabem de crear a DataManager
            DataManager.resourcemasterlist.Add(resource);
                        
            /* Debug.Log($"Carregat recurs: {resource.ResourceID}, {resource.ResourceName}, {resource.ResourceType}, " +
                $"{resource.ResourceSubtype}, {resource.BasePrice}, {resource.BaseWeight}, {resource.Perishable}, {resource.Service}"); */
        }
        Debug.Log($"Llistat de recursos carregats. Total de recursos: {DataManager.resourcemasterlist.Count}, " +
            $"Resource Types: {uniqueResourceTypes.Count}, Resource Subtypes: {uniqueResourceSubtypes.Count}");
        
    }
    [System.Serializable]
    public class WrapperResource
    {
        public string ID;      
        public string Name;    
        public string Type;    
        public string Subtype; 
        public int Price;      
        public float Weight;   
        public string Per;     
        public string Serv;    
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
                // Mirem primer Inputs
                var processedInputs = new List<ProductionMethod.MethodInput>();
                foreach (var input in method.Inputs)
                {
                    if (!string.IsNullOrEmpty(input.ResourceID))
                    {
                        processedInputs.Add(new ProductionMethod.MethodInput(input.ResourceID, input.Amount, ProductionMethod.MethodInput.InputType.ResourceID));
                    }
                    else if (!string.IsNullOrEmpty(input.ResourceType))
                    {
                        processedInputs.Add(new ProductionMethod.MethodInput(input.ResourceType, input.Amount, ProductionMethod.MethodInput.InputType.ResourceType));
                    }
                    else if (!string.IsNullOrEmpty(input.ResourceSubtype))
                    {
                        processedInputs.Add(new ProductionMethod.MethodInput(input.ResourceSubtype, input.Amount, ProductionMethod.MethodInput.InputType.ResourceSubtype));
                    }
                }

                // Quadrem outputs
                var processedOutputs = new List<ProductionMethod.MethodOutput>();
                foreach (var output in method.Outputs)
                {
                    processedOutputs.Add(new ProductionMethod.MethodOutput
                    {
                        ResourceID = output.ResourceID,
                        Amount = output.Amount,
                        Chance = output.Chance,
                        Type = output.Type
                    });
                }
                
                // I els enxufem a dins
                var newMethod = new ProductionMethod(method.MethodID, method.MethodName, method.MethodType, method.CycleTime, processedInputs, processedOutputs);
                dataManager.productionMethods.Add(newMethod);
                
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
        Debug.Log($"Total de Factors de Treballadors carregats: {dataManager.employeeFactors.Count}");
        Debug.Log($"Total de Factors de Recurs carregats: {dataManager.resourceFactors.Count}");
    }

    /* private void LoadBuildingTemplates()
    {
        string jsonContent = Resources.Load<TextAsset>("Statics/BuildingTemplates").text;
        BuildingTemplateListWrapper templateWrapper = JsonConvert.DeserializeObject<BuildingTemplateListWrapper>(jsonContent);
        Debug.Log($"Total de Templates llegits: {templateWrapper.Templates.Count}");

        foreach (var templateData in templateWrapper.Templates)
        {
            //Debug.Log($"Processant Template - ID: {templateData.TemplateID}, ClassName: {templateData.ClassName}, TemplateType: {templateData.TemplateType}");
            string individualJsonContent = JsonConvert.SerializeObject(templateData);
            
            if (templateData.TemplateType == "Productive")
            {
                // Crear el ProductiveTemplate a partir de BuildingTemplateJSON
                var productiveTemplate = new ProductiveTemplate(
                    templateData.TemplateID,
                    templateData.ClassName,
                    templateData.TemplateType,
                    templateData.TemplateSubtype,
                    null, // DefaultMethod es definirà després
                    new List<ProductionMethod>(),
                    new List<TemplateFactor>(),
                    templateData.JobsPoor,
                    templateData.JobsMid,
                    templateData.JobsRich,
                    templateData.Capacity
                );

                // Assigna el ProductionMethod per defecte
                ProductionMethod defaultMethod = FindMethodById(templateData.DefaultMethod);
                if (defaultMethod != null)
                {
                    productiveTemplate.GetType().GetProperty("DefaultMethod").SetValue(productiveTemplate, defaultMethod);
                    //Debug.Log($"Assignat DefaultMethod: {defaultMethod.MethodID}");
                }
                else
                {
                    Debug.LogWarning($"No s'ha trobat ProductionMethod amb ID: {templateData.DefaultMethod}");
                }

                // Assigna els ProductionMethods disponibles
                if (templateData.PossibleMethods != null && templateData.PossibleMethods.Count > 0)
                {
                    foreach (var methodReference in templateData.PossibleMethods)
                    {
                        ProductionMethod foundMethod = FindMethodById(methodReference.Method);
                        if (foundMethod != null)
                        {
                            productiveTemplate.PossibleMethods.Add(foundMethod);
                            //Debug.Log($"-- ProductionMethod afegit: ID={foundMethod.MethodID}, Name={foundMethod.MethodName}");
                        }
                        else
                        {
                            Debug.LogWarning($"No s'ha trobat ProductionMethod amb ID: {methodReference.Method}");
                        }
                    }
                }
                
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
                //Debug.Log($"ProductiveTemplate afegit: {productiveTemplate.ClassName}, ID: {productiveTemplate.TemplateID}");
            }
            else if (templateData.TemplateType == "Civic")
            {
                var civicTemplate = JsonConvert.DeserializeObject<CivicTemplate>(individualJsonContent);
                dataManager.civicTemplates.Add(civicTemplate);
            }

            
        }

        Debug.Log($"Total de Plantilles Productives carregades: {dataManager.productiveTemplates.Count}");
        Debug.Log($"Total de Plantilles Cíviques carregades: {dataManager.civicTemplates.Count}");
    } */

    private void LoadCivicTemplates()
    {
        string jsonContent = Resources.Load<TextAsset>("Statics/CivicBldgTemplates").text;
        CivicTemplateListWrapper templateWrapper = JsonConvert.DeserializeObject<CivicTemplateListWrapper>(jsonContent);
        Debug.Log($"Total de Civic Templates llegits: {templateWrapper.Templates.Count}");

        foreach (var templateData in templateWrapper.Templates)
        {
            //Debug.Log($"Processant CivicTemplate: TemplateID = {templateData.TemplateID}, BuildingName = {templateData.Name}");

            List<Service> servOffered = new List<Service>();
            List<Service> servNeeded = new List<Service>();

            // Afegir els Effects com a ServOffered
            foreach (var effect in templateData.Effects)
            {
                var matchedResource = DataManager.resourcemasterlist.FirstOrDefault(r => r.ResourceName == effect.Effect);
                if (matchedResource != null)
                {
                    servOffered.Add(new Service(effect.Effect, effect.Amount)
                    {
                        ResourceType = matchedResource.ResourceType // Assegura't que assignem ResourceType correctament
                    });
                    //Debug.Log($"[ServOffered] Type: {matchedResource.ResourceType}, Qty: {effect.Amount}");
                }
                else
                {
                    Debug.LogWarning($"ResourceID {effect.Effect} no trobat a la llista de recursos!");
                }
            }

            // Afegir els Needs com a ServNeeded
            foreach (var need in templateData.Needs)
            {
                var matchedResource = DataManager.resourcemasterlist.FirstOrDefault(r => r.ResourceName == need.Need);
                if (matchedResource != null)
                {
                    servNeeded.Add(new Service(need.Need, need.Amount)
                    {
                        ResourceType = matchedResource.ResourceType // Assegura't que assignem ResourceType correctament
                    });
                    //Debug.Log($"[ServNeeded] Type: {matchedResource.ResourceType}, Qty: {need.Amount}");
                }
                else
                {
                    Debug.LogWarning($"ResourceID {need.Need} no trobat a la llista de recursos!");
                }
            }
            
            var civicTemplate = new CivicTemplate(
                templateData.TemplateID,
                templateData.Name,
                "Civic", // TemplateType fixed to Civic
                "", // TemplateSubtype can be added if needed
                templateData.Function,
                templateData.JobsPoor,
                templateData.JobsMid,
                templateData.JobsRich,
                templateData.Repeat,
                templateData.Labour,
                templateData.HardMat,
                templateData.LightMat,
                templateData.SpecialMat,
                templateData.BuildPoints,
                templateData.Water,
                templateData.WaterYes,
                servOffered,
                servNeeded
            );
            dataManager.civicTemplates.Add(civicTemplate);
        }

        Debug.Log($"Total de Plantilles Cíviques carregades: {dataManager.civicTemplates.Count}");
    }
    public class CivicTemplateListWrapper
    {
        public List<CivicTemplateJSON> Templates { get; set; }
    }
    public class CivicTemplateJSON
    {
        public string TemplateID { get; set; }
        public string Name { get; set; }
        public string Function { get; set; }
        public float Labour { get; set; }
        public float HardMat { get; set; }
        public float LightMat { get; set; }
        public float SpecialMat { get; set; }
        public int JobsPoor { get; set; }
        public int JobsMid { get; set; }
        public int JobsRich { get; set; }
        public float Repeat { get; set; }
        public float BuildPoints { get; set; }
        public string Water { get; set; }
        public bool WaterYes { get; set; }

        public List<EffectData> Effects { get; set; }
        public List<NeedData> Needs { get; set; }
    }

    public class EffectData
    {
        public string Effect { get; set; }
        public int Amount { get; set; }
    }

    public class NeedData
    {
        public string Need { get; set; }
        public int Amount { get; set; }
    }

    private void LoadProductiveTemplates()
    {
        string jsonContent = Resources.Load<TextAsset>("Statics/ProdBldgTemplates").text;
        ProductiveTemplateListWrapper templateWrapper = JsonConvert.DeserializeObject<ProductiveTemplateListWrapper>(jsonContent);
        Debug.Log($"Total de Productive Templates llegits: {templateWrapper.Templates.Count}");

        foreach (var templateData in templateWrapper.Templates)
        {
            var productiveTemplate = new ProductiveTemplate(
                templateData.TemplateID,
                templateData.ClassName,
                templateData.TemplateType,
                templateData.TemplateSubtype,
                templateData.DefaultMethod, 
                new List<string>(),
                new List<string>(),
                templateData.JobsPoor,
                templateData.JobsMid,
                templateData.JobsRich,
                templateData.Capacity
            );

            // Assignar els PossibleMethods com a IDs
            if (templateData.PossibleMethods != null && templateData.PossibleMethods.Count > 0)
            {
                foreach (var methodID in templateData.PossibleMethods)
                {
                    productiveTemplate.PossibleMethods.Add(methodID); 
                }
            }
            
            /* // Assigna els ProductionMethods disponibles
            if (templateData.PossibleMethods != null && templateData.PossibleMethods.Count > 0)
            {
                foreach (var methodReference in templateData.PossibleMethods)
                {
                    ProductionMethod foundMethod = FindMethodById(methodReference.MethodID);
                    if (foundMethod != null)
                    {
                        productiveTemplate.PossibleMethods.Add(foundMethod);
                    }
                    else
                    {
                        Debug.LogWarning($"No s'ha trobat ProductionMethod amb ID: {methodReference.MethodID}");
                    }
                }
            } */

            // Verifica si hi ha factors a assignar
            /* if (templateData.Factors != null && templateData.Factors.Count > 0)
            {
                foreach (var factorReference in templateData.Factors)
                {
                    TemplateFactor foundFactor = FindFactorById(factorReference.FactorID);
                    if (foundFactor != null)
                    {
                        productiveTemplate.Factors.Add(foundFactor);
                    }
                    else
                    {
                        Debug.LogWarning($"No s'ha trobat Factor amb ID: {factorReference.FactorID}");
                    }
                }
            } */

            // Assignar els Factors com a IDs
            if (templateData.Factors != null && templateData.Factors.Count > 0)
            {
                foreach (var factorID in templateData.Factors)
                {
                    productiveTemplate.Factors.Add(factorID); 
                }
            }

            dataManager.productiveTemplates.Add(productiveTemplate);
        }

        Debug.Log($"Total de Plantilles Productives carregades: {dataManager.productiveTemplates.Count}");
    }

    public class ProductiveTemplateListWrapper
    {
        public List<ProductiveTemplate> Templates { get; set; }
    }
    private TemplateFactor FindFactorById(string factorId)
    {
        return dataManager.employeeFactors.FirstOrDefault(f => f.FactorID == factorId) ??
            dataManager.resourceFactors.FirstOrDefault(f => f.FactorID == factorId) as TemplateFactor;
    }
    private ProductionMethod FindMethodById(string methodId)
    {
        return dataManager.productionMethods.FirstOrDefault(m => m.MethodID == methodId);
    }
    

    [System.Serializable]
    private class ListWrapper<T>
    {
        public List<T> Items;
    }
}

// Els collons de wrappers, els necessita per desempaquetar els jsons

[System.Serializable]
public class NodeDataWrapper
{
    public List<JsonNodeData> nodes_jsonfile;
}

[System.Serializable]
public class JsonNodeData
{
    public string id;
    public string name;
    public string cityId;
    public float latitude;
    public float longitude;
    public string LandNodeType;
    public string LandContinentId;
    public string LandRegionId;
    public string LandSubregionId;
    public string WaterNodeType;
    public string WaterNodeRegion;
    public string WaterNodeSubregion;
    public List<MineralResource> NodeDeposits { get; set; } = new List<MineralResource>();
    public string Climate;
    public float ExtraMinTemp;
    public float ExtraMaxTemp;
}

[System.Serializable]
public class ClimateDataWrapper
{
    public List<Climate> Climates;
}


/* [System.Serializable]
public class BuildingTemplateListWrapper
{
    public List<BuildingTemplateJSON> Templates;
} */

[System.Serializable]
public class BuildingTemplateJSON
{
    public string TemplateID;
    public string ClassName;
    public string TemplateType;
    public string TemplateSubtype;
    public int JobsPoor { get; set; }
    public int JobsMid { get; set; }
    public int JobsRich { get; set; }
    public string DefaultMethod { get; set; }
    public int Capacity { get; set; }
    public List<MethodReference> PossibleMethods { get; set; } = new List<MethodReference>(); 
    public List<FactorReference> Factors { get; set; } = new List<FactorReference>(); 
    
}

[System.Serializable]
public class MethodReference
{
    public string Method { get; set; }
}
[System.Serializable]
public class FactorReference
{
    public string Factor; 
}

[System.Serializable]
public class ProductionMethodWrapper
{
    public List<ProductionMethod> ProductionMethods { get; set; }
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



