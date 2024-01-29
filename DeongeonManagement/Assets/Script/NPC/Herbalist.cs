using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herbalist : NPC
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

        SetStatus("æ‡√ ≤€",
            lv: 1,
            atk: 3,
            def: 3,
            agi: 3,
            luk: 10,
            hp: 10,
            ap: 3,
            mp: 50,
            speed: 1.5f,
            delay: 1.0f);


        Init_AvoidType();
    }
    protected override void SetPriorityList()
    {
        if (PriorityList != null) PriorityList.Clear();

        var list1 = GetPriorityPick(typeof(Herb_Low));
        var list2 = GetPriorityPick(typeof(Herb_High));

        AddList(list1);
        AddList(list2, AddPos.Front);
    }
}
