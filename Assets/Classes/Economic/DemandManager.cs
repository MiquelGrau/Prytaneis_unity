using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Text;
using UnityEngine;

public class DemandManager : MonoBehaviour
{
    
    public DataManager dataManager; 
    //public CityData currentCity  { get; private set; }
    //public CityInventory currentCityInventory { get; private set; } 
    
    //public List<CityData> cities; 
    public List<LifestyleTier> lifestyleTiers; 
    
    private float timer = 0f;
    private float timeToWait = 6f; 
    
    public TextMeshProUGUI inventoryDisplayText; 

    private void Start()
    {  
        Debug.Log("Iniciant DemandManager...");
        // Obté el llistat de ciutats
        /* cities = dataManager.GetCities(); 
        if (cities == null || cities.Count == 0)        // Comprova si 'cities' és null
        {
            Debug.LogError("No s'han pogut carregar les ciutats.");
            return;
        } */
        
        // Assigna la ciutat actual des de GameManager
        //AssignCurrentCity(GameManager.Instance.CurrentCity);
        lifestyleTiers = DataManager.lifestyleTiers; 
        
        
        // Calcula les demandes basades en la ciutat actual
        foreach (CityData city in dataManager.allCityList)
        {
            GetTierNeedsForCity(city);
        }
        Debug.Log("Demandes per població a la ciutat calculades");

        
        

    }

    private void Update()
    {
        timer += Time.deltaTime; // Aquesta línia incrementa el temporitzador basant-se en el temps real que ha passat

        if (timer >= timeToWait)
        {
            // Reset del temporitzador
            timer = 0f;

            Debug.Log("Tic tac! Temporitzador cada 6 segons.");

            //AssignDemandsToVarieties();
            GenerateMarketDemands();
            inventoryDisplayText.text = GetCityInventoryDisplayText();
            CalculatePrices();
        }
    }

    /* private void GetTierNeedsForCity(CityData chosenCity) 
    {
        if (chosenCity == null)
        {
            Debug.LogError("La ciutat passada a GetTierNeedsForCity és null.");
            return;
        }

        CityInventory chosenCityInventory = chosenCity.CityInventory;
        if (chosenCityInventory  == null)
        {
            Debug.LogError("No s'ha assignat cap inventari de ciutat.");
            return;
        }
        
        // Netejar les CityDemands existents
        chosenCityInventory.Demands.Clear();

        // Obtenir els LifestyleTiers per a cada grup de població
        LifestyleTier[] specificTiers = {
            lifestyleTiers.Find(tier => tier.TierID == chosenCity.PoorLifestyleID),
            lifestyleTiers.Find(tier => tier.TierID == chosenCity.MidLifestyleID),
            lifestyleTiers.Find(tier => tier.TierID == chosenCity.RichLifestyleID)
        };

        int[] populations = { chosenCity.PoorPopulation, chosenCity.MidPopulation, chosenCity.RichPopulation };
        string[] populationNames = { "pobra", "mitjana", "rica" };


        // Inicialitzar o resetejar les línies de CityInventoryResource per a cada ResourceType
        foreach (var tier in specificTiers)
        {
            foreach (var need in tier.LifestyleDemands)
            {
                var headerResource = chosenCityInventory.InventoryResources
                    .FirstOrDefault(resline => resline.ResourceType == need.resourceType && resline.ResourceID == null);

                if (headerResource == null)
                {
                    headerResource = new CityInventoryResource(need.resourceType);
                    chosenCityInventory.InventoryResources.Add(headerResource);
                }
                else
                {
                    headerResource.Quantity = 0;
                    headerResource.DemandConsume = 0;
                    headerResource.DemandCritical = 0;
                    headerResource.DemandTotal = 0;
                }
            }
        }

        // Ara calcula les demandes per a cada grup de població. Es queda guardat a DEMANDS, no a inventory. 
        for (int i = 0; i < specificTiers.Length; i++)
        {
            LifestyleTier tier = specificTiers[i];
            // Debug part
            if (tier == null)
            {
                Debug.LogError($"El tier per a la població {populationNames[i]} és null.");
                continue;
            }
            
            // Per a cada tier que tinc a la ciutat (es a dir, els 3 que hi ha), llista les needs i volca-ho al CityInventory
            foreach (var need in tier.LifestyleDemands)
            {
                
                // Inicialitzar o resetejar les línies de CityInventoryResource basades en ResourceType
                var headerResource = chosenCityInventory.InventoryResources
                .FirstOrDefault(resline => resline.ResourceType == need.resourceType && resline.ResourceID == null);
                
                CityInventory.CityDemands newDemand = new CityInventory.CityDemands(
                    CityInventory.CityDemands.DemandType.ResourceType, 
                    need.resourceType, 
                    populationNames[i], 
                    need.resourceVariety);
                
                // Calcular les demandes
                newDemand.DemandConsume = populations[i] * need.quantityPerThousand / 1000;
                newDemand.DemandCritical = newDemand.DemandConsume * need.monthsCritical;
                newDemand.DemandTotal = newDemand.DemandConsume * need.monthsTotal;

                // Afegir a la llista de CityDemands
                chosenCityInventory.Demands.Add(newDemand);

                    
                // Mou al Inventoryresource
                headerResource.DemandConsume += newDemand.DemandConsume;
                headerResource.DemandCritical += newDemand.DemandCritical;
                headerResource.DemandTotal += newDemand.DemandTotal;
     
                // Log de la informació
                //Debug.Log($"ResourceType: {newDemand.ResourceType}, PopulationType: {newDemand.PopulationType}, " +
                //        $"Variety: {newDemand.Variety}, Demands: {newDemand.DemandConsume} / " +
                //        $"{newDemand.DemandCritical} / {newDemand.DemandTotal}");
                
            }
        }

    } */

    private void GetTierNeedsForCity(CityData chosenCity) 
    {
        // Assignem
        if (chosenCity == null)
        {
            Debug.LogError("La ciutat passada a GetTierNeedsForCity és null.");
            return;
        }

        CityInventory chosenCityInventory = chosenCity.CityInventory;
        if (chosenCityInventory == null)
        {
            Debug.LogError("No s'ha assignat cap inventari de ciutat.");
            return;
        }
        
        // Netejar les PopDemands existents
        chosenCityInventory.PopDemands.Clear();

        // Obtenir els LifestyleTiers per a cada grup de població
        var totalPopDemands = new List<LifestyleTier>
        {
            lifestyleTiers.Find(tier => tier.TierID == chosenCity.PoorLifestyleID),
            lifestyleTiers.Find(tier => tier.TierID == chosenCity.MidLifestyleID),
            lifestyleTiers.Find(tier => tier.TierID == chosenCity.RichLifestyleID)
        };

        int[] populations = { chosenCity.PoorPopulation, chosenCity.MidPopulation, chosenCity.RichPopulation };
        string[] populationClasses = { "Poor", "Mid", "Rich" };

        // Ara calcula les demandes per a cada grup de població
        for (int i = 0; i < totalPopDemands.Count; i++)
        {
            var tier = totalPopDemands[i];
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
    }


    /* public void AssignDemandsToVarieties()
    {
        CityData currentCity = GameManager.Instance.CurrentCity;
        CityInventory currentCityInventory = currentCity.CityInventory;

        // Primer, netejar les demandes existents de linies de recursos d'inventari
        foreach (var resline in currentCityInventory.InventoryResources)
        {
            if (resline.ResourceID != null)
            {
                resline.DemandConsume = 0;
                resline.DemandCritical = 0;
                resline.DemandTotal = 0;
            }
        }

        // Assignar les demandes a les varietats
        foreach (var demand in currentCityInventory.Demands)
        {
            // Ordenar els resource lines d'inventari per quantitat, de major a menor
            var sortedInventoryResLines = currentCityInventory.InventoryResources
                .Where(resline => resline.ResourceType == demand.ResourceType && resline.ResourceID != null)
                .OrderByDescending(resline => resline.Quantity)
                .ToList();

            // Assignar les demandes a les varietats
            for (int i = 0; i < demand.Variety && i < sortedInventoryResLines.Count; i++)
            {
                var resline = sortedInventoryResLines[i];
                resline.DemandConsume += demand.DemandConsume / demand.Variety;
                resline.DemandCritical += demand.DemandCritical / demand.Variety;
                resline.DemandTotal += demand.DemandTotal / demand.Variety;

                // Buscar el nom del recurs
                var matchedResource = DataManager.resourcemasterlist.FirstOrDefault(r => r.ResourceID == resline.ResourceID);
                string resourceName = matchedResource != null ? matchedResource.ResourceName : "Desconegut";

                // Afegir log per a cada resource line
                //Debug.Log($"ResourceType: {resline.ResourceType}, ID: {resline.ResourceID}, {resourceName}, Qty: {resline.Quantity} " +
                //$"Demands: {resline.DemandConsume} / {resline.DemandCritical} / {resline.DemandTotal}");
            }
        }

        // Sumar les quantitats per a cada ResourceType i assignar-les als elements header
        foreach (var resourceType in currentCityInventory.InventoryResources.Select(resline => resline.ResourceType).Distinct())
        {
            var headerResource = currentCityInventory.InventoryResources
                .FirstOrDefault(resline => resline.ResourceType == resourceType && resline.ResourceID == null);

            if (headerResource != null)
            {
                float totalQuantity = currentCityInventory.InventoryResources
                    .Where(resline => resline.ResourceType == resourceType && resline.ResourceID != null)
                    .Sum(resline => resline.Quantity);

                headerResource.Quantity = totalQuantity;

                // Opcional: Afegir un log per confirmar l'assignació
                //Debug.Log($"HeaderResource per a {resourceType}: Total Quantity = {totalQuantity}");
            }
        }


    } */
    
    private void GenerateMarketDemands()
    {
        CityData currentCity = GameManager.Instance.CurrentCity;
        CityInventory currentCityInventory = currentCity.CityInventory;

        if (currentCityInventory == null)
        {
            Debug.LogError("No s'ha assignat cap inventari de ciutat.");
            return;
        }

        // Netejar les MarketDemands existents
        currentCityInventory.MarketDemands.Clear();

        // Processar les PopulationDemands
        foreach (var populationDemand in currentCityInventory.PopDemands)
        {
            var existingMarketDemand = currentCityInventory.MarketDemands
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

                currentCityInventory.MarketDemands.Add(newMarketDemand);
            }
        }

        // Ordenar les MarketDemands per ConsumeQty descendent
        currentCityInventory.MarketDemands = currentCityInventory.MarketDemands
            .OrderByDescending(md => md.ConsumeQty).ToList();

        // Assignar recursos del inventari, a les demandes de població. Edificis vindran després. 
        foreach (var marketDemand in currentCityInventory.MarketDemands)
        {
            var matchingResources = currentCityInventory.InventoryResources
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
        foreach (var buildingDemand in currentCityInventory.BuildingDemands)
        {
            if (buildingDemand.AssignedResource != null)
        {
            var existingMarketDemandWithResource = currentCityInventory.MarketDemands
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
                var existingMarketDemand = currentCityInventory.MarketDemands
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

                    currentCityInventory.MarketDemands.Add(newMarketDemand);
                }
            }
        }
        else
        {
            var existingMarketDemand = currentCityInventory.MarketDemands
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

                currentCityInventory.MarketDemands.Add(newMarketDemand);
            }
        }
        }

        // Ordenar de nou després de processar les BuildingDemands
        currentCityInventory.MarketDemands = currentCityInventory.MarketDemands
            .OrderByDescending(md => md.ConsumeQty).ToList();

        Debug.Log("MarketDemands generades correctament.");
    }

    public void CalculatePrices()
    {
        CityData currentCity = GameManager.Instance.CurrentCity;
        CityInventory currentCityInventory = currentCity.CityInventory;

        foreach (var resline in currentCityInventory.InventoryResources)
        {
            if (resline.ResourceID != null)
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
        }
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