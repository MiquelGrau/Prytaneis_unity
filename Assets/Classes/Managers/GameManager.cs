using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    /* public List<InventoryItem> startInventory1 = new List<InventoryItem>();
    public List<InventoryItem> startInventory2 = new List<InventoryItem>();
    public List<InventoryItem> startInventory3 = new List<InventoryItem>();
    public List<InventoryItem> startInventory4 = new List<InventoryItem>();
     */
    //public List<CityData> cities = new List<CityData>();
    //public List<Agent> agents = new List<Agent>();
    
    
    void Start()
    {
        
        
        // tot lo de sota és de la primera versió, ja no caldrà tenir-ho un cop tot funcioni. 
        
        // Definim els recursos inicials
        // Ara fa referencia a InventoryItems. Aquest constructor només vol ID, Current qty, current price
        /* startInventory1.Add(new InventoryItem(000, 5, 10));
        startInventory1.Add(new InventoryItem(001, 7, 15));

        startInventory2.Add(new InventoryItem(002, 2, 30));
        startInventory2.Add(new InventoryItem(003, 19, 40));

        startInventory3.Add(new InventoryItem(004, 4, 200));
        startInventory3.Add(new InventoryItem(005, 5, 80));

        startInventory4.Add(new InventoryItem(006, 8, 30));
        startInventory4.Add(new InventoryItem(007, 2, 50));
         */
        // Afegeix més recursos aquí...

        
        //LoadInventories();
        //LoadAgents();

        // Creem les ciutats
        //cities.Add(new CityData { cityID = 0, cityName = "Barcelona", longitude = 2.17f, latitude = 41.38f, cityInventory = new List<InventoryItem>(startInventory1), money = 100 });
        //cities.Add(new CityData { cityID = 1, cityName = "Roma", longitude = 12.48f, latitude = 41.89f, cityInventory = new List<InventoryItem>(startInventory2), money = 100 });

        /* // Creem els agents
        agents.Add(new Agent { agentID = 0, agentName = "Morosini", currentCityID = 0, agentInventory = new List<InventoryItem>(startInventory3), money = 50 });
        agents.Add(new Agent { agentID = 1, agentName = "Medici", currentCityID = 1, agentInventory = new List<InventoryItem>(startInventory4), money = 50 });
         */

        // Mostrar els inventaris de les ciutats
        /* foreach (CityData city in cities)
        {
            //Debug.Log("Ciutat: " + city.cityName);
            //Debug.Log("Diners: " + city.money);
            foreach (InventoryItem item in city.cityInventory)
            {
                Resource resource = ResourceManager.GetResourceById(item.resourceID);
                //Debug.Log("Recurs: " + resource.resourceName + ", Quantitat: " + item.quantity + ", Preu Base: " + resource.basePrice + ", Preu Actual: " + item.currentPrice);
            }
            
        } */

        // Mostrar els inventaris dels agents
        /* foreach (Agent agent in agents)
        {
            //Debug.Log("Agent: " + agent.agentName);
            //Debug.Log("Diners: " + agent.money);
            foreach (InventoryItem item in agent.agentInventory)
            {
                Resource resource = ResourceManager.GetResourceById(item.resourceID);
                //Debug.Log("Recurs: " + resource.resourceName + ", Quantitat: " + item.quantity + ", Preu Base: " + resource.basePrice + ", Preu Actual: " + item.currentPrice);
            }
            
        } */

    }
}
