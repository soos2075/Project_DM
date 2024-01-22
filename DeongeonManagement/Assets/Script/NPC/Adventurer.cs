using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : NPC
{
    public override List<BasementTile> PriorityList { get; set; }

    protected override void Initialize_Status()
    {
        SetStatus("¸ðÇè°¡",
            lv: 1,
            atk: 5,
            def: 5,
            hp: 20,
            ap: 5,
            mp: 20,
            speed: 2f,
            delay: 0.5f);
    }

    protected override void SetPriorityList()
    {
        if (PriorityList != null) PriorityList.Clear();

        PriorityList = GetFloorObjectsAll(Define.TileType.Monster);
    }
}
