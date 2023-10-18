using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public class DataManager<T> : MonoBehaviour where T : class, new() 
{
    private string dataPath;
    public T dataItems;

    private void Awake()
    {
        TextAsset cityDataAsset = Resources.Load<TextAsset>("CityData");
        if(cityDataAsset != null)
        {
            dataItems = JsonConvert.DeserializeObject<T>(cityDataAsset.text);
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
            dataItems = new T();
            Debug.LogError("No es pot trobar el fitxer CityData.json a la carpeta Resources.");
        }
    }

    public void SaveData()
    {
        string json = JsonConvert.SerializeObject(dataItems, Formatting.Indented); // Use Formatting.Indented for pretty print
        File.WriteAllText(dataPath, json);
    }
}
