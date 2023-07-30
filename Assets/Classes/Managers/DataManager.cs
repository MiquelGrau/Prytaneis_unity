using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager<T> : MonoBehaviour where T : class
{
    private string dataPath;
    public List<T> dataItems;

    private void Awake()
    {
        dataPath = Path.Combine(Application.persistentDataPath, typeof(T).Name + ".json");
        LoadData();
    }

    public void LoadData()
    {
        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            dataItems = JsonUtility.FromJson<List<T>>(json);
        }
        else
        {
            dataItems = new List<T>();
        }
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(dataItems, prettyPrint: true);
        File.WriteAllText(dataPath, json);
    }
}
