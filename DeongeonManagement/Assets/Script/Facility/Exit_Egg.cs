using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit_Egg : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        InteractionOfTimes = 10000;
        Type = FacilityType.Portal;
        Name = "��й�";
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        Cor_Facility = StartCoroutine(FacilityEvent(npc, 1, "��й� - �߸��� �̺�Ʈ", ap: 0, mp: 0, hp: 0));
        return Cor_Facility;
    }

    public override Coroutine NPC_Interaction_Portal(NPC npc, out int floor)
    {
        Cor_Facility = StartCoroutine(FacilityEvent(npc, 1, "��й濡�� Ż����...", ap: 1, mp: 3, hp: 0));
        floor = 4;

        return Cor_Facility;
    }
}