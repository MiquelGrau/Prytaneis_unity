using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        building.LinearOutput = 100;
        building.InputEfficiency = 100;
        building.OutputEfficiency = 100;
        building.CycleEfficiency = 100;
        building.SalaryEfficiency = 100;

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

    public void UpdateProduction()
    {
        if (GameManager.Instance.CurrentCity != null)
        {
            foreach (var building in GameManager.Instance.CurrentCity.CityBuildings)
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
            foreach (var output in batch.BatchOutputs)
            {
                if (Random.Range(0, 100) < output.OutputChance)
                {
                    var resource = GameManager.Instance.CurrentCity.CityInventory.InventoryResources.Find(r => r.ResourceID == output.OutputResource.ResourceID);
                    if (resource != null)
                    {
                        resource.Quantity += output.OutputAmount;
                    }
                    else
                    {
                        GameManager.Instance.CurrentCity.CityInventory.InventoryResources.Add(new CityInventoryResource(output.OutputResource.ResourceID, output.OutputAmount, (int)output.OutputUnitValue));
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

    public void SetupNewBatch(ProductionMethod method, ProductiveBuilding building)
    {
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


    
}
