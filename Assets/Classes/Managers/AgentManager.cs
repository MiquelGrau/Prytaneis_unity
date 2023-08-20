using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AgentManager : MonoBehaviour
{
    public List<Agent> agents;
    public GameObject agentItemPrefab; // Aquest serà el prefab que acabes de crear
    public Transform agentListParent;  // Aquest és el contenidor (parent) on vols que apareguin els ítems

    void Start() {
        LoadAgents();
    }

    public void LoadAgents()
    {
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("DDBB_Agents");
        if (jsonFiles.Length == 0)
        {
            Debug.Log("No s'han trobat fitxers JSON dins de DDBB_Agents.");
            return;
        }
        foreach (TextAsset jsonFile in jsonFiles)
        {
            Debug.Log($"Processant fitxer {jsonFile.name}...");

            AgentListString agentListString = JsonUtility.FromJson<AgentListString>(jsonFile.text);
            foreach (AgentString agentString in agentListString.agents)
            {
                Agent agent = new Agent
                {
                    agentID = int.Parse(agentString.agentID),
                    agentName = agentString.agentName,
                    currentCityID = int.Parse(agentString.currentCityID),
                    money = int.Parse(agentString.money),
                    inventoryID = int.Parse(agentString.inventoryID)
                };
                agents.Add(agent);
            }
        }
        Debug.Log("Nombre total d'agents carregats: " + agents.Count);

        foreach(Agent agent in agents)
        {
            Debug.Log($"Creant agent: {agent.agentName}");

            GameObject agentItemInstance = Instantiate(agentItemPrefab, agentListParent);
            TextMeshProUGUI agentNameText = agentItemInstance.transform.Find("AgentNameText").GetComponent<TextMeshProUGUI>();
            Button actionButton = agentItemInstance.transform.Find("ActionButton").GetComponent<Button>();
            
            agentNameText.text = agent.agentName;
            actionButton.onClick.AddListener(() => OnAgentActionButtonClicked(agent));
        }
    }

    public void OnAgentActionButtonClicked(Agent agent)
    {
        // Ací pots posar el codi que vols que s'execute quan es premi el botó d'un agent.
        Debug.Log($"El botó de l'agent {agent.agentName} ha estat premut.");
        SceneManager.LoadScene("RouteScene");
    }

    // Constructors varios
    public Agent GetAgentById(int id)
    {
        return agents.Find(agent => agent.agentID == id);
    }
    public List<Agent> GetAgents()
    {
        return agents;
    }
}