using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : NPC
{
    public override List<BasementTile> PriorityList { get; set; }
    protected override Define.TileType[] AvoidTileType { get; set; }

    void Init_AvoidType()
    {
        AvoidTileType = new Define.TileType[] { Define.TileType.NPC, Define.TileType.Facility };
    }

    protected override void Initialize_Status()
    {
        int index = Random.Range(0, 100);
        Name_Index = index;

        SetStatus("¸ðÇè°¡",
            lv: 1,
            atk: 8,
            def: 6,
            agi: 5,
            luk: 5,
            hp: 20,
            ap: 10,
            mp: 100,
            speed: 2f,
            delay: 0.8f);

        Init_AvoidType();
    }




    protected override void SetPriorityList()
    {
        if (PriorityList != null) PriorityList.Clear();

        PriorityList = GetFloorObjectsAll(Define.TileType.Monster);
    }
}
