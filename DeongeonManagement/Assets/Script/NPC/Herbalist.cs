using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herbalist : NPC
{
    public override List<BasementTile> PriorityList { get; set; }

    protected override void Initialize_Status()
    {
        SetStatus("æ‡√ ≤€",
            lv: 1,
            atk: 3,
            def: 3,
            hp: 15,
            ap: 2,
            mp: 10,
            speed: 1.5f,
            delay: 0.8f);
    }
    protected override void SetPriorityList()
    {
        if (PriorityList != null) PriorityList.Clear();
        PriorityList = GetPriorityPick(typeof(Herb_Low));
    }
}
