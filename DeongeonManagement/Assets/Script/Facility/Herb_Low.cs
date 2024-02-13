using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herb_Low : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        Type = FacilityType.Herb;
        InteractionOfTimes = 1;
        Name = "하급 약초";
        Detail_KR = "흔한 약초입니다. 그렇지만 던전에서만 얻을 수 있어 인기는 많습니다.";
        Name_prefab = this.GetType().Name;
    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, 3, "약초 채집중...", ap: 1, mp: 7, hp: 0));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}의 이벤트 횟수없음");
            return null;
        }
    }





}
