using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herbalist : NPC
{
    public override List<BasementTile> PriorityList { get; set; }

    protected override void SetPriorityList()
    {
        PriorityList = GetPriorityPick(typeof(Herb_Low));
        PriorityRemove(Place_Tile);

        if (PriorityList.Count > 0)
        {
            MoveToTargetTile(PriorityList[0]);
        }
    }
}
