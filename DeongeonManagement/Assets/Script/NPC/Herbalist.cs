using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herbalist : NPC
{
    [field: SerializeField]
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

        var list1 = GetPriorityPick(typeof(Herb_Low));
        var list2 = GetPriorityPick(typeof(Herb_High));

        AddList(list1);
        AddList(list2, AddPos.Front);

        var list5 = GetFacilityPick(Facility.FacilityType.NPCEvent);
        AddList(list5);

        PickToProbability(GetPriorityPick(typeof(Entrance_Egg)), (PlacementInfo.Place_Floor.FloorIndex + Rank) * 0.04f);
    }
}
