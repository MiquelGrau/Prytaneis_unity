using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    public string ID;
    public string Name;

    private static int characterCount = 0;

    public Character(string name)
    {
        this.ID = GenerateID();
        this.Name = name;
    }

    private string GenerateID()
    {
        return $"C{(++characterCount).ToString().PadLeft(5, '0')}";
    }
}

