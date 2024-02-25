using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateFactor
{
    public string FactorID { get; private set; }
    public string FactorName { get; private set; }
    public string FactorType { get; private set; }
    public string FactorEffect { get; private set; }

    // Constructor
    public TemplateFactor(string id, string name, string type, string effect)
    {
        FactorID = id;
        FactorName = name;
        FactorType = type;
        FactorEffect = effect;
    }
}

// Classe derivada per al factor d'empleats
public class EmployeeFactor : TemplateFactor
{
    //public string FactorID { get; private set; }
    //public string FactorName { get; private set; }
    //public string FactorType { get; private set; }
    //public string FactorEffect { get; private set; }
    public int FactorSize { get; private set; }
    public string WorkerID { get; private set; }
    public string EmployeeStrata { get; private set; }
    public int EmployeeMin { get; private set; }
    public int EmployeeOptimal { get; private set; }
    public int EmployeeMax { get; private set; }
    public string FactorShortfallEffect { get; private set; }
    public int FactorShortfallSize { get; private set; }
    
    // Constructor
    public EmployeeFactor(string factorID, string factorName, string factorType, 
                string factorEffect, int factorSize, 
                string workerID, string employeeStrata, 
                int employeeMin, int employeeOptimal, int employeeMax, 
                string shortfallEffect, int shortfallSize)
        : base(factorID, factorName, factorType, factorEffect)
    {
        WorkerID = workerID;
        EmployeeStrata = employeeStrata;
        EmployeeMin = employeeMin;
        EmployeeOptimal = employeeOptimal;
        EmployeeMax = employeeMax;
        FactorShortfallEffect = shortfallEffect;
        FactorShortfallSize = shortfallSize;
        FactorSize = factorSize;
    }
}

// Classe derivada per al factor de combustible
public class ResourceFactor : TemplateFactor
{
    //public string FactorID { get; private set; }
    //public string FactorName { get; private set; }
    //public string FactorType { get; private set; }
    //public string FactorEffect { get; private set; }
    public int FactorSize { get; private set; }
    public Resource FactorResource { get; private set; }
    public float MonthlyConsumption { get; private set; }
    public int ResourceMin { get; private set; }
    public int ResourceOptimal { get; private set; }
    public int ResourceMax { get; private set; }
    public string FactorShortfallEffect { get; private set; }
    public int FactorShortfallSize { get; private set; }
    
    // Constructor
    public ResourceFactor(string factorID, string factorName, string factorType, 
                string factorEffect, int factorSize,
                Resource factorResource, float monthlyConsumption,
                int resourceMin, int resourceOptimal, int resourceMax,
                string shortfallEffect, int shortfallSize)
        : base(factorID, factorName, factorType, factorEffect)
    {
        FactorSize = factorSize;
        FactorResource = factorResource;
        MonthlyConsumption = monthlyConsumption;
        ResourceMin = resourceMin;
        ResourceOptimal = resourceOptimal;
        ResourceMax = resourceMax;
        FactorShortfallEffect = shortfallEffect;
        FactorShortfallSize = shortfallSize;
    }
}