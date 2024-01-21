using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : NPC
{
    public override List<BasementTile> PriorityList { get; set; }

    protected override void SetPriorityList()
    {
        PriorityList = GetFloorObjectsAll();
    }
}
