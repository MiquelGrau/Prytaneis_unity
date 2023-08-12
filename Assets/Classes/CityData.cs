using System.Collections.Generic;

[System.Serializable]
public class CityData
{
    public string cityName;
    public float latitude;
    public float longitude;
    // Afegir qualsevol altre camp que necessitis
}

[System.Serializable]
public class CityDataList
{
    public List<CityData> cities;
}
