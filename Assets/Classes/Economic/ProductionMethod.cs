using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionMethod
{
    public string MethodID { get; private set; }
    public string MethodName { get; private set; }
    public string MethodType { get; private set; }
    public int CycleTime { get; private set; }
    public List<MethodInput> Inputs { get; private set; }
    public List<MethodOutput> Outputs { get; private set; }

    // Definicions per a inputs i outputs
    public class MethodInput
    {
        public string ResourceID { get; set; }
        public float Amount { get; set; }
    }

    public class MethodOutput
    {
        public string ResourceID { get; set; }
        public float Amount { get; set; }
        public int Chance { get; set; }
        public string Type { get; set; }
    }

    // Constructor
    public ProductionMethod(string methodID, string methodName, string methodType, int cycleTime,
                            List<MethodInput> inputs, List<MethodOutput> outputs)
    {
        MethodID = methodID;
        MethodName = methodName;
        MethodType = methodType;
        CycleTime = cycleTime;
        Inputs = inputs ?? new List<MethodInput>();
        Outputs = outputs ?? new List<MethodOutput>();
    }

    // Afegir inputs i outputs manualment després de la creació de l'objecte si és necessari
    public void AddInput(string resourceID, float amount)
    {
        Inputs.Add(new MethodInput() { ResourceID = resourceID, Amount = amount });
    }

    public void AddOutput(string resourceID, float amount, int chance)
    {
        Outputs.Add(new MethodOutput() { ResourceID = resourceID, Amount = amount, Chance = chance });
    }
}
