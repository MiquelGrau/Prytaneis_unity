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
    
    private GameObject activeDetailPanel; // Mantenir una referència al panell de detalls actiu

    
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
            Button detailButton = newBuildingCell.transform.Find("BBasicButtonDetail").GetComponent<Button>();
            detailButton.onClick.AddListener(() => {
                Debug.Log($"Edifici {building.BuildingID} ha estat seleccionat, mostrant detalls");
                SetupBuildingUI(building);
            });
        }
    }



    // Funció per aplicar les dades de base de l'edifici. Tot el que es quedarà fixe
    public void SetupBuildingUI(Building building)
    {
        if (activeDetailPanel != null) Destroy(activeDetailPanel);  // Elimina possibles duplicats
        
        Transform canvasTransform = GameObject.Find("CanvasCityInfoView").transform;
        activeDetailPanel = Instantiate(detailPanelPrefab, canvasTransform);
        RectTransform rectTransform = activeDetailPanel.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(50, -50); // Configura les coordenades respecte a les ancoratges

        activeDetailPanel.transform.Find("BDetailID").GetComponent<TMP_Text>().text = building.BuildingID;
        activeDetailPanel.transform.Find("BDetailName").GetComponent<TMP_Text>().text = building.BuildingName;
        activeDetailPanel.transform.Find("BDetailTemplate").GetComponent<TMP_Text>().text = DataManager.Instance.GetTemplateNameByID(building.BuildingTemplateID);
        activeDetailPanel.transform.Find("BDetailOwner").GetComponent<TMP_Text>().text = building.BuildingOwnerID;

        // Botó de tancar el quadre
        Button closeButton = activeDetailPanel.transform.Find("BDetailCloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(() => Destroy(activeDetailPanel));

        // Troba el panel interior per als prefabs productiu i cívic
        Transform innerPanel = activeDetailPanel.transform.Find("BDetailInnerPanel");

        if (building is CivicBuilding civic)
        {
            GameObject civicPanel = Instantiate(civicBuildingPrefab, innerPanel);
            civicPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; 
            civicPanel.transform.Find("BCivicFunction").GetComponent<TMP_Text>().text = civic.Function;
        }
        else if (building is ProductiveBuilding productive)
        {
            //GameObject prodPanel = Instantiate(prodBuildingPrefab, activeDetailPanel.transform);
            GameObject prodPanel = Instantiate(prodBuildingPrefab, innerPanel);
            prodPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;  
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
            
            TMP_Text textComponent = factorPrefab.transform.Find("FactorName").GetComponent<TMP_Text>();
            if (textComponent != null)
            {
                textComponent.text = factor.FactorName;
            }
            else
            {
                Debug.LogError("FactorName TextMeshProUGUI component is missing or incorrectly named on the factor prefab!");
            }

        }
        
    }
}

