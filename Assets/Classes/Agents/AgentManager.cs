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
    public static int SelectedAgentID { get; private set; } = -1;

    public void OnAgentActionButtonClicked(Agent agent)
    {
        GameData.Instance.SelectedAgent = agent;
        Debug.Log($"El botó de l'agent {agent.agentName} ha estat premut.");
        SceneManager.LoadScene("RouteScene");
    }

    // Constructors varios
    public Agent GetAgentById(string id)
    {
        return agents.Find(agent => agent.agentID == id);
    }
    public List<Agent> GetAgents()
    {
        return agents;
    }
}