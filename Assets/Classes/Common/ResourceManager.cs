using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public List<Resource> resources;

    public static List<Resource> AllResources = new List<Resource>();
    // blank, to be filled through the json 
    
    /* public static List<Resource> AllResources = new List<Resource>
    {   // ID, nom, current quantity, base price, current price, weight. Current values must be zero
        new Resource(0, "Fusta", 0, 10, 0, 10),
        new Resource(1, "Pedra", 0, 15, 0, 10),
        new Resource(2, "Ferro", 0, 30, 0, 10),
        new Resource(3, "Bronze", 0, 30, 0, 10),
        new Resource(4, "Or", 0, 30, 0, 10),
        new Resource(5, "Plata", 0, 30, 0, 10),
        new Resource(6, "Peix", 0, 30, 0, 10),
        new Resource(7, "Carn", 0, 30, 0, 10),
        // Afegeix tots els recursos aquí...
    }; */

    void Start() {
        LoadResources();
    }


    // Funció per llegir json
    public void LoadResources()
    {
        // Carrega el fitxer JSON. Nota: no incloguis l'extensió del fitxer
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("DDBB_Resource");  // ALL files in that folder
        if (jsonFiles.Length == 0)
        {
            Debug.Log("No s'han trobat fitxers JSON dins de DDBB_Resource.");
            return;
        }
        foreach (TextAsset jsonFile in jsonFiles)
        {
            Debug.Log($"Processant fitxer {jsonFile.name}...");
            // El següent codi processa un fitxer JSON tal com ho estaves fent
            ResourceListString resourceListString = JsonUtility.FromJson<ResourceListString>(jsonFile.text); 

        
            foreach (ResourceString resourceString in resourceListString.resource_jsonfile)
            {
                // Convertim les strings a números
                int id = int.Parse(resourceString.resourceID);
                int qty = string.IsNullOrEmpty(resourceString.resourceQty) ? 0 : int.Parse(resourceString.resourceQty);
                int bPrice = int.Parse(resourceString.basePrice);
                int cPrice = string.IsNullOrEmpty(resourceString.currentPrice) ? 0 : int.Parse(resourceString.currentPrice);
                float bWeight = float.Parse(resourceString.baseWeight);

                // Ara crea una nova instància de Resource amb les dades convertides
                Resource resource = new Resource(id, resourceString.resourceName, qty, bPrice, cPrice, bWeight);

                // Assigna la nova instància a la llista de AllResources
                AllResources.Add(resource);
                Debug.Log("Resource ID: " + id + ", Name: " + resource.resourceName + ", Quantity: " + qty + ", Price: " + cPrice);
            }
        }
    }

    
    // es cridarà sempre que fem servir inventaris, per donar des del ID a la resta de propietats
    public static Resource GetResourceById(int resourceID)
    {
        return AllResources.Find(resource => resource.resourceID == resourceID);
    }

    // efecte per actualitzar el preu promig de mercaderies
    public static void UpdateWeightedPrice(int resourceID, int qty, int price)
    {
        Resource resource = AllResources[resourceID];
        int newTotalQty = resource.resourceQty + qty;
        resource.currentPrice = ((resource.currentPrice * resource.resourceQty) + (price * qty)) / newTotalQty;
        resource.resourceQty = newTotalQty;
    }
}
