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
    public CityInterface cityInterface;

    // Desplegable i botó inicial
    public TMP_Dropdown buildingDropdown;
    public Button createBuildingButton;
    
    
    private void Start()
    {
        PopulateBuildingDropdown();
        createBuildingButton.onClick.AddListener(CreateNewBuilding);
        cityInterface.UpdateBuildingGridForCity(gameManager.CurrentCity);

        // Agents
        cityInterface.UpdateAgentGrid();
        
    }

    private void PopulateBuildingDropdown()
    {
        List<string> buildingNames = new List<string>();

        foreach (var template in DataManager.Instance.productiveTemplates)
        {
            //Debug.Log($"Afegint template productiu: {template.ClassName}");
            buildingNames.Add(template.ClassName);
        }

        foreach (var template in DataManager.Instance.civicTemplates)
        {
            //Debug.Log($"Afegint template cívic: {template.ClassName}");
            buildingNames.Add(template.ClassName);
        }
        Debug.Log($"Llista d'edificis disponibles: {buildingNames.Count}");

        buildingDropdown.ClearOptions();
        buildingDropdown.AddOptions(buildingNames);
    }

    private void CreateNewBuilding()
    {
        // Assumeix que l'opció seleccionada en el desplegable correspon al índex de la plantilla en la llista combinada de totes les plantilles
        int selectedIndex = buildingDropdown.value;
        
        // Calcular l'índex real tenint en compte que el llistat combina productius i cívics
        bool isProductive = selectedIndex < DataManager.Instance.productiveTemplates.Count;
        string selectedTemplateID = isProductive ? 
            DataManager.Instance.productiveTemplates[selectedIndex].TemplateID : 
            DataManager.Instance.civicTemplates[selectedIndex - DataManager.Instance.productiveTemplates.Count].TemplateID;
        
        BuildingTemplate selectedTemplate = FindTemplateByID(selectedTemplateID);

        if (selectedTemplate is ProductiveTemplate)
        {
            AddProductiveBuilding(selectedTemplate as ProductiveTemplate);
        }
        else if (selectedTemplate is CivicTemplate)
        {
            //AddCivicBuilding(selectedTemplate as CivicTemplate);
        }
    } 

    private BuildingTemplate FindTemplateByID(string templateID)
    {
        // Intenta trobar el template en la llista de productiveTemplates
        BuildingTemplate template = DataManager.Instance.productiveTemplates.FirstOrDefault(t => t.TemplateID == templateID);

        // Si no es troba en productiveTemplates, busca en civicTemplates
        if (template == null)
        {
            template = DataManager.Instance.civicTemplates.FirstOrDefault(t => t.TemplateID == templateID);
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
            1, // Mida de l'edifici
            0, // HPCurrent
            0, // HPMaximum
            template.TemplateID,
            new List<ProductiveFactor>(), // CurrentFactors com a llistat buit per ara
            template.PossibleMethods,
            null, // Method Active, no s'està fabricant res encara. 
            template.DefaultMethod,
            null, // BatchCurrent com a null per ara
            new List<Batch>(), // BatchBacklog com a llistat buit per ara
            1.0f, // Linear
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
        Debug.Log($"Número de ProductionMethods disponibles: {newBuilding.MethodsAvailable.Count}");
    }

    /* private void AddCivicBuilding(CivicTemplate template)
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
            1, // Mida de l'edifici
            0, // HPCurrent
            0, // HPMaximum
            template.Function,
            template.JobsPoor,
            template.JobsMid,
            template.JobsRich
        );

        // Aquí es podria afegir el nou edifici a una llista d'edificis dins de la ciutat actual, per exemple
        AddBuildingToCurrentCity(newBuilding);
        Debug.Log("Nou edifici cívic creat: " + newBuilding.BuildingName);
    } */


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
            
            // Actualitza la graella d'edificis per la ciutat actual
            if (cityInterface != null)
            {
                cityInterface.UpdateBuildingGridForCity(currentCity);
            }
            else
            {
                Debug.LogError("cityInterface no està assignat!");
            }

        }
        else
        {
            Debug.LogError("No hi ha cap ciutat seleccionada per afegir l'edifici.");
        }
    }

    public void SetupFactors(ProductiveBuilding newBuilding, ProductiveTemplate template)
    {
        // Crear la llista de factors productius per a l'edifici si no existeix
        if (template.Factors == null)
        {
            Debug.Log($"No hi ha cap factor carregat per aquesta template");
        }

        // Agafar el CityInventory on està situat l'edifici
        /* CityInventory cityInventory = gameManager.CurrentCity.CityInventory;
        Debug.Log($"[SetupFactors] Obtenint inventari de la ciutat: {gameManager.CurrentCity.cityName}"); */

        //foreach (TemplateFactor templateFactor in template.Factors)
        foreach (string factorID in template.Factors) 
        {
            TemplateFactor templateFactor = DataManager.Instance.GetFactorById(factorID);
            if (templateFactor == null)
            {
                Debug.LogWarning($"No s'ha trobat cap factor amb ID {factorID} per l'edifici {newBuilding.BuildingID}");
                continue;
            }

            if (templateFactor is EmployeeFT employeeFT)
            {
                // Crear un nou EmployeePT i assignar-li les propietats
                EmployeePT newEmployeeFactor = new EmployeePT(
                    templateFactor,
                    newBuilding.ProductionTempID,
                    0, // EffectSize inicial és zero
                    0, // CurrentEmployees inicial és zero
                    0 * 10 // MonthlySalary, multiplica el nombre d'empleats per 10
                );

                // Afegir el nou factor a la llista de factors de l'edifici
                //newBuilding.AddFactor(newEmployeeFactor);
                newBuilding.CurrentFactors.Add(newEmployeeFactor);
                Debug.Log($"[SetupFactors] Afegit nou factor empleat: {newEmployeeFactor.FactorName} a l'edifici: {newBuilding.BuildingID}");
            }
            else if (templateFactor is ResourceFT resourceFT)
            {
                // Obté el recurs associat usant ResourceID
                Resource resource = DataManager.resourcemasterlist.FirstOrDefault(r => r.ResourceID == resourceFT.ResourceID);
                if (resource == null)
                {
                    Debug.LogWarning($"Recurs amb ID {resourceFT.ResourceID} no trobat.");
                    continue;
                }

                // Trobar la quantitat actual del recurs en el CityInventory
                /* float currentQuantity = cityInventory.InventoryResources
                    .FirstOrDefault(r => r.ResourceID == resourceFT.FResource.ResourceID)?.Quantity ?? 0f; */

                // Crear un nou ResourcePT i assignar-li les propietats
                ResourcePT newResourceFactor = new ResourcePT(
                    templateFactor,
                    newBuilding.ProductionTempID,
                    0, // EffectSize
                    resource, 
                    0, // Quantitat actual
                    0, // MonthlyConsumption
                    0  // MonthlyValue
                );

                // Afegir el nou factor a la llista de factors de l'edifici
                newBuilding.CurrentFactors.Add(newResourceFactor);
                Debug.Log($"[SetupFactors] Afegit nou factor recurs: {newResourceFactor.FactorName} amb quantitat actual: {newResourceFactor.CurrentQuantity} a l'edifici: {newBuilding.BuildingID}");
            }
        }

        Debug.Log($"[SetupFactors] Factors configurats per a l'edifici: {newBuilding.BuildingID} amb un total de {newBuilding.CurrentFactors.Count} factors.");
    }


}