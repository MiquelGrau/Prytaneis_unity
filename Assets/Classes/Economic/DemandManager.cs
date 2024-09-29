using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Text;
using UnityEngine;

public class DemandManager : MonoBehaviour
{
    
    //public List<LifestyleTier> demandTiers; 
    
    public TextMeshProUGUI inventoryDisplayText; 

    // Comptador inicial per aplicar calculs
    private float timeToWait = 2f; 
    private bool firstDemand = false;
    
    private void Start()
    {  
        //Debug.Log("Iniciant DemandManager...");
        
        // Calcula les demandes basades en la ciutat actual
        foreach (CityData city in DataManager.Instance.allCityList)
        {
            GetTierNeedsForCity(city);
            GenerateMarketDemands(city);
            CalculateAllCityPrices(city);
            GetServiceNeedsForCity(city);
        }
        
       
        Debug.Log("[DemandManager] Demandes a totes les ciutats calculades");
        
    }

    private void Update()
    {
        
        if (GlobalTime.Instance.currentDayTime >= timeToWait && !firstDemand) // Aplico directament el "daytime", que és com la hora
        {
            Debug.Log("Aplicats calculs de demanda a segon 2");

            //AssignDemandsToVarieties();
            GenerateMarketDemands(GameManager.Instance.CurrentCity);  
            CoverCityDemands(GameManager.Instance.CurrentCity);
            CalculateAllCityPrices(GameManager.Instance.CurrentCity); 
            inventoryDisplayText.text = GetCityInventoryDisplayText();
            firstDemand = true;
        }
    }

    private void GetTierNeedsForCity(CityData chosenCity) 
    {
        // Assignem
        if (chosenCity == null)
        {
            Debug.LogError("La ciutat passada a GetTierNeedsForCity és null.");
            return;
        }
        //Debug.Log($"Processant la ciutat: {chosenCity.cityName} (ID: {chosenCity.cityID})");

        CityInventory chosenCityInventory = chosenCity.CityInventory;
        if (chosenCityInventory == null)
        {
            Debug.LogError("No s'ha assignat cap inventari de ciutat.");
            return;
        }
        
        // Netejar les PopDemands existents
        chosenCityInventory.PopDemands.Clear();
        //Debug.Log($"Inventari de ciutat netejat per {chosenCity.cityName}.");


        // Obtenir els LifestyleTiers per a cada grup de població
        var totalPopDemands = new List<LifestyleTier>
        {
            DataManager.lifestyleTiers.Find(tier => tier.TierID == chosenCity.PoorLifestyleID),
            DataManager.lifestyleTiers.Find(tier => tier.TierID == chosenCity.MidLifestyleID),
            DataManager.lifestyleTiers.Find(tier => tier.TierID == chosenCity.RichLifestyleID)
        };
        
        if (totalPopDemands[0] == null)
        {
            Debug.LogError($"No s'ha trobat LifestyleTier per PoorLifestyleID: {chosenCity.PoorLifestyleID}");
        }
        if (totalPopDemands[1] == null)
        {
            Debug.LogError($"No s'ha trobat LifestyleTier per MidLifestyleID: {chosenCity.MidLifestyleID}");
        }
        if (totalPopDemands[2] == null)
        {
            Debug.LogError($"No s'ha trobat LifestyleTier per RichLifestyleID: {chosenCity.RichLifestyleID}");
        }
        
        int[] populations = { chosenCity.PoorPopulation, chosenCity.MidPopulation, chosenCity.RichPopulation };
        string[] populationClasses = { "Poor", "Mid", "Rich" };

        // Ara calcula les demandes per a cada grup de població
        for (int i = 0; i < totalPopDemands.Count; i++)
        {
            var tier = totalPopDemands[i];
            if (tier == null)
            {
                Debug.LogError($"El LifestyleTier per {populationClasses[i]} és null. Saltant aquest grup de població.");
                continue;
            }
            
            foreach (var demand in tier.LifestyleDemands)
            {
                var newDemand = new PopulationDemand(
                    populationClasses[i], 
                    demand.ResType, 
                    demand.DemType, 
                    demand.Position);

                // Calcular les demandes
                newDemand.ConsumeQty = populations[i] * demand.MonthlyQty / 1000;
                newDemand.CritQty = newDemand.ConsumeQty * demand.MonthsCrit;
                newDemand.TotalQty = newDemand.ConsumeQty * demand.MonthsTotal;
                
                // Afegir a la llista de PopDemands
                chosenCityInventory.PopDemands.Add(newDemand);
            }
        }
        //Debug.Log($"Processament de demandes complet per la ciutat: {chosenCity.cityName} (ID: {chosenCity.cityID})");
    }


    private void GenerateMarketDemands(CityData city)
    {
        CityInventory cityInventory = city.CityInventory;

        if (cityInventory == null)
        {
            Debug.LogError("No s'ha assignat cap inventari de ciutat.");
            return;
        }

        // Netejar valors antics, de Market Demands i Population Demand (els covered)
        foreach (var resource in cityInventory.InventoryResources)
        {
            resource.DemandConsume = 0;
            resource.DemandCritical = 0;
            resource.DemandTotal = 0;
            resource.PositionPoor = 0;
            resource.PositionMid = 0;
            resource.PositionRich = 0;
        }

        foreach (var populationDemand in cityInventory.PopDemands)
        {
            populationDemand.AssignedResID = null;
            populationDemand.CoveredQty = 0;
            populationDemand.Fulfilled = false;
        }
        
        // Ordenar InventoryResources per Quantity descendent, per assignar els més grans
        cityInventory.InventoryResources = cityInventory.InventoryResources
            .OrderByDescending(ir => ir.Quantity)
            .ToList();

        foreach (var populationDemand in cityInventory.PopDemands)
        {
            var existingResource = cityInventory.InventoryResources
                .FirstOrDefault(ir => ir.ResourceType == populationDemand.ResourceType &&
                                    ir.ResourceID != null &&
                                    (populationDemand.Class == "Poor" && ir.PositionPoor == 0 ||
                                    populationDemand.Class == "Mid" && ir.PositionMid == 0 ||
                                    populationDemand.Class == "Rich" && ir.PositionRich == 0));
                    // Sembla enrabessat, pero permet buscar a la popdemand correcta
            
            var resourceWithNoID = cityInventory.InventoryResources
                .FirstOrDefault(ir => ir.ResourceType == populationDemand.ResourceType &&
                                    ir.ResourceID == null);
                    // Per si en un cas anterior del foreach ha generat ja alguna restype generica
                                    
            if (existingResource != null)
            {
                switch (populationDemand.Class) {
                    case "Poor": existingResource.PositionPoor = populationDemand.Position; break;
                    case "Mid":  existingResource.PositionMid = populationDemand.Position;  break;
                    case "Rich": existingResource.PositionRich = populationDemand.Position; break;
                }
                existingResource.DemandConsume += populationDemand.ConsumeQty;
                existingResource.DemandCritical += populationDemand.CritQty;
                existingResource.DemandTotal += populationDemand.TotalQty;
            }      // Com que abans ja hem posat el break, si no troba la posició bona marxarà a crear-ne un de nou
            else if (resourceWithNoID != null)
            {
                switch (populationDemand.Class) {
                    case "Poor": resourceWithNoID.PositionPoor = populationDemand.Position; break;
                    case "Mid":  resourceWithNoID.PositionMid = populationDemand.Position;  break;
                    case "Rich": resourceWithNoID.PositionRich = populationDemand.Position; break;
                }
                resourceWithNoID.DemandConsume += populationDemand.ConsumeQty;
                resourceWithNoID.DemandCritical += populationDemand.CritQty;
                resourceWithNoID.DemandTotal += populationDemand.TotalQty;
            }
            else
            {
                var newResource = new CityInventoryResource(
                    populationDemand.ResourceType)
                {
                    PositionPoor = populationDemand.Class == "Poor" ? populationDemand.Position : 0,
                    PositionMid = populationDemand.Class == "Mid" ? populationDemand.Position : 0,  
                    PositionRich = populationDemand.Class == "Rich" ? populationDemand.Position : 0, 
                    DemandConsume = populationDemand.ConsumeQty,
                    DemandCritical = populationDemand.CritQty,
                    DemandTotal = populationDemand.TotalQty
                };

                cityInventory.InventoryResources.Add(newResource);
            }
        }

        
        // Processar les BuildingDemands
        foreach (var buildingDemand in cityInventory.BuildingDemands)
        {
            var existingResource = cityInventory.InventoryResources
                .FirstOrDefault(ir => ir.ResourceType == buildingDemand.ResType && ir.ResourceID != null);
            var resourceWithNoID = cityInventory.InventoryResources
                .FirstOrDefault(ir => ir.ResourceType == buildingDemand.ResType && ir.ResourceID == null);

            if (existingResource != null)
            {
                existingResource.DemandConsume += buildingDemand.ConsumeQty;
                existingResource.DemandCritical += buildingDemand.CritQty;
                existingResource.DemandTotal += buildingDemand.TotalQty;
            }
            else if (resourceWithNoID != null)
            {
                resourceWithNoID.DemandConsume += buildingDemand.ConsumeQty;
                resourceWithNoID.DemandCritical += buildingDemand.CritQty;
                resourceWithNoID.DemandTotal += buildingDemand.TotalQty;
            }
            else
            {
                var newResource = new CityInventoryResource(
                    buildingDemand.ResType)
                {
                    DemandConsume = buildingDemand.ConsumeQty,
                    DemandCritical = buildingDemand.CritQty,
                    DemandTotal = buildingDemand.TotalQty
                };

                cityInventory.InventoryResources.Add(newResource);
            }
        }
        
        //Debug.Log("MarketDemands generades correctament.");
    }

    private void CoverCityDemands(CityData city)
    {
        CityInventory cityInventory = city.CityInventory;

        if (cityInventory == null)
        {
            Debug.LogError("No s'ha assignat cap inventari de ciutat.");
            return;
        }

        // Copiar la llista de InventoryResources amb Quantity positiu i ordenar per Quantity descendent
        List<CityInventoryResource> resourcesToCover = cityInventory.InventoryResources
            .Where(ir => ir.Quantity > 0)
            .Select(ir => new CityInventoryResource(ir.ResourceID, ir.Quantity, ir.CurrentValue)
            {
                ResourceType = ir.ResourceType,
                DemandConsume = ir.DemandConsume,
                DemandCritical = ir.DemandCritical,
                DemandTotal = ir.DemandTotal,
                CurrentPrice = ir.CurrentPrice,
                PositionPoor = ir.PositionPoor,
                PositionMid = ir.PositionMid,
                PositionRich = ir.PositionRich
            })
            .OrderByDescending(ir => ir.Quantity)
            .ToList();

        // Treballar les BuildingDemands
        foreach (var buildingDemand in cityInventory.BuildingDemands)
        {
            var resourceToCover = resourcesToCover.FirstOrDefault(r => r.ResourceType == buildingDemand.ResType && string.IsNullOrEmpty(buildingDemand.AssignedResID));

            if (resourceToCover != null)
            {
                buildingDemand.AssignedResID = resourceToCover.ResourceID;
                float coveredQty = Mathf.Min(resourceToCover.Quantity, buildingDemand.CritQty);
                buildingDemand.CoveredQty += coveredQty;
                resourceToCover.Quantity -= coveredQty;
            }
            else if (!string.IsNullOrEmpty(buildingDemand.AssignedResID))
            {
                resourceToCover = resourcesToCover.FirstOrDefault(r => r.ResourceID == buildingDemand.AssignedResID);
                if (resourceToCover != null)
                {
                    float coveredQty = Mathf.Min(resourceToCover.Quantity, buildingDemand.CritQty);
                    buildingDemand.CoveredQty += coveredQty;
                    resourceToCover.Quantity -= coveredQty;
                }
            }
        }

        // Funció auxiliar per treballar PopulationDemands per classe
        void CoverPopulationDemands(string popClass)
        {
            foreach (var resource in resourcesToCover.Where(r => r.Quantity > 0).ToList())
            {
                var populationDemand = cityInventory.PopDemands
                    .FirstOrDefault(pd => pd.ResourceType == resource.ResourceType && pd.Class == popClass && pd.Position != 0);

                if (populationDemand != null)
                {
                    populationDemand.AssignedResID = resource.ResourceID;
                    float coveredQty = Mathf.Min(resource.Quantity, populationDemand.ConsumeQty);
                    populationDemand.CoveredQty += coveredQty;
                    resource.Quantity -= coveredQty;
                    populationDemand.Fulfilled = populationDemand.CoveredQty >= populationDemand.ConsumeQty;
                }
            }
        }

        // Treballar les PopulationDemands per classe
        CoverPopulationDemands("Rich");
        CoverPopulationDemands("Mid");
        CoverPopulationDemands("Poor");

        //Debug.Log("Covered city demands correctly.");
    }

    public void CalculateAllCityPrices(CityData city)
    {
        CityInventory cityInventory = city.CityInventory;

        foreach (var resline in cityInventory.InventoryResources)
        {
            if (resline.ResourceID != null)
            {
                FindPriceForCityResource(resline);  // Separat aixi es pot cridar més vegades
            }
        }
    }

    private void FindPriceForCityResource(CityInventoryResource resline) // Aqui centralitzo la funció de preu per ciutats
    {
        // Calcula el quantitat de diferencia respecte la demanda
        float difQty = resline.DemandTotal == 0 ? 3f : 
            ((float)resline.Quantity - (float)resline.DemandTotal) / (float)resline.DemandTotal;

        // Calcula la price elasticity segons la fórmula donada: y= -0.6x^3 +0.8x^2 -0.6x +1
        float priceElasticity = -0.6f * Mathf.Pow(difQty, 3) + 0.8f * Mathf.Pow(difQty, 2) - 0.6f * difQty + 1f;

        // Assegura't que la price elasticity no sigui negativa, posa valor minim, 25%
        if (priceElasticity < 0f) { priceElasticity = 0.25f; }

        // Troba el base price del recurs
        var matchedResource = DataManager.resourcemasterlist.FirstOrDefault(r => r.ResourceID == resline.ResourceID);
        float basePrice = matchedResource != null ? matchedResource.BasePrice : 0f;

        // Calcula el CurrentPrice
        resline.CurrentPrice = Mathf.RoundToInt(priceElasticity * basePrice);
    }
    
    private void GetServiceNeedsForCity(CityData chosenCity)
    {
        
        //Debug.Log($"Processant serveis per a la ciutat: {chosenCity.cityName} (ID: {chosenCity.cityID})");

        CityInventory chosenCityInventory = chosenCity.CityInventory;
        if (chosenCityInventory == null)
        {
            Debug.LogError("No s'ha assignat cap inventari de ciutat.");
            return;
        }
        
        // Netejar els serveis existents
        chosenCityInventory.Services.Clear();
        //Debug.Log($"Inventari de serveis netejat per {chosenCity.cityName}.");

        // Obtenir els LifestyleTiers per a cada grup de població
        var totalPopDemands = new List<LifestyleTier>
        {
            DataManager.lifestyleTiers.Find(tier => tier.TierID == chosenCity.PoorLifestyleID),
            DataManager.lifestyleTiers.Find(tier => tier.TierID == chosenCity.MidLifestyleID),
            DataManager.lifestyleTiers.Find(tier => tier.TierID == chosenCity.RichLifestyleID)
        };

        if (totalPopDemands[0] == null)
        {
            Debug.LogError($"No s'ha trobat LifestyleTier per PoorLifestyleID: {chosenCity.PoorLifestyleID}");
        }
        if (totalPopDemands[1] == null)
        {
            Debug.LogError($"No s'ha trobat LifestyleTier per MidLifestyleID: {chosenCity.MidLifestyleID}");
        }
        if (totalPopDemands[2] == null)
        {
            Debug.LogError($"No s'ha trobat LifestyleTier per RichLifestyleID: {chosenCity.RichLifestyleID}");
        }

        int[] populations = { chosenCity.PoorPopulation, chosenCity.MidPopulation, chosenCity.RichPopulation };
        string[] populationClasses = { "Poor", "Mid", "Rich" };

        // Ara calcula les demandes de serveis per a cada grup de població
        for (int i = 0; i < totalPopDemands.Count; i++)
        {
            var tier = totalPopDemands[i];
            if (tier == null)
            {
                Debug.LogError($"El LifestyleTier per {populationClasses[i]} és null. Saltant aquest grup de població.");
                continue;
            }

            foreach (var service in tier.ServiceDemands)
            {
                var existingService = chosenCityInventory.Services.FirstOrDefault(s => s.ResourceType == service.ResType);
                
                if (existingService != null)
                {
                    // Si el servei ja existeix, només cal sumar la demanda i actualitzar la posició
                    existingService.Demand += populations[i] * service.MonthlyQty / 1000;

                    if (i == 0) existingService.PositionPoor = service.Position;
                    if (i == 1) existingService.PositionMid = service.Position;
                    if (i == 2) existingService.PositionRich = service.Position;
                }
                
                else
                {
                    // Si no existeix, crear un nou servei
                    var newService = new CityService(
                        service.ResType,
                        0,
                        populations[i] * service.MonthlyQty / 1000 
                    )
                    {
                        PositionPoor = (i == 0) ? service.Position : 0,
                        PositionMid = (i == 1) ? service.Position : 0,
                        PositionRich = (i == 2) ? service.Position : 0,
                        MinRatio = service.Minimum,
                        OptimalRatio = service.Optimum,
                        FulfilledRatio = 0  // Potser es calcula més tard
                    };
                    // Afegir el nou servei a la llista de serveis de la ciutat
                    chosenCityInventory.Services.Add(newService);
                }
            }
        }
        
        // Un cop ha acabat de repassar tots els serveis, fer el Debug.Log per cada servei
        /* foreach (var service in chosenCityInventory.Services)
        {
            Debug.Log($"Service: {service.ResourceType}, Demand: {service.Demand}, " +
                    $"Positions: {service.PositionPoor}/{service.PositionMid}/{service.PositionRich}, " +
                    $"Ratio {service.FulfilledRatio}, Min {service.MinRatio}, Opt {service.OptimalRatio}");
        } */
        
        //Debug.Log($"Processament de serveis complet per la ciutat: {chosenCity.cityName} (ID: {chosenCity.cityID})");
    }



    // DISPLAY TEXTS

    private string GetCityInventoryDisplayText()
    {
        CityData currentCity = GameManager.Instance.CurrentCity;
        CityInventory currentCityInventory = currentCity.CityInventory;
        
        if (currentCityInventory == null)
        {
            return "No s'ha assignat cap inventari de ciutat.";
        }

        StringBuilder displayText = new StringBuilder();
        displayText.AppendLine($"Inventari de la Ciutat: {currentCity.cityName} (ID: {currentCity.cityID})");
        
        displayText.AppendLine("Recursos de l'Inventari:");
        var shownResources = currentCityInventory.InventoryResources
            .OrderBy(res => res.ResourceType)
            .ThenByDescending(res => res.Quantity);
        foreach (var resline in shownResources)
        {
            var matchedResource = DataManager.resourcemasterlist.FirstOrDefault(r => r.ResourceID == resline.ResourceID);
            int basePrice = matchedResource != null ? matchedResource.BasePrice : 0; // default a zero
            string resname = matchedResource != null ? matchedResource.ResourceName : ""; 

            displayText.AppendLine($"{resline.ResourceType}: {resline.ResourceID} {resname}, " +
                       $"Qty: {resline.Quantity}, Demands: {resline.DemandConsume} / " +
                       $"{resline.DemandCritical} / {resline.DemandTotal}, " + 
                       $"Price: {resline.CurrentPrice} (base {basePrice}), " +
                       $"Positions: {resline.PositionPoor} / {resline.PositionMid} / {resline.PositionRich}");
        }

        displayText.AppendLine("\nPopulation Demands:");
        foreach (var popDemand in currentCityInventory.PopDemands)
        {
            string assignedResourceName = string.IsNullOrEmpty(popDemand.AssignedResID) ? "None" :
                DataManager.resourcemasterlist.FirstOrDefault(r => r.ResourceID == popDemand.AssignedResID)?.ResourceName ?? "None";
            
            displayText.AppendLine($"- {popDemand.Class}, RType: {popDemand.ResourceType}, " +
                               $" #{popDemand.Position} ({popDemand.DemType}), " +
                               $"Quantity: {popDemand.CoveredQty} / {popDemand.ConsumeQty}, Crit: {popDemand.CritQty}, Total: {popDemand.TotalQty}, " +
                               $"Assigned : {assignedResourceName}, 100%? {popDemand.Fulfilled}");
        }

        displayText.AppendLine("\nBuilding Demands:");
        foreach (var buildingDemand in currentCityInventory.BuildingDemands)
        {
            string assignedResourceName = string.IsNullOrEmpty(buildingDemand.AssignedResID) ? "None" :
                DataManager.resourcemasterlist.FirstOrDefault(r => r.ResourceID == buildingDemand.AssignedResID)?.ResourceName ?? "None";
            
            displayText.AppendLine($"- ResType: {buildingDemand.ResType ?? "N/A"}, Assg Res Name: {assignedResourceName ?? "N/A"}, " +
                            $"RelatedBdgID: {buildingDemand.RelatedBuildID}, " +
                            $"Demands: {buildingDemand.ConsumeQty} / {buildingDemand.CritQty} / {buildingDemand.TotalQty}");
        }

        
        return displayText.ToString();
    }

    
}