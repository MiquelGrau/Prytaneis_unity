using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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
    
    public GameObject agentPrefab;
    public Transform PInfoAgentList;
    
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

            TMP_Text outputText = newBuildingCell.transform.Find("BBasicOutput").GetComponent<TMP_Text>();
            if (building is CivicBuilding civic)
            {
                outputText.text = civic.Function;
            }
            else if (building is ProductiveBuilding productive && productive.MethodActive != null)
            {
                outputText.text = productive.MethodActive.MethodName;
            }
            else
            {
                outputText.text = "No additional info";
            }

            // Afegeix un event al clicar per mostrar detalls
            Button detailButton = newBuildingCell.transform.Find("BBasicButtonDetail").GetComponent<Button>();
            detailButton.onClick.AddListener(() => {
                Debug.Log($"Edifici {building.BuildingID} ha estat seleccionat, mostrant detalls");
                SetupBuildingUI(building);
            });
        }
    }

    // Funció per actualitzar la graella d'agents
    public void UpdateAgentGrid()
    {
        foreach (Transform child in PInfoAgentList)
        {
            Destroy(child.gameObject);
        }

        foreach (Agent agent in DataManager.Instance.agents)
        {
            GameObject newAgentCell = Instantiate(agentPrefab, PInfoAgentList);
            string agentName = agent.agentName;
            if (agent == GameManager.Instance.CurrentAgent)
            {
                agentName += " *";
            }
            newAgentCell.transform.Find("PInfoAgentDesc").GetComponent<TMP_Text>().text = agentName;
            newAgentCell.transform.Find("PInfoAgentMoney").GetComponent<TMP_Text>().text = agent.Inventory.InventoryMoney.ToString();
            newAgentCell.transform.Find("PInfoAgentWares").GetComponent<TMP_Text>().text = agent.Inventory.InventoryResources.Sum(res => res.Quantity).ToString();

            // Afegeix un event al clicar per mostrar detalls
            Button agentButton = newAgentCell.transform.Find("PInfoAgentButton").GetComponent<Button>();
            agentButton.onClick.AddListener(() => {
                Debug.Log($"Agent {agent.agentID} ha estat seleccionat, mostrant detalls");
                OnAgentSelected(agent);
            });
        }

        // Configura el layout
        VerticalLayoutGroup layoutGroup = PInfoAgentList.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 2f;
        }

        ContentSizeFitter contentFitter = PInfoAgentList.GetComponent<ContentSizeFitter>();
        if (contentFitter != null)
        {
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
    // Funció que s'executa quan un agent és seleccionat
    private void OnAgentSelected(Agent agent)
    {
        // Aquí pots afegir la lògica per mostrar els detalls de l'agent seleccionat
        Debug.Log($"Agent seleccionat: {agent.agentName}");
        GameManager.Instance.AssignCurrentAgent(agent.agentID);
        UpdateAgentGrid();
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
            prodPanel.transform.Find("BProdLinear").GetComponent<TMP_Text>().text = productive.LinearOutput.ToString("P2");
            prodPanel.transform.Find("BProdInput").GetComponent<TMP_Text>().text = productive.InputEfficiency.ToString("P2");
            prodPanel.transform.Find("BProdOutput").GetComponent<TMP_Text>().text = productive.OutputEfficiency.ToString("P2");
            prodPanel.transform.Find("BProdCycle").GetComponent<TMP_Text>().text = productive.CycleEfficiency.ToString("P2");
            prodPanel.transform.Find("BProdSalary").GetComponent<TMP_Text>().text = productive.SalaryEfficiency.ToString("P2");
            prodPanel.transform.Find("BProdJobsPoor").GetComponent<TMP_Text>().text = productive.JobsPoor.ToString();
            prodPanel.transform.Find("BProdJobsMid").GetComponent<TMP_Text>().text = productive.JobsMid.ToString();
            prodPanel.transform.Find("BProdJobsRich").GetComponent<TMP_Text>().text = productive.JobsRich.ToString();
            //prodPanel.transform.Find("BProdBatchCurrent").GetComponent<TMP_Text>().text = productive.MethodActive.MethodName.ToString();

            // Informació del batch actual
            if (productive.BatchCurrent != null)
            {
                string batchInfo = "Current Batch:\n";
                
                foreach (var input in productive.BatchCurrent.BatchInputs)
                {
                    batchInfo += $"Input: {input.InputResource.ResourceName}, Amt: {input.InputAmount}\n";
                }

                foreach (var output in productive.BatchCurrent.BatchOutputs)
                {
                    batchInfo += $"Output: {output.OutputResource.ResourceName}, Amt: {output.OutputAmount}\n";
                }
                batchInfo += $"Progress: {productive.BatchCurrent.CycleTimeProgress}/{productive.BatchCurrent.CycleTimeTotal}\n";

                prodPanel.transform.Find("BProdBatchCurrent").GetComponent<TMP_Text>().text = batchInfo;
            }
            else
            {
                prodPanel.transform.Find("BProdBatchCurrent").GetComponent<TMP_Text>().text = "No active production.";
            }

            // Configurar factors
            Transform factorsPanel = prodPanel.transform.Find("BProdFactorPanel");
            PopulateProductiveFactors(productive, factorsPanel);
            
            // Configurar el Dropdown
            TMP_Dropdown methodDropdown = prodPanel.transform.Find("BProdMethodsDD").GetComponent<TMP_Dropdown>();
            PopulateMethodDropdown(methodDropdown, productive);

            methodDropdown.onValueChanged.AddListener(delegate {
                OnMethodSelected(methodDropdown, productive);
            });
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

    private void PopulateMethodDropdown(TMP_Dropdown dropdown, ProductiveBuilding building)
    {
        dropdown.ClearOptions();
        List<string> methodoptions = new List<string>();
        foreach (var method in building.MethodsAvailable)
        {
            methodoptions.Add(method.MethodName);
        }
        dropdown.AddOptions(methodoptions);
    }

    private void OnMethodSelected(TMP_Dropdown dropdown, ProductiveBuilding building)
    {
        int index = dropdown.value;
        if (index >= 0 && index < building.MethodsAvailable.Count)
        {
            ProductionMethod selectedMethod = building.MethodsAvailable[index];
            ProductionManager.Instance.SetupNewBatch(selectedMethod, building);
        }
    }
    

}

