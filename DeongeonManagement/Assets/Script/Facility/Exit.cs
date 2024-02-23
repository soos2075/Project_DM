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
        Name = "�ⱸ";
        Detail_KR = "���� ������ ������ ���Ա� �Դϴ�.";
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


    IEnumerator FloorExit(NPC npc) //? ���������� �������� �Ա��� �������� �� ȣ��
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
