using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : NPC
{
    public override List<BasementTile> PriorityList { get; set; }

    protected override void SetPriorityList()
    {
        PriorityList = GetFloorObjectsAll(Define.TileType.Facility);
        PriorityList = GetPriorityPick(typeof(Mineral_Low));
    }
}
