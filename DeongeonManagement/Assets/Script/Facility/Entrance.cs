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
        Name = "입구";
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        Cor_Facility = StartCoroutine(FacilityEvent(npc, 1, 1, 5, "다음층으로 이동중..."));
        return Cor_Facility;
    }
}
