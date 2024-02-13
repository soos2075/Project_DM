using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        Type = FacilityType.RestZone;
        InteractionOfTimes = 4;
        Name = "모닥불";
        Detail_KR = "모험가들이 쉬어가며 체력과 기력을 회복할 수 있습니다.";
        Name_prefab = this.GetType().Name;
    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, 5, "휴식중...", ap: 1, mp: -10, hp: -5));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}의 이벤트 횟수없음");
            return null;
        }
    }
}
