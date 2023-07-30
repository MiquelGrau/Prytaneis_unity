using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager<T> : MonoBehaviour where T : class, new() 
{
    private string dataPath;
    public T dataItems;

    private void Awake()
    {
        dataPath = Path.Combine(Application.persistentDataPath, "CityData.json");
        LoadData();
    }

   public void LoadData()
    {
        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            dataItems = JsonUtility.FromJson<T>(json);
            Debug.Log("Dades carregades: " + json);
        }
        else
        {
            dataItems = new T();
            Debug.LogError("No es pot trobar el fitxer JSON. Es crea una nova instància buida de dataItems.");
        }
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(dataItems, prettyPrint: true);
        File.WriteAllText(dataPath, json);
    }
}