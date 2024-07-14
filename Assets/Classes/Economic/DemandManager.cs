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
        Debug.Log("Iniciant DemandManager...");
        //demandTiers = DataManager.Instance.lifestyleTiers; 
        
        // Calcula les demandes basades en la ciutat actual
        foreach (CityData city in DataManager.Instance.allCityList)
        {
            GetTierNeedsForCity(city);
            GenerateMarketDemands(city);
            CalculateAllCityPrices(city);
        }
        
       
        Debug.Log("Demandes per població a la ciutat calculades");
        
    }

    private void Update()
    {
        
        if (GlobalTime.Instance.currentDayTime >= timeToWait && !firstDemand) // Aplico directament el "daytime", que és com la hora
        {
            Debug.Log("Aplicats calculs de demanda a segon 2");

            //AssignDemandsToVarieties();
            GenerateMarketDemands(GameManager.Instance.CurrentCity);  
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
        Debug.Log($"Processant la ciutat: {chosenCity.cityName} (ID: {chosenCity.cityID})");

        CityInventory chosenCityInventory = chosenCity.CityInventory;
        if (chosenCityInventory == null)
        {
            Debug.LogError("No s'ha assignat cap inventari de ciutat.");
            return;
        }
        
        // Netejar les PopDemands existents
        chosenCityInventory.PopDemands.Clear();
        Debug.Log($"Inventari de ciutat netejat per {chosenCity.cityName}.");


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
        Debug.Log($"Processament de demandes complet per la ciutat: {chosenCity.cityName} (ID: {chosenCity.cityID})");
    }


    private void GenerateMarketDemands(CityData city)
    {
        //CityData currentCity = GameManager.Instance.CurrentCity;
        CityInventory cityInventory = city.CityInventory;

        if (cityInventory == null)
        {
            Debug.LogError("No s'ha assignat cap inventari de ciutat.");
            return;
        }

        // Netejar les MarketDemands existents
        cityInventory.MarketDemands.Clear();

        // Processar les PopulationDemands
        foreach (var populationDemand in cityInventory.PopDemands)
        {
            var existingMarketDemand = cityInventory.MarketDemands
                .FirstOrDefault(md => md.ResType == populationDemand.ResourceType && 
                                      md.PositionPoor == populationDemand.Position && 
                                      md.PositionMid == populationDemand.Position && 
                                      md.PositionRich == populationDemand.Position);

            if (existingMarketDemand != null)
            {
                switch (populationDemand.Class)
                {
                    case "Poor":
                        existingMarketDemand.PositionPoor = populationDemand.Position;
                        break;
                    case "Mid":
                        existingMarketDemand.PositionMid = populationDemand.Position;
                        break;
                    case "Rich":
                        existingMarketDemand.PositionRich = populationDemand.Position;
                        break;
                }

                existingMarketDemand.ConsumeQty += populationDemand.ConsumeQty;
                existingMarketDemand.CritQty += populationDemand.CritQty;
                existingMarketDemand.TotalQty += populationDemand.TotalQty;
            }
            else
            {
                var newMarketDemand = new ResourceDemand(
                    populationDemand.ResourceType,
                    null,
                    populationDemand.ConsumeQty,
                    populationDemand.CritQty,
                    populationDemand.TotalQty,
                    populationDemand.Class == "Poor" ? populationDemand.Position : 0,
                    populationDemand.Class == "Pid" ? populationDemand.Position : 0,
                    populationDemand.Class == "Rich" ? populationDemand.Position : 0);

                cityInventory.MarketDemands.Add(newMarketDemand);
            }
        }

        // Ordenar les MarketDemands per ConsumeQty descendent
        cityInventory.MarketDemands = cityInventory.MarketDemands
            .OrderByDescending(md => md.ConsumeQty).ToList();

        // Assignar recursos del inventari, a les demandes de població. Edificis vindran després. 
        foreach (var marketDemand in cityInventory.MarketDemands)
        {
            var matchingResources = cityInventory.InventoryResources
                .Where(ir => ir.ResourceType == marketDemand.ResType && ir.ResourceID != null)
                .OrderByDescending(ir => ir.Quantity)
                .ToList();

            foreach (var resource in matchingResources)
            {
                if (marketDemand.AssignedResource == null)
                {
                    marketDemand.AssignedResource = DataManager.resourcemasterlist.FirstOrDefault(r => r.ResourceID == resource.ResourceID);
                    resource.DemandConsume += marketDemand.ConsumeQty;
                    resource.DemandCritical += marketDemand.CritQty;
                    resource.DemandTotal += marketDemand.TotalQty;
                }
            }
        }

        // Processar les BuildingDemands
        foreach (var buildingDemand in cityInventory.BuildingDemands)
        {
            if (buildingDemand.AssignedResource != null)
        {
            var existingMarketDemandWithResource = cityInventory.MarketDemands
                .FirstOrDefault(md => md.ResType == buildingDemand.ResType && md.AssignedResource != null && md.AssignedResource.ResourceID == buildingDemand.AssignedResource.ResourceID);

            if (existingMarketDemandWithResource != null)
            {
                // Sumar les quantitats a la línia existent amb el mateix ResourceAssigned
                existingMarketDemandWithResource.ConsumeQty += buildingDemand.ConsumeQty;
                existingMarketDemandWithResource.CritQty += buildingDemand.CritQty;
                existingMarketDemandWithResource.TotalQty += buildingDemand.TotalQty;
            }
            else
            {
                // Si no hi ha cap línia amb el mateix ResourceAssigned, buscar per ResType
                var existingMarketDemand = cityInventory.MarketDemands
                    .FirstOrDefault(md => md.ResType == buildingDemand.ResType && md.AssignedResource == null);

                if (existingMarketDemand != null)
                {
                    // Assignar el Resource i sumar les quantitats
                    existingMarketDemand.AssignedResource = buildingDemand.AssignedResource;
                    existingMarketDemand.ConsumeQty += buildingDemand.ConsumeQty;
                    existingMarketDemand.CritQty += buildingDemand.CritQty;
                    existingMarketDemand.TotalQty += buildingDemand.TotalQty;
                }
                else
                {
                    // Crear una nova línia de ResourceDemand
                    var newMarketDemand = new ResourceDemand(
                        buildingDemand.ResType,
                        buildingDemand.AssignedResource,
                        buildingDemand.ConsumeQty,
                        buildingDemand.CritQty,
                        buildingDemand.TotalQty,
                        0,
                        0,
                        0);

                    cityInventory.MarketDemands.Add(newMarketDemand);
                }
            }
        }
        else
        {
            var existingMarketDemand = cityInventory.MarketDemands
                .FirstOrDefault(md => md.ResType == buildingDemand.ResType);

            if (existingMarketDemand != null)
            {
                existingMarketDemand.ConsumeQty += buildingDemand.ConsumeQty;
                existingMarketDemand.CritQty += buildingDemand.CritQty;
                existingMarketDemand.TotalQty += buildingDemand.TotalQty;
            }
            else
            {
                var newMarketDemand = new ResourceDemand(
                    buildingDemand.ResType,
                    null,
                    buildingDemand.ConsumeQty,
                    buildingDemand.CritQty,
                    buildingDemand.TotalQty,
                    0,
                    0,
                    0);

                cityInventory.MarketDemands.Add(newMarketDemand);
            }
        }
        }

        // Ordenar de nou després de processar les BuildingDemands
        cityInventory.MarketDemands = cityInventory.MarketDemands
            .OrderByDescending(md => md.ConsumeQty).ToList();

        Debug.Log("MarketDemands generades correctament.");
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
        displayText.AppendLine("Recursos d'Inventari:");
        
        foreach (var resline in currentCityInventory.InventoryResources)
        {
            var matchedResource = DataManager.resourcemasterlist.FirstOrDefault(r => r.ResourceID == resline.ResourceID);
            int basePrice = matchedResource != null ? matchedResource.BasePrice : 0; // default a zero

            displayText.AppendLine($"{resline.ResourceID}, Type: {resline.ResourceType}, " +
                           $"Qty: {resline.Quantity}, Demands: {resline.DemandConsume} / " +
                           $"{resline.DemandCritical} / {resline.DemandTotal}, " + 
                           $"Current price: {resline.CurrentPrice} (Base price: {basePrice})");
        }

        displayText.AppendLine("\nDemandes:");
        foreach (var demand in currentCityInventory.MarketDemands)
        {
            displayText.AppendLine($"- Rtype: {demand.ResType}, Demands: {demand.ConsumeQty} /" +
                                $" {demand.CritQty} / {demand.TotalQty}");
        }

        return displayText.ToString();
    }

    
}