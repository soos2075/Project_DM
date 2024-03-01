using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEgg : Facility
{
    public override FacilityEventType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        Type = FacilityEventType.NPC_Event;
        InteractionOfTimes = 10000;
        Name = "던전의 알";
        Detail_KR = "정체모를 수수께끼의 알입니다.";
        Name_prefab = this.GetType().Name;
    }
    public override void Init_FacilityEgo()
    {
        isOnlyOne = true;
        isClearable = false;
    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, 3, "알 조사중...", ap: 1, mp: 5, hp: 0));

            Managers.UI.ClearAndShowPopUp<UI_GameOver>();
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}의 이벤트 횟수없음");
            return null;
        }
    }
}
