using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : NPC
{
    public override List<BasementTile> PriorityList { get; set; }
    protected override Define.TileType[] AvoidTileType { get; set; }

    void Init_AvoidType()
    {
        AvoidTileType = new Define.TileType[] { Define.TileType.NPC, Define.TileType.Monster };
    }

    protected override void SetPriorityList()
    {
        Init_AvoidType();

        if (PriorityList != null) PriorityList.Clear();


        var list1 = GetPriorityPick(typeof(Mineral_High));
        var list2 = GetPriorityPick(typeof(Mineral_Low));

        AddList(list1);
        AddList(list2);


        {
            var list5 = GetFacilityPick(Facility.FacilityType.Event);
            AddList(list5);
        }
       

        PickToProbability(GetPriorityPick(typeof(Entrance_Egg)), (PlacementInfo.Place_Floor.FloorIndex + Rank) * 0.04f);
    }
}
