using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Vehicle
{
    public string ID;
    public string Name;

    private static int vehicleCount = 0;

    public Vehicle(string name)
    {
        this.ID = GenerateID();
        this.Name = name;
    }

    private string GenerateID()
    {
        return $"V{(++vehicleCount).ToString().PadLeft(5, '0')}";
    }
}