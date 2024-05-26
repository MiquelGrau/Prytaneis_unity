using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineralResource
{
    public string MineralID { get; set; }
    public int SlotPosition { get; set; }
    public int TotalReserves { get; set; }
    public int[] DepthReserves { get; set; } = new int[4];
    public int[] DepthPurity { get; set; } = new int[4];

    public MineralResource(string mineralID, int slotPosition, int totalReserves,
                           int[] depthReserves, int[] depthPurity)
    {
        MineralID = mineralID;
        SlotPosition = slotPosition;
        TotalReserves = totalReserves;
        DepthReserves = depthReserves;
        DepthPurity = depthPurity;
    }
}
