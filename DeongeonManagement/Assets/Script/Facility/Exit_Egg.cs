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
        Name = "비밀문";
        Name_prefab = this.GetType().Name;
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        Cor_Facility = StartCoroutine(FacilityEvent(npc, 1, "비밀문 - 잘못된 이벤트", ap: 0, mp: 0, hp: 0));
        return Cor_Facility;
    }

    public override Coroutine NPC_Interaction_Portal(NPC npc, out int floor)
    {
        Cor_Facility = StartCoroutine(FacilityEvent(npc, 1, "비밀방에서 탈출중...", ap: 1, mp: 3, hp: 0));
        floor = 4;

        return Cor_Facility;
    }
}
