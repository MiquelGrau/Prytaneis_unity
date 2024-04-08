using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using System.Linq;

public class BuildingManager : MonoBehaviour
{
    public GameManager gameManager;
    public DataManager dataManager;

    // Desplegable i botó inicial
    public TMP_Dropdown buildingDropdown;
    public Button createBuildingButton;
    
    
    private void Start()
    {
        createBuildingButton.onClick.AddListener(CreateNewBuilding);
        PopulateDropdown();
    }

    private void PopulateDropdown()
    {
        List<string> buildingNames = new List<string>();

        foreach (var template in dataManager.productiveTemplates)
        {
            Debug.Log($"Afegint template productiu: {template.ClassName}");
            buildingNames.Add(template.ClassName);
        }

        foreach (var template in dataManager.civicTemplates)
        {
            Debug.Log($"Afegint template cívic: {template.ClassName}");
            buildingNames.Add(template.ClassName);
        }

        buildingDropdown.ClearOptions();
        buildingDropdown.AddOptions(buildingNames);
    }

    private void CreateNewBuilding()
    {
        // Assumeix que l'opció seleccionada en el desplegable correspon al índex de la plantilla en la llista combinada de totes les plantilles
        int selectedIndex = buildingDropdown.value;
        
        // Calcular l'índex real tenint en compte que el llistat combina productius i cívics
        bool isProductive = selectedIndex < dataManager.productiveTemplates.Count;
        string selectedTemplateID = isProductive ? 
            dataManager.productiveTemplates[selectedIndex].TemplateID : 
            dataManager.civicTemplates[selectedIndex - dataManager.productiveTemplates.Count].TemplateID;
        
        BuildingTemplate selectedTemplate = FindTemplateByID(selectedTemplateID);

        if (selectedTemplate is ProductiveTemplate)
        {
            AddProductiveBuilding(selectedTemplate as ProductiveTemplate);
        }
        else if (selectedTemplate is CivicTemplate)
        {
            AddCivicBuilding(selectedTemplate as CivicTemplate);
        }
    } 

    private BuildingTemplate FindTemplateByID(string templateID)
    {
        // Intenta trobar el template en la llista de productiveTemplates
        BuildingTemplate template = dataManager.productiveTemplates.FirstOrDefault(t => t.TemplateID == templateID);

        // Si no es troba en productiveTemplates, busca en civicTemplates
        if (template == null)
        {
            template = dataManager.civicTemplates.FirstOrDefault(t => t.TemplateID == templateID);
        }

        return template;
    }


    private void AddProductiveBuilding(ProductiveTemplate template)
    {
        // Aquí crearem la lògica per a crear un nou ProductiveBuilding amb la informació de la template
        ProductiveBuilding newBuilding = new ProductiveBuilding(
            DataManager.Instance.GenerateBuildingID(),
            template.ClassName,
            template.TemplateID,
            gameManager.CurrentCity.cityID,
            null, // OwnerID buit
            gameManager.CurrentCity.cityInventoryID,
            null, // Estat inicial d'activitat
            10, // Mida de l'edifici
            0, // HPCurrent
            0, // HPMaximum
            template.Capacity,
            template.TemplateID,
            new List<ProductiveFactor>(), // CurrentFactors com a llistat buit per ara
            template.PossibleMethods,
            null, // Method Active, no s'està fabricant res encara. 
            template.DefaultMethod,
            null, // BatchCurrent com a null per ara
            new List<string>(), // BatchBacklog com a llistat buit per ara
            1.0f, // InputEfficiency
            1.0f, // OutputEfficiency
            1.0f, // CycleEfficiency
            1.0f, // SalaryEfficiency
            template.JobsPoor,
            template.JobsMid,
            template.JobsRich
        );

        // Aquí es podria afegir el nou edifici a una llista d'edificis dins de la ciutat actual, per exemple
        SetupFactors(newBuilding, template);
        AddBuildingToCurrentCity(newBuilding);
        Debug.Log("Nou edifici productiu creat: " + newBuilding.BuildingName);
    }

    private void AddCivicBuilding(CivicTemplate template)
    {
        // Aquí crearem la lògica per a crear un nou CivicBuilding amb la informació de la template
        CivicBuilding newBuilding = new CivicBuilding(
            DataManager.Instance.GenerateBuildingID(),
            template.ClassName,
            template.TemplateID,
            gameManager.CurrentCity.cityID,
            "", // OwnerID buit
            gameManager.CurrentCity.cityInventoryID,
            "Inactive", // Estat inicial d'activitat
            10, // Mida de l'edifici
            0, // HPCurrent
            0, // HPMaximum
            template.Capacity,
            template.Function,
            template.JobsPoor,
            template.JobsMid,
            template.JobsRich
        );

        // Aquí es podria afegir el nou edifici a una llista d'edificis dins de la ciutat actual, per exemple
        AddBuildingToCurrentCity(newBuilding);
        Debug.Log("Nou edifici cívic creat: " + newBuilding.BuildingName);
    }


    public void AddBuildingToCurrentCity(Building newBuilding)
    {
        CityData currentCity = GameManager.Instance.CurrentCity;
        if (currentCity != null)
        {
            // Si la llista d'edificis de la ciutat és null, inicialitza-la
            if (currentCity.CityBuildings == null)
            {
                currentCity.CityBuildings = new List<Building>();
            }

            // Afegeix el nou edifici a la llista
            currentCity.CityBuildings.Add(newBuilding);

            Debug.Log($"Edifici afegit a la ciutat {currentCity.cityName}: {newBuilding.BuildingName}");
        }
        else
        {
            Debug.LogError("No hi ha cap ciutat seleccionada per afegir l'edifici.");
        }
    }

    public void SetupFactors(ProductiveBuilding building, ProductiveTemplate template)
    {
        /* // Crear la llista de factors productius per a l'edifici si no existeix
        if (building.CurrentFactors == null)
        {
            building.CurrentFactors = new List<ProductiveFactor>();
            Debug.Log($"[SetupFactors] Inicialitzada la llista de factors per l'edifici amb ID: {building.BuildingID}");
        } */

        // Agafar el CityInventory on està situat l'edifici
        CityInventory cityInventory = gameManager.CurrentCity.CityInventory;
        Debug.Log($"[SetupFactors] Obtenint inventari de la ciutat: {gameManager.CurrentCity.cityName}");

        foreach (TemplateFactor templateFactor in template.Factors)
        {
            if (templateFactor is EmployeeFT employeeFT)
            {
                // Crear un nou EmployeePT i assignar-li les propietats
                EmployeePT newEmployeeFactor = new EmployeePT(
                    templateFactor,
                    building.ProductionTempID,
                    0, // EffectSize inicial és zero
                    0, // CurrentEmployees inicial és zero
                    0 * 10 // MonthlySalary, multiplica el nombre d'empleats per 10
                );

                // Afegir el nou factor a la llista de factors de l'edifici
                building.AddFactor(newEmployeeFactor);
                Debug.Log($"[SetupFactors] Afegit nou factor empleat: {newEmployeeFactor.FactorName} a l'edifici: {building.BuildingID}");
            }
            else if (templateFactor is ResourceFT resourceFT)
            {
                // Trobar la quantitat actual del recurs en el CityInventory
                float currentQuantity = cityInventory.InventoryResources
                    .FirstOrDefault(r => r.ResourceID == resourceFT.FResource.ResourceID)?.Quantity ?? 0f;

                // Crear un nou ResourcePT i assignar-li les propietats
                ResourcePT newResourceFactor = new ResourcePT(
                    templateFactor,
                    building.ProductionTempID,
                    0, // EffectSize inicial és zero
                    resourceFT.FResource, // El recurs associat
                    currentQuantity, // Quantitat actual del recurs
                    0, // MonthlyConsumption inicial és zero
                    0  // MonthlyValue inicial és zero
                );

                // Afegir el nou factor a la llista de factors de l'edifici
                building.AddFactor(newResourceFactor);
                Debug.Log($"[SetupFactors] Afegit nou factor recurs: {newResourceFactor.FactorName} amb quantitat actual: {newResourceFactor.CurrentQuantity} a l'edifici: {building.BuildingID}");
            }
        }

        Debug.Log($"[SetupFactors] Factors configurats per a l'edifici: {building.BuildingID} amb un total de {building.CurrentFactors.Count} factors.");
    }


}