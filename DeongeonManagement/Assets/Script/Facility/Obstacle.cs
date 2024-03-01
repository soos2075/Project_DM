using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : Facility
{
    public override FacilityEventType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        Type = FacilityEventType.Non_Interaction;
        Name_prefab = name;
        InteractionOfTimes = 10000;

        Name = "";
        Detail_KR = "";
    }
    public override void Init_FacilityEgo()
    {
        isOnlyOne = false;
        isClearable = false;
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        throw new System.NotImplementedException();
    }


}
