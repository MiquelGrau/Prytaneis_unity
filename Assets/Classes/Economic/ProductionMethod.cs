using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionMethod
{
    public string MethodID { get; private set; }
    public string MethodName { get; private set; }
    public string MethodType { get; private set; }
    public int CycleTime { get; private set; }
    public int FirstChance { get; private set; }
    
    // Constructor
    public ProductionMethod(string methodID, string methodName, string methodType, int cycleTime, int firstChance)
    {
        MethodID = methodID;
        MethodName = methodName;
        MethodType = methodType;
        CycleTime = cycleTime;
        FirstChance = firstChance;
    }
}

// Classe per a la manufactura bàsica
public class BasicManufacturing : ProductionMethod
{
    //public string MethodID { get; private set; }
    //public string MethodName { get; private set; }
    //public string MethodType { get; private set; }
    //public int CycleTime { get; private set; }
    //public int FirstChance { get; private set; }
    public Resource InputResource { get; private set; }
    public float InputAmount { get; private set; }
    public Resource OutputResource { get; private set; }
    public float OutputAmount { get; private set; }

    // Constructor
    public BasicManufacturing(string methodID, string methodName, string methodtype, 
                            int firstChance, int cycleTime, 
                            Resource inputResource, float inputAmount, Resource outputResource, float outputAmount)
        : base(methodID, methodName, methodtype, firstChance, cycleTime)
    {
        InputResource = inputResource;
        InputAmount = inputAmount;
        OutputResource = outputResource;
        OutputAmount = outputAmount;
    }
}

// Classe per a la manufactura amb bonificació
public class BonusManufacturing : ProductionMethod
{
    //public string MethodID { get; private set; }
    //public string MethodName { get; private set; }
    //public int CycleTime { get; private set; }
    public Resource InputResource { get; private set; }
    public float InputAmount { get; private set; }
    //public int FirstChance { get; private set; }
    public Resource FirstOutputResource { get; private set; }
    public float FirstOutputAmount { get; private set; }
    public int SecondChance { get; private set; }
    public Resource SecondOutputResource { get; private set; }
    public float SecondOutputAmount { get; private set; }

    // Constructor
    public BonusManufacturing(string methodID, string methodName,  string methodtype, int cycleTime, 
                              Resource inputResource, float inputAmount, 
                              int firstChance, Resource firstOutputResource, float firstOutputAmount,
                              int secondChance, Resource secondOutputResource, float secondOutputAmount)
        : base(methodID, methodName, methodtype, firstChance, cycleTime)
    {
        InputResource = inputResource;
        InputAmount = inputAmount;
        FirstOutputResource = firstOutputResource;
        FirstOutputAmount = firstOutputAmount;
        SecondChance = secondChance;
        SecondOutputResource = secondOutputResource;
        SecondOutputAmount = secondOutputAmount;
    }
}




