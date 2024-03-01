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
        Detail_KR = "지상으로 향하는 출입구 입니다.";
        Name_prefab = this.GetType().Name;
    }
    public override void Init_FacilityEgo()
    {
        isOnlyOne = true;
        isClearable = true;
        if (PlacementInfo.Place_Floor.FloorIndex == 3)
        {
            isClearable = false;
        }
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        Cor_Facility = StartCoroutine(FloorExit(npc));
        return Cor_Facility;
    }


    IEnumerator FloorExit(NPC npc) //? 지하층으로 내려가는 입구에 도착했을 때 호출
    {
        if (npc.State == NPC.NPCState.Return_Empty)
        {
            yield return new WaitForSeconds(npc.ActionDelay);
            npc.FloorPrevious();
        }
        else if(npc.State == NPC.NPCState.Runaway || npc.State == NPC.NPCState.Return_Satisfaction)
        {
            yield return new WaitForSeconds(npc.ActionDelay);
            npc.FloorEscape();
        }
        else
        {
            npc.State = npc.StateRefresh();
        }
    }
}
