using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingInterfaces : MonoBehaviour
{
    // Prefabs
    public GameObject buildingSimplePrefab;
    public Transform buildingGridPanel; 
    
    public GameObject detailPanelPrefab; 
    public GameObject civicBuildingPrefab;
    public GameObject prodBuildingPrefab;
    public GameObject prodFactorPrefab;
    
    
    // Funció per actualitzar la graella d'edificis per a una ciutat seleccionada
    public void UpdateBuildingGridForCity(CityData currentCity)
    {
        foreach (Transform child in buildingGridPanel)
        {
            Destroy(child.gameObject); 
        }

        foreach (Building building in currentCity.CityBuildings)
        {
            GameObject newBuildingCell = Instantiate(buildingSimplePrefab, buildingGridPanel);
            newBuildingCell.transform.Find("BBasicID").GetComponent<TMP_Text>().text = building.BuildingID;
            newBuildingCell.transform.Find("BBasicName").GetComponent<TMP_Text>().text = building.BuildingName;
            newBuildingCell.transform.Find("BBasicTemplate").GetComponent<TMP_Text>().text = DataManager.Instance.GetTemplateNameByID(building.BuildingTemplateID);

            // Afegeix un event al clicar per mostrar detalls
            //newBuildingCell.GetComponent<Button>().onClick.AddListener(() => SetupBuildingUI(building));
        }
    }



    // Funció per aplicar les dades de base de l'edifici. Tot el que es quedarà fixe
    public void SetupBuildingUI(Building building)
    {
        GameObject detailPanel = Instantiate(detailPanelPrefab, transform);
        detailPanel.GetComponent<RectTransform>().localPosition = new Vector3(40, -150, 0);  

        detailPanel.transform.Find("BDetailID").GetComponent<TMP_Text>().text = building.BuildingID;
        detailPanel.transform.Find("BDetailName").GetComponent<TMP_Text>().text = building.BuildingName;
        detailPanel.transform.Find("BDetailTemplate").GetComponent<TMP_Text>().text = DataManager.Instance.GetTemplateNameByID(building.BuildingTemplateID);
        detailPanel.transform.Find("BDetailOwner").GetComponent<TMP_Text>().text = building.BuildingOwnerID;

        if (building is CivicBuilding civic)
        {
            GameObject civicPanel = Instantiate(civicBuildingPrefab, detailPanel.transform);
            civicPanel.transform.Find("BCivicFunction").GetComponent<TMP_Text>().text = civic.Function;
        }
        else if (building is ProductiveBuilding productive)
        {
            GameObject prodPanel = Instantiate(prodBuildingPrefab, detailPanel.transform);
            prodPanel.transform.Find("BProdInput").GetComponent<TMP_Text>().text = productive.InputEfficiency.ToString("P2");
            prodPanel.transform.Find("BProdOutput").GetComponent<TMP_Text>().text = productive.OutputEfficiency.ToString("P2");
            prodPanel.transform.Find("BProdCycle").GetComponent<TMP_Text>().text = productive.CycleEfficiency.ToString("P2");
            prodPanel.transform.Find("BProdSalary").GetComponent<TMP_Text>().text = productive.SalaryEfficiency.ToString("P2");
            prodPanel.transform.Find("BProdJobsPoor").GetComponent<TMP_Text>().text = productive.JobsPoor.ToString();
            prodPanel.transform.Find("BProdJobsMid").GetComponent<TMP_Text>().text = productive.JobsMid.ToString();
            prodPanel.transform.Find("BProdJobsRich").GetComponent<TMP_Text>().text = productive.JobsRich.ToString();
            prodPanel.transform.Find("BProdBatchCurrent").GetComponent<TMP_Text>().text = productive.BatchCurrent;

            Transform factorsPanel = prodPanel.transform.Find("BProdFactorPanel");
            PopulateProductiveFactors(productive, factorsPanel);
        }
    }

    private void PopulateProductiveFactors(ProductiveBuilding building, Transform factorsPanel)
    {
        foreach(Transform child in factorsPanel)
        {
            Destroy(child.gameObject);
        }

        foreach(ProductiveFactor factor in building.CurrentFactors)
        {
            GameObject factorPrefab = Instantiate(prodFactorPrefab, factorsPanel);
            factorPrefab.GetComponent<ProductiveFactorUI>().SetupFactorUI(factor);
        }
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

    public void SetupFactorUI(ProductiveFactor factor)
    {
        factorNameText.text = factor.FactorName;
        // Configura altres elements UI aquí
        increaseButton.onClick.AddListener(() => IncreaseFactor(factor));
        decreaseButton.onClick.AddListener(() => DecreaseFactor(factor));
    }

    void IncreaseFactor(ProductiveFactor factor)
    {
        // Incrementa factor logic aquí
    }

    void DecreaseFactor(ProductiveFactor factor)
    {
        // Decrementa factor logic aquí
    }
}
