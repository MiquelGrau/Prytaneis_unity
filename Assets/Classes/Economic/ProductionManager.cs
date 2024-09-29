using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProductionManager : MonoBehaviour
{
    public static ProductionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        
    }

    public void UpdateProdEfficiencies(ProductiveBuilding building)
    {
        // Reinicialitzar les eficiències
        building.LinearOutput = 1;
        building.InputEfficiency = 1;
        building.OutputEfficiency = 1;
        building.CycleEfficiency = 1;
        building.SalaryEfficiency = 1;

        foreach (var factor in building.CurrentFactors)
        {
            float effectSize = 0;

            // Binaris i lineals, segons quant de ple està aquest factor
            if (factor is EmployeePT employeeFactor)
            {
                var template = employeeFactor.FactorTemplate as EmployeeFT;
                if (template != null)
                {
                    if (employeeFactor.CurrentEmployees >= template.Optimal)        { effectSize = 1; }
                    else if (employeeFactor.CurrentEmployees <= template.Minimum)   { effectSize = 0; }
                    else
                    {
                        effectSize = (float)employeeFactor.EffectSize / (employeeFactor.CurrentEmployees - template.Minimum);
                    }
                }
            }
            else if (factor is ResourcePT resourceFactor)
            {
                if (resourceFactor.CurrentQuantity >= resourceFactor.MonthlyConsumption) { effectSize = 1; }
                else
                {
                    effectSize = (float)resourceFactor.EffectSize / resourceFactor.MonthlyConsumption;
                }
            }

            switch (factor.FactorType)
            {
                case "Linear":
                    building.LinearOutput *= effectSize;
                    break;
                case "Input buff":
                    building.InputEfficiency *= effectSize;
                    break;
                case "Output buff":
                    building.OutputEfficiency *= effectSize;
                    break;
                // anem afegint totes les definicions, crec que n'hi haurà unes 8 o 9. 
            }
        }

        Debug.Log($"Actualitzades eficiencies per: {building.BuildingName} - LinearOutput: {building.LinearOutput}, " + 
                $"InputEfficiency: {building.InputEfficiency}, OutputEfficiency: {building.OutputEfficiency}, "+
                $"CycleEfficiency: {building.CycleEfficiency}, SalaryEfficiency: {building.SalaryEfficiency}");
    }

    public void DebugUpdateProduction()
    {
        if (GameManager.Instance.currentCity != null)
        {
            foreach (var building in GameManager.Instance.currentCity.Buildings)
            {
                if (building is ProductiveBuilding productiveBuilding)
                {
                    UpdateProdEfficiencies(productiveBuilding);

                    // Incrementar el CycleTimeProgress del BatchCurrent
                    if (productiveBuilding.BatchCurrent != null)
                    {
                        int previousCycleTime = productiveBuilding.BatchCurrent.CycleTimeProgress;
                        productiveBuilding.BatchCurrent.CycleTimeProgress += 100;
                        int newCycleTime = productiveBuilding.BatchCurrent.CycleTimeProgress;
                        Debug.Log($"Avançat en la producció de {productiveBuilding.BuildingName}: Progrés: {productiveBuilding.BatchCurrent.CycleTimeProgress}");

                        // Verificar si el lot s'ha completat
                        if (productiveBuilding.BatchCurrent.CycleTimeProgress >= productiveBuilding.BatchCurrent.CycleTimeTotal)
                        {
                            CompleteBatch(productiveBuilding, newCycleTime - productiveBuilding.BatchCurrent.CycleTimeTotal);
                        }
                    }

                }
            }
        }
    }

    
    private void CompleteBatch(ProductiveBuilding building, int overflowTime)
    {
        var batch = building.BatchCurrent;

        if (batch != null && batch.IsCompleted())
        {
            CityInventory cityInventory = DataManager.Instance.GetCityInvByID(building.RelatedInventoryID);
            if (cityInventory == null)
            {
                Debug.LogError("No s'ha trobat l'inventari de la ciutat.");
                return;
            }
            
            foreach (var output in batch.BatchOutputs)
            {
                if (Random.Range(0, 100) < output.OutputChance)
                {
                    //var resource = GameManager.Instance.CurrentCity.CityInventory.InventoryResources.Find(r => r.ResourceID == output.OutputResource.ResourceID);
                    var resource = cityInventory.InventoryResources.Find(r => r.ResourceID == output.OutputResource.ResourceID);
                    if (resource != null)
                    {
                        resource.Quantity += output.OutputAmount;
                    }
                    else
                    {
                        //GameManager.Instance.CurrentCity.CityInventory.InventoryResources.Add(new CityInventoryResource(output.OutputResource.ResourceID, output.OutputAmount, (int)output.OutputUnitValue));
                        cityInventory.InventoryResources.Add(new CityInventoryResource(output.OutputResource.ResourceID, output.OutputAmount, (int)output.OutputUnitValue));
                    }
                }
            }

            building.BatchCurrent = null;

            if (building.BatchBacklog.Count > 0)    // Si hi ha un proper batch, l'agafa li assigna el que ja portava
            {
                var nextBatch = building.BatchBacklog[0];
                building.BatchBacklog.RemoveAt(0);
                building.BatchCurrent = nextBatch;
                building.BatchCurrent.CycleTimeProgress = overflowTime;
            }

            Debug.Log($"Batch completed for Building: {building.BuildingName}, Overflow Time: {overflowTime}");
        }
    }

    public void SetupNewBatch(string methodID, ProductiveBuilding building)
    {
        var method = DataManager.Instance.GetProductionMethodByID(methodID);  
        if (method == null)
        {
            Debug.LogError($"No s'ha trobat ProductionMethod amb ID: {methodID}");
            return;
        }
        
        var inputs = new List<Batch.BatchInput>();
        foreach (var input in method.Inputs)
        {
            var resource = DataManager.Instance.GetResourceByID(input.ResourceID);
            if (resource != null)
            {
                var inputAmount = input.Amount * building.InputEfficiency * building.LinearOutput;  // Sempre linear, per building size
                inputs.Add(new Batch.BatchInput(resource, inputAmount, 0)); // InputUnitValue a zero, serà configurat més tard
            }
        }

        var outputs = new List<Batch.BatchOutput>();
        foreach (var output in method.Outputs)
        {
            var resource = DataManager.Instance.GetResourceByID(output.ResourceID);
            if (resource != null)
            {
                var outputAmount = output.Amount * building.OutputEfficiency * building.LinearOutput;   // Sempre linear també
                outputs.Add(new Batch.BatchOutput(resource, outputAmount, output.Chance, 1, 0)); // OutputExpGain i OutputUnitValue
            }
        }

        var newBatch = new Batch(method.MethodID, inputs, outputs, method.CycleTime * (int)building.CycleEfficiency, 0, 0, 0);

        if (building.BatchCurrent == null)
        {
            building.BatchCurrent = newBatch;
        }
        else
        {
            building.BatchBacklog.Add(newBatch);
        }

        Debug.Log($"New batch set up for Building: {building.BuildingName} with Method: {method.MethodName}");
    }

    public void KickstartProductives(ProductiveBuilding building)
    {
        // Obtenir l'inventari associat a l'edifici
        CityInventory cityInventory = DataManager.Instance.cityInventories
            .FirstOrDefault(inv => inv.CityInvID == building.RelatedInventoryID);

        if (cityInventory == null)
        {
            Debug.LogError($"No s'ha trobat l'inventari per l'edifici: {building.BuildingName}");
            return;
        }

        // Identificar el mètode de producció per defecte
        var methodDefault = building.MethodDefault;
        if (string.IsNullOrEmpty(methodDefault))
        {
            Debug.LogError($"No hi ha cap MethodDefault per a l'edifici: {building.BuildingName}");
            return;
        }

        var method = DataManager.Instance.GetProductionMethodByID(methodDefault);
        if (method == null)
        {
            Debug.LogError($"No s'ha trobat ProductionMethod amb ID: {methodDefault}");
            return;
        }

        Debug.Log($"Iniciant producció a l'edifici: {building.BuildingName}. MethodDefault: {method.MethodName}");


        // Comprovació dels inputs i disponibilitat en l'inventari
        foreach (var input in method.Inputs)
        {
            var inventoryResource = cityInventory.InventoryResources
                .FirstOrDefault(res => res.ResourceID == input.ResourceID);

            if (inventoryResource == null || inventoryResource.Quantity < input.Amount)
            {
                Debug.Log($"No hi ha prou quantitat de {input.ResourceID} per a l'edifici {building.BuildingName}.");
                return; // Aturem si no hi ha prou recursos
            }
            Debug.Log($"Revisant recurs: {input.ResourceID}. Requerit: {input.Amount}, Disponible: {inventoryResource.Quantity}");
        }

        // Si tot està correcte, crear un nou batch i començar la producció
        SetupNewBatch(methodDefault, building);

        // Restar els recursos de l'inventari i actualitzar els valors
        foreach (var batchInput in building.BatchCurrent.BatchInputs)
        {
            var inventoryResource = cityInventory.InventoryResources
                .FirstOrDefault(res => res.ResourceID == batchInput.InputResource.ResourceID);

            if (inventoryResource != null)
            {
                Debug.Log($"Consumint {batchInput.InputAmount} del recurs {inventoryResource.ResourceID}. " +
                $"Quantitat restant abans de restar: {inventoryResource.Quantity}");

                // Restar la quantitat consumida de l'inventari
                inventoryResource.Quantity -= batchInput.InputAmount;
                
                // Transferir el CurrentValue de l'inventari a InputUnitValue del BatchInput
                //batchInput.InputUnitValue = inventoryResource.CurrentValue;
            }
        }

        // Activar la producció
        building.ProdActive = true;

        Debug.Log($"Producció iniciada a l'edifici {building.BuildingName} amb el mètode {method.MethodName}. "+
        $"Batch creat amb {building.BatchCurrent.BatchInputs.Count} inputs de diferents recursos.");
    }



    // Funció per calcular quants cicles de producció es poden fer
    public int CalculateAvailableProductionCycles(ProductiveBuilding building)
    {
        // Comprovem si la producció està activa
        if (!building.ProdActive)
        {
            return 0; // Si no està activa, no es pot produir
        }

        // Obtenim el batch actual
        Batch currentBatch = building.BatchCurrent;
        if (currentBatch == null || currentBatch.BatchInputs == null)
        {
            return 0; // Si no hi ha cap batch, no es pot produir
        }

        // Obtenim l'inventari relacionat amb l'edifici des de DataManager
        CityInventory cityInventory = DataManager.Instance.cityInventories
            .FirstOrDefault(inv => inv.CityInvID == building.RelatedInventoryID);

        if (cityInventory == null)
        {
            return 0; // Si no hi ha inventari, no es pot produir
        }

        int maxCycles = 999; // El màxim nombre de cicles que es poden fer és 999

        // Iterem sobre cada recurs input del batch
        foreach (var input in currentBatch.BatchInputs)
        {
            // Busquem la quantitat disponible del recurs a l'inventari
            var inventoryResource = cityInventory.InventoryResources
                .FirstOrDefault(res => res.ResourceID == input.InputResource.ResourceID);

            if (inventoryResource == null || inventoryResource.Quantity <= 0)
            {
                return 0; // Si no hi ha prou recursos, no es pot fer cap cicle
            }

            // Calculem quants cicles es poden fer amb el recurs actual
            int availableCyclesForResource = Mathf.FloorToInt(inventoryResource.Quantity / input.InputAmount);

            // El nombre màxim de cicles es limita al recurs que menys permet
            maxCycles = Mathf.Min(maxCycles, availableCyclesForResource);
        }

        return maxCycles;
    }


    
}
