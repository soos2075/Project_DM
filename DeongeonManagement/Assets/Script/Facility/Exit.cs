using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : Facility
{
    public override FacilityEventType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        InteractionOfTimes = 10000;
        Type = FacilityEventType.NPC_Event;
        Name = "출구";
        Detail_KR = "던전 밖으로 나가는 출입구 입니다.";
        Name_prefab = this.GetType().Name;
    }
    public override void SetFacilityBool()
    {
        isOnlyOne = true;
        isClearable = true;
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        Cor_Facility = StartCoroutine(FloorExit(npc));
        return Cor_Facility;
    }


    IEnumerator FloorExit(NPC npc) //? 지하층으로 내려가는 입구에 도착했을 때 호출
    {
        yield return new WaitForSeconds(npc.ActionDelay);
        yield return new WaitForSeconds(1);

        if (npc.State == NPC.NPCState.Return_Empty)
        {
            npc.FloorPrevious();
        }
        else if(npc.State == NPC.NPCState.Runaway || npc.State == NPC.NPCState.Return_Satisfaction)
        {
            npc.FloorEscape();
        }
    }
}
