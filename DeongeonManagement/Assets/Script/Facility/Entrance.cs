using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        InteractionOfTimes = 100;
        Type = FacilityType.Exit;
        Name = "하행통로";
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        Debug.Log($"잘못된 이벤트 - {Name}");
        return null;
    }
}
