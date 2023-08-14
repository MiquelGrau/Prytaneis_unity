using System.Collections.Generic;
using TMPro; 
using UnityEngine;
using UnityEngine.UI;

public class TradeviewUIManager : MonoBehaviour
{
    public TMP_Text barcelonaNameText;
    public TMP_Text barcelonaMoneyText;
    public TMP_Text barcelonaInventoryText;
    public TMP_Text barcelonaAgentsText;

    public TMP_Text romaNameText;
    public TMP_Text romaMoneyText;
    public TMP_Text romaInventoryText;
    public TMP_Text romaAgentsText;

    public Button Ag1toRoma; // Morosini es mou a Roma
    public Button Ag1toBarcelona; // Morosini es mou a Barcelona
    public Button Ag2toRoma; // Medici es mou a Roma
    public Button Ag2toBarcelona; // Medici es mou a Barcelona

    public Vector2 morosiniBarcelonaPosition;
    public Vector2 morosiniRomaPosition;
    public Vector2 mediciBarcelonaPosition;
    public Vector2 mediciRomaPosition;

    private GameManager gameManager;
    public TMP_Text allResourcesText;



    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        // Configura els botons amb les seves funcions corresponents
        Ag1toRoma.onClick.AddListener(MoveMorosiniToRoma);
        Ag1toBarcelona.onClick.AddListener(MoveMorosiniToBarcelona);
        Ag2toRoma.onClick.AddListener(MoveMediciToRoma);
        Ag2toBarcelona.onClick.AddListener(MoveMediciToBarcelona);

        // Defineix les posicions
        morosiniBarcelonaPosition = new Vector2(50, 150);
        morosiniRomaPosition = new Vector2(50, 150);
        mediciBarcelonaPosition = new Vector2(150, 150);
        mediciRomaPosition = new Vector2(150, 150);
    }
    
    private void Update()
    {
        // Actualitzem el text amb la informació de tots els recursos
        allResourcesText.text = AllResourcesToString();
        
        // Ubicacions dels agents, i condicional a la ciutat, per moure els botons
        Agent morosini = gameManager.agents.Find(agent => agent.agentName == "Morosini");
        Agent medici = gameManager.agents.Find(agent => agent.agentName == "Medici");

        if (morosini.currentCityID == gameManager.GetCityByName("Barcelona").cityID)
        {
            Ag1toRoma.GetComponent<RectTransform>().anchoredPosition = morosiniBarcelonaPosition;
            Ag1toBarcelona.GetComponent<RectTransform>().anchoredPosition = new Vector2(morosiniBarcelonaPosition.x, morosiniBarcelonaPosition.y - 30);
        }
        else
        {
            Ag1toRoma.GetComponent<RectTransform>().anchoredPosition = morosiniRomaPosition;
            Ag1toBarcelona.GetComponent<RectTransform>().anchoredPosition = new Vector2(morosiniRomaPosition.x, morosiniRomaPosition.y - 30);
        }

        if (medici.currentCityID == gameManager.GetCityByName("Barcelona").cityID)
        {
            Ag2toRoma.GetComponent<RectTransform>().anchoredPosition = mediciBarcelonaPosition;
            Ag2toBarcelona.GetComponent<RectTransform>().anchoredPosition = new Vector2(mediciBarcelonaPosition.x, mediciBarcelonaPosition.y - 30);
        }
        else
        {
            Ag2toRoma.GetComponent<RectTransform>().anchoredPosition = mediciRomaPosition;
            Ag2toBarcelona.GetComponent<RectTransform>().anchoredPosition = new Vector2(mediciRomaPosition.x, mediciRomaPosition.y - 30);
        }

        // Textos de les ciutats
        CityData barcelona = gameManager.cities.Find(city => city.cityName == "Barcelona");
        CityData roma = gameManager.cities.Find(city => city.cityName == "Roma");

        barcelonaNameText.text = barcelona.cityName;
        barcelonaMoneyText.text = "Money: " + barcelona.money.ToString();
        barcelonaInventoryText.text = "Inventory: \n" + InventoryToString(barcelona.cityInventory);
        barcelonaAgentsText.text = "Agents: \n" + AgentsToString(barcelona);

        romaNameText.text = roma.cityName;
        romaMoneyText.text = "Money: " + roma.money.ToString() + "€\n";
        romaInventoryText.text = " Inventory: \n" + InventoryToString(roma.cityInventory);
        romaAgentsText.text = "Agents: \n" + AgentsToString(roma);
    }

    private string AllResourcesToString()
    {
        string result = "";
        // Suposant que ResourceManager.AllResources retorna una llista o col·lecció de recursos
        foreach(var resource in ResourceManager.AllResources)
        {
            result += resource.ToString() + "\n"; // Aquí suposo que cada recurs té una funció ToString() que retorna la seva descripció
        }
        return result;
    }

    private string InventoryToString(List<InventoryItem> inventory)
    {
        string result = "";
        foreach (InventoryItem item in inventory)
        {
            Resource resource = ResourceManager.GetResourceById(item.resourceID);
            result += "  " + resource.resourceName + ": " + item.quantity + "; " + item.currentPrice + "€ \n";
        }
        return result;
    }

    
    private string AgentsToString(CityData city) // Modificada per rebre CityData
    {
        string result = "";
        foreach (Agent agent in gameManager.agents)
        {
            if (agent.currentCityID == city.cityID)
            {
                result += agent.agentName + ": " + agent.money + "€\n Inventory: \n" + InventoryToString(agent.agentInventory) + "\n";
            }
        }
        return result;
    }

    public void MoveMorosiniToRoma()
    {
        MoveAgent(gameManager.agents.Find(agent => agent.agentName == "Morosini"), gameManager.cities.Find(city => city.cityName == "Roma"));
    }

    public void MoveMorosiniToBarcelona()
    {
        MoveAgent(gameManager.agents.Find(agent => agent.agentName == "Morosini"), gameManager.cities.Find(city => city.cityName == "Barcelona"));
    }

    public void MoveMediciToRoma()
    {
        MoveAgent(gameManager.agents.Find(agent => agent.agentName == "Medici"), gameManager.cities.Find(city => city.cityName == "Roma"));
    }

    public void MoveMediciToBarcelona()
    {
        MoveAgent(gameManager.agents.Find(agent => agent.agentName == "Medici"), gameManager.cities.Find(city => city.cityName == "Barcelona"));
    }

    private void MoveAgent(Agent agent, CityData targetCity)
    {
        if(agent.currentCityID == targetCity.cityID)
        {
            Debug.Log(agent.agentName + " ja és a " + targetCity.cityName);
            return;
        }

        Debug.Log(agent.agentName + " es mou a " + targetCity.cityName);
        agent.currentCityID = targetCity.cityID;
    }

}
