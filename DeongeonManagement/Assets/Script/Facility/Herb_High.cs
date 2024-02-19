using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herb_High : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        Type = FacilityType.Herb;
        Name = "고급 약초";
        Detail_KR = "꽤 귀한 약초입니다. 고급 포션의 재료로 사용된다고 하네요.";
        Name_prefab = this.GetType().Name;

        if (InteractionOfTimes <= 0)
        {
            InteractionOfTimes = 2;
        }
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, 3, "약초 채집중...", ap: 1, mp: 15, hp: 0));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}의 이벤트 횟수없음");
            return null;
        }
    }
}
