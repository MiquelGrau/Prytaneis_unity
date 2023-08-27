using System.Collections.Generic;
using UnityEngine;

public class DemandManager : MonoBehaviour
{
    private Dictionary<int, LifestyleTier> allLifestyleTiers;  // Assumint que tens aquest diccionari amb totes les dades
    
    void Start() 
    {
        allLifestyleTiers = new Dictionary<int, LifestyleTier>();
        LoadLifestyleTiersFromJSON();
    }

    public void LoadLifestyleTiersFromJSON()
    {
        Debug.Log("Començant la càrrega del fitxer JSON...");
        
        TextAsset jsonText = Resources.Load<TextAsset>("DDBB_Resource/lifestyle_definitions");
        if (jsonText == null)
        {
            Debug.LogError("No s'ha pogut carregar el fitxer JSON. Comprova el camí o el nom del fitxer.");
            return;
        }
        Debug.Log("Fitxer JSON carregat. Iniciant la deserialització...");

        
        LifestyleDataWrapper lifestyleDataWrapper;
        try
        {
            lifestyleDataWrapper = JsonUtility.FromJson<LifestyleDataWrapper>(jsonText.text);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error en la deserialització del JSON: {e.Message}");
            return;
        }

        Debug.Log("Deserialització completada. Començant a carregar les dades al diccionari...");

        foreach (LifestyleTierJSON tierJSON in lifestyleDataWrapper.lifestyleData_jsonfile)
        {
            LifestyleTier tier = new LifestyleTier(tierJSON.lifestyleTierID);
            tier.LifestyleDemands = tierJSON.data;
            allLifestyleTiers[tier.LifestyleTierID] = tier;
        }

        Debug.Log($"Càrrega completada: {allLifestyleTiers.Count} LifestyleTiers carregats.");
    }
    
    public LifestyleTier GetLifestyleTierByID(int id)
    {
        if(allLifestyleTiers.TryGetValue(id, out LifestyleTier tier))
        {
            return tier;
        }
        else
        {
            Debug.LogError($"No es troba cap LifestyleTier amb ID {id}.");
            return null;
        }
    }
    
    

    public List<ResourceDemand> CalculateCityDemands(CityData city)
    {
        List<ResourceDemand> cityDemands = new List<ResourceDemand>();

        // Per a cada població, obtenir les demandes de la seva LifestyleID i multiplicar per la seva quantitat
        cityDemands.AddRange(CalculateDemandsForPopulation(city.PoorPopulation, allLifestyleTiers[city.poorLifestyleID]));
        cityDemands.AddRange(CalculateDemandsForPopulation(city.MidPopulation, allLifestyleTiers[city.midLifestyleID]));
        cityDemands.AddRange(CalculateDemandsForPopulation(city.RichPopulation, allLifestyleTiers[city.richLifestyleID]));

        return cityDemands;
    }

    private List<ResourceDemand> CalculateDemandsForPopulation(int population, LifestyleTier lifestyle)
    {
        List<ResourceDemand> demandsForThisLifestyle = new List<ResourceDemand>();

        foreach (var ld in lifestyle.LifestyleDemands)
        {
            int totalDemandQuantity = population * ld.demandPerPopulation;
            demandsForThisLifestyle.Add(new ResourceDemand(ld.resourceType, totalDemandQuantity, ld.variety));
        }

        return demandsForThisLifestyle;
    }
}


