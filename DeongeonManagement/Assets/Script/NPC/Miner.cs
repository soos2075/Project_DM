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


    protected override void Initialize_Status()
    {
        int index = Random.Range(0, 100);
        Name_Index = index;

        SetStatus("±¤ºÎ",
            lv: 1,
            atk: 5,
            def: 4,
            agi: 2,
            luk: 7,
            hp: 15,
            ap: 5,
            mp: 50,
            speed: 1.2f,
            delay: 1.2f);

        Init_AvoidType();
    }
    protected override void SetPriorityList()
    {
        if (PriorityList != null) PriorityList.Clear();


        var list1 = GetPriorityPick(typeof(Mineral_High));
        var list2 = GetPriorityPick(typeof(Mineral_Low));

        AddList(list1);
        AddList(list2);
    }
}
