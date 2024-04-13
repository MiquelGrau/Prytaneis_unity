using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingInterfaces : MonoBehaviour
{
    // Prefabs
    public GameObject buildingInfoPrefab;
    public GameObject civicBuildingPrefab;
    public GameObject prodBuildingPrefab;
    public GameObject prodFactorPrefab;

    // Caratula UI per edificis, en general
    public TextMeshProUGUI buildingNameText;
    public TextMeshProUGUI buildingIDText;
    public TextMeshProUGUI buildingStatusText;
    public Image buildingIcon;

    // Caratula UI per CivicBuildings
    public TextMeshProUGUI civicFunctionText;

    // Caratula UI per ProductiveBuildings
    public Transform prodFactorsPanel;
    public Transform prodMethodsPanel;
    public TextMeshProUGUI batchRunInfoText;

    // Funció per aplicar les dades de base de l'edifici. Tot el que es quedarà fixe
    public void SetupBuildingUI(Building building)
    {
        // Set common building info
        buildingNameText.text = building.BuildingName;
        buildingIDText.text = building.BuildingID;
        buildingStatusText.text = building.ActivityStatus;

        // Canviarem el text de dins segons l'ús que es faci de l'edifici
        if(building is CivicBuilding civic)
        {
            civicFunctionText.text = civic.Function;
            
        }
        else if(building is ProductiveBuilding productive)
        {
            // Set productive specific info and populate methods
            // ...
            PopulateProductiveFactors(productive);
            //PopulateProductiveMethods(productive);
            batchRunInfoText.text = "Current Batch: " + productive.BatchCurrent; 
            // Instantiate or enable the productive building UI prefab
            
        }
    }

    private void PopulateProductiveFactors(ProductiveBuilding building)
    {
        // Clear existing factors
        foreach(Transform child in prodFactorsPanel)
        {
            Destroy(child.gameObject);
        }

        // Populate factors
        foreach(ProductiveFactor factor in building.CurrentFactors)
        {
            GameObject factorPrefab = Instantiate(prodFactorPrefab, prodFactorsPanel);
            // Set up the prefab with the factor's details
            factorPrefab.GetComponent<ProductiveFactorUI>().Setup(factor);
        }
    }

    private void PopulateProductiveMethods(ProductiveBuilding building)
    {
        
    }
}

// You would also need a class to control each ProductiveFactor's UI, something like:
public class ProductiveFactorUI : MonoBehaviour
{
    // UI references
    public Image factorIcon;
    public TextMeshProUGUI factorNameText;
    public Button increaseButton;
    public Button decreaseButton;

    public void Setup(ProductiveFactor factor)
    {
        // Set up the factor UI elements
        factorNameText.text = factor.FactorName;
        // Set the icon based on the factor type
        // Add listeners to the buttons to increase/decrease the CurrentEmployees or CurrentQuantity
        
    }
}
