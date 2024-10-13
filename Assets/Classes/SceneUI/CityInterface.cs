using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CityInterface : MonoBehaviour
{
    // Prefabs
    public GameObject buildingSimplePrefab;
    public Transform buildingGridPanel; 
    
    public GameObject detailPanelPrefab; 
    public GameObject civicBuildingPrefab;
    public GameObject prodBuildingPrefab;
    public GameObject prodFactorPrefab;
    
    private GameObject activeDetailPanel; 
    
    public GameObject agentPrefab;
    public Transform PInfoAgentList;

    // Mostrar el panell de la interfície de comerç
    public GameObject tradeInterfaceMaster;  
    public TradeInterface tradeInterface;    
    public TradeManager tradeManager;    
    
    public Button PInfoToMarketButton;
    public TMP_Text PInfoToMarketTxt;
    private bool isMarketOpen = false;


    private void Start()    // Start per les interaccions que hi ha només per l'escena
    {
        // Comerç
        InitializeTradeInterface();
        PInfoToMarketButton.onClick.AddListener(ShowTradePanel);
    }
    
    // DISPLAY PRINCIPAL
    // Funció per actualitzar la graella d'edificis per a una ciutat seleccionada
    public void UpdateBuildingGridForCity()
    {
        Location currentLocation = GameManager.Instance.currentLocation;
        
        foreach (Transform child in buildingGridPanel)
        {
            Destroy(child.gameObject); 
        }

        foreach (Building building in currentLocation.Buildings)
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
                ProductionMethod method = DataManager.Instance.GetProductionMethodByID(productive.MethodActive);
                if (method != null)
                {
                    outputText.text = method.MethodName;
                }
                else
                {
                    outputText.text = "Method not found";
                }
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

    // DISPLAY DRET, info del jugador

    // Funció per actualitzar la graella d'agents
    public void UpdateAgentGrid()
    {
        foreach (Transform child in PInfoAgentList)
        {
            Destroy(child.gameObject);
        }

        foreach (Agent agent in DataManager.Instance.allAgentsList)
        {
            GameObject newAgentCell = Instantiate(agentPrefab, PInfoAgentList);
            AgentInventory agentInv = DataManager.Instance.GetAgInvByID(agent.AgentInventoryID);
            string agentName = agent.agentName;
            if (agent == GameManager.Instance.currentAgent)
            {
                agentName += " *";
            }
            newAgentCell.transform.Find("PInfoAgentDesc").GetComponent<TMP_Text>().text = agentName;
            newAgentCell.transform.Find("PInfoAgentMoney").GetComponent<TMP_Text>().text = agentInv.InventoryMoney.ToString();
            newAgentCell.transform.Find("PInfoAgentWares").GetComponent<TMP_Text>().text = agentInv.InventoryResources.Sum(res => res.Quantity).ToString();

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
    
    // Display de la Interficie de Comerç
    private void InitializeTradeInterface()
    {
        if (tradeInterfaceMaster != null && tradeInterface != null)
        {
            Debug.Log("Component TradeInterface trobat correctament.");
            tradeInterface.tradeManager = FindObjectOfType<TradeManager>();
            if (tradeInterface.tradeManager != null)
            {
                tradeInterface.tradeManager.tradeInterface = tradeInterface;
            }
            else
            {
                Debug.LogError("No s'ha trobat el component TradeManager a l'escena.");
            }
            tradeInterfaceMaster.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -410, -10);
            tradeInterfaceMaster.SetActive(false); // Amaga la interfície de comerç inicialment
        
        }
        else
        {
            Debug.LogError("Falta assignar al Inspector o el TradeInterfaceMaster o el script de TradeInterface.");
        }
        
    }

    private void ShowTradePanel() // El botó per mostrar o tancar el mercat
    {
        if (!isMarketOpen)
        {
            // Mostrar el panell de comerç
            tradeInterfaceMaster.SetActive(true);
            tradeInterface.UpdateTradeInterface();

            // Canviar el text del botó
            PInfoToMarketTxt.text = "Leave Market";
            isMarketOpen = true;
        }
        else
        {
            // Amagar el panell de comerç
            tradeInterfaceMaster.SetActive(false);
            tradeManager.TradeDeskCleanup();

            // Canviar el text del botó
            PInfoToMarketTxt.text = "Go to Market";
            isMarketOpen = false;
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

            // Mostrar el MethodActive utilitzant el seu ID
            if (!string.IsNullOrEmpty(productive.MethodActive))
            {
                var activeMethod = DataManager.Instance.GetProductionMethodByID(productive.MethodActive);
                if (activeMethod != null)
                {
                    prodPanel.transform.Find("BProdMethodActive").GetComponent<TMP_Text>().text = activeMethod.MethodName;
                }
                else
                {
                    prodPanel.transform.Find("BProdMethodActive").GetComponent<TMP_Text>().text = "No active method";
                }
            }
            else
            {
                prodPanel.transform.Find("BProdMethodActive").GetComponent<TMP_Text>().text = "No active method";
            }
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
        
        foreach (var methodID in building.MethodsAvailable)
        {
            var method = DataManager.Instance.GetProductionMethodByID(methodID);
            if (method != null)
            {
                methodoptions.Add(method.MethodName);
            }
        }
        dropdown.AddOptions(methodoptions);
    }

    private void OnMethodSelected(TMP_Dropdown dropdown, ProductiveBuilding building)
    {
        int index = dropdown.value;
        if (index >= 0 && index < building.MethodsAvailable.Count)
        {
            string selectedMethodID = building.MethodsAvailable[index];  
            ProductionManager.Instance.SetupNewBatch(selectedMethodID, building);  
        }
    }
    

}

