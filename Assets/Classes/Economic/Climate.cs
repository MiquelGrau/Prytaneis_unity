using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Climate
{
    public string ClimateID { get; private set; }
    public string ClimateName { get; private set; }
    public List<Season> Seasons { get; private set; }

    public Climate(string climateID, string climateName, List<Season> seasons)
    {
        ClimateID = climateID;
        ClimateName = climateName;
        Seasons = seasons ?? new List<Season>();
    }
}

[System.Serializable]
public class Season
{
    public string SeasonName { get; private set; }
    public float AvgMaxTemp { get; private set; }
    public float AvgMinTemp { get; private set; }
    public float Precipitation { get; private set; }

    public Season(string seasonName, float avgMaxTemp, float avgMinTemp, float precipitation)
    {
        SeasonName = seasonName;
        AvgMaxTemp = avgMaxTemp;
        AvgMinTemp = avgMinTemp;
        Precipitation = precipitation;
    }
}
