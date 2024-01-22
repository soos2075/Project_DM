using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : NPC
{
    public override List<BasementTile> PriorityList { get; set; }


    protected override void Initialize_Status()
    {
        SetStatus("±¤ºÎ",
            lv: 1,
            atk: 6,
            def: 1,
            hp: 15,
            ap: 7,
            mp: 10,
            speed: 1f,
            delay: 1f);
    }
    protected override void SetPriorityList()
    {
        if (PriorityList != null) PriorityList.Clear();

        PriorityList = GetFloorObjectsAll(Define.TileType.Facility);
        PriorityList = GetPriorityPick(typeof(Mineral_High));
    }
}
