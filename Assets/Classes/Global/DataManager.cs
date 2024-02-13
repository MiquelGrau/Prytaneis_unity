using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
        // Aquesta és la classe per guardar DADES del joc. Tot el que es vagi fent, comerç, moviment, etc, es volcarà aquí. 


public class DataManager : MonoBehaviour
{
    private string dataPath;
    // Classes estatiques, definicions
    public static List<LifestyleTier> lifestyleTiers;
    public static List<Resource> resources;
    
    // Geografia
    public static List<WorldMapNode> worldMapNodes = new List<WorldMapNode>();
    public static List<WorldMapLandPath> worldMapLandPaths = new List<WorldMapLandPath>();

    // Classes de city
    public CityDataList dataItems;
    public List<CityInventory> cityInventories;
    
    // Classes de agents, merchants, etc
    public List<Agent> agents = new List<Agent>();
    public List<AgentInventory> agentInventories;

    

    private void Awake()
    {
        TextAsset cityDataAsset = Resources.Load<TextAsset>("CityData");
        if(cityDataAsset != null)
        {
            dataItems = JsonConvert.DeserializeObject<CityDataList>(cityDataAsset.text);
            Debug.Log("Dades carregades: " + cityDataAsset.text);

            if(dataItems == null)
            {
                Debug.LogError("La deserialització ha fallat. Es pot que el format JSON no coincideixi amb l'estructura de dades esperada.");
            }
            else
            {
                CityDataList dataList = dataItems as CityDataList;
            }
        }
        else
        {
            dataItems = new CityDataList();
            Debug.LogError("No es pot trobar el fitxer CityData.json a la carpeta Resources.");
        }

        
    }

    public void SaveData()
    {
        string json = JsonConvert.SerializeObject(dataItems, Formatting.Indented); // Use Formatting.Indented for pretty print
        File.WriteAllText(dataPath, json);
    }
    
    public List<CityData> GetCities()
    {
        if (dataItems is CityDataList cityDataList)
        {
            return cityDataList.cities;
        }
        else
        {
            Debug.LogError("DataManager no conté una instància de CityDataList.");
            return new List<CityData>();
        }
    }
    public List<Agent> GetAgents()
    {
        return agents;
    }
    /* public List<Agent> GetAgents()
    {
        if (agents is AgentList agentList)
        {
            return agentList.agents;
        }
        else
        {
            Debug.LogError("DataManager no conté una instància de Agents.");
            return new List<Agent>();
        }
    } */


}
