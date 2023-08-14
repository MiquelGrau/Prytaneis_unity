using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public List<Agent> agents;

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
    }


    // Agents
    public Agent GetAgentById(int id)
    {
        return agents.Find(agent => agent.agentID == id);
    }
    
}