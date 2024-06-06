using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : Facility
{

    public override void Init_Personal()
    {

    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, 2, UserData.Instance.LocaleText("Event_Resting"), ap: 1, mp: 0, hp: -15));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}의 이벤트 횟수없음");
            return null;
        }
    }
}
