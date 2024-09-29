using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldSceneManager : MonoBehaviour
{
    public static WorldSceneManager Instance;

    // Actualitzem el delegat per utilitzar l'enum WorldSceneInteractionMode
    public delegate void ModeChangeAction(WorldSceneInteractionMode newMode);
    public event ModeChangeAction OnModeChange;
    private string routeSceneName = "RoutesScene";

    // Panell inferior, configuració d'agent
    public GameObject agentBasicPrefab; // Prefab "AgentInstanceBasic"
    public Transform agentsPoolGrid; // El grid dins del panel "AgentsPool"
    public TMP_Text aConfigName;
    public TMP_Text configOneData;

    private List<Agent> agents;


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
        // Obtenim la llista d'agents de DataManager
        agents = DataManager.Instance.GetAgents();

        // Poblarem el grid amb els agents
        PopulateAgentsPoolGrid();
    }

    // Canvis d'escena
    // -------------------------------------------------------------

    public void ChangeMode(WorldSceneInteractionMode newMode)
    {
        OnModeChange?.Invoke(newMode);
    }

    public void SetDefaultMode() {
        ChangeMode(WorldSceneInteractionMode.Default);
        UnloadRouteScene();
    }

    public void SetRouteMode() {
        ChangeMode(WorldSceneInteractionMode.Route);
        LoadRouteScene();
    }

    public void LoadRouteScene()
    {
        // Comprova si l'escena ja està carregada
        if (SceneManager.GetSceneByName(routeSceneName).isLoaded)
        {
            Debug.Log($"{routeSceneName} ja està carregada.");
            return;
        }

        // Carrega l'escena additivament
        SceneManager.LoadScene(routeSceneName, LoadSceneMode.Additive);
        Debug.Log($"Carregant {routeSceneName} additivament.");
    }

    // Opcional: Mètode per descarregar RouteScene
    public void UnloadRouteScene()
    {
        if (SceneManager.GetSceneByName(routeSceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(routeSceneName);
            Debug.Log($"Descarregant {routeSceneName}.");
        }
    }

    // Funcions del panell de sota, per configurar agents
    // --------------------------------------------------

    // Funció per crear instàncies del prefab i assignar valors
    private void PopulateAgentsPoolGrid()
    {
        foreach (var agent in agents)
        {
            // Instanciar el prefab al grid
            GameObject agentInstance = Instantiate(agentBasicPrefab, agentsPoolGrid);

            // Trobar el text del nom a dins del prefab i assignar-li el nom de l'agent
            TMP_Text nameText = agentInstance.transform.Find("ANameTxt").GetComponent<TMP_Text>();
            if (nameText != null)
            {
                nameText.text = agent.agentName; 
            }
            // Pendent: assignar la imatge de l'aegnt

            // Afegir el listener al botó dins del prefab
            Button agentSelectedButton = agentInstance.transform.Find("ABasicButton").GetComponent<Button>();
            
            if (agentSelectedButton != null)
            {
                string agentID = agent.agentID; 
                agentSelectedButton.onClick.AddListener(() => OnAgentSelected(agentID));
            }
        }
    }

    // Aquesta funció serà cridada quan es seleccioni un agent
    private void OnAgentSelected(string agentID)
    {
        GameManager.Instance.AssignCurrentAgent(agentID);
        ResetAgentConfigPanel();
    }

    // Actualitza valors del ribbon de sota, amb la info compactada
    private void ResetAgentConfigPanel()
    {
        Agent currConfigAgent = GameManager.Instance.currentAgent;

        if (currConfigAgent != null)
        {
            // Actualitzar el nom de l'agent seleccionat
            aConfigName.text = currConfigAgent.agentName;

            // Composar la informació de la capacitat i diners de l'agent
            string agentInfo = $"Capacitat {currConfigAgent.Inventory.CurrentCapacity} / " +
                               $"{currConfigAgent.Inventory.MaxCapacity}, " +
                               $"Diners: {currConfigAgent.Inventory.InventoryMoney}€";

            // Actualitzar el camp de text amb aquesta informació
            configOneData.text = agentInfo;
        }
    }


}
