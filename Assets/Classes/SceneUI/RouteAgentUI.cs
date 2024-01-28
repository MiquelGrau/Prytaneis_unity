using UnityEngine;
using TMPro; // Si vas a fer servir TextMeshPro per mostrar text

public class RouteAgentUI : MonoBehaviour
{
    public TMP_Text agentNameText;  // Canvia per Text si no utilitzes TextMeshPro
    public TMP_Text agentMoneyText; // Canvia per Text si no utilitzes TextMeshPro
    // Afegeix més camps si necessites mostrar més informació
    
    void Start()
    {
        UpdateAgentInfo();
    }

    public void UpdateAgentInfo()
    {
        Agent selectedAgent = GameData.Instance.SelectedAgent;
        if (selectedAgent != null)
        {
            agentNameText.text = selectedAgent.agentName;
            //agentMoneyText.text = "Diners: " + selectedAgent.money.ToString();
            // Actualitza més camps aquí segons necessitis
        }
    }
}
