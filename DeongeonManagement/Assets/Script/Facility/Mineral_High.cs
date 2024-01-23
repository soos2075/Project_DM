using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mineral_High : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes
    {
        get { return times; }
        set { times = value; TimesCheck(); }
    }

    private int times;

    public override void FacilityInit()
    {
        Type = FacilityType.Mineral;
        InteractionOfTimes = 3;
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        Cor_Facility = StartCoroutine(FacilityEvent(npc));

        return Cor_Facility;
    }


    Coroutine Cor_Facility;
    IEnumerator FacilityEvent(NPC npc)
    {
        Debug.Log("광물 채집 이벤트 진행");

        yield return new WaitForSeconds(3);

        Debug.Log("광물 채집 이벤트 종료");

        Debug.Log($"{npc.name} 의 AP : {npc.ActionPoint} - 1, {name} 의 횟수 : {InteractionOfTimes} - 1");
        npc.ActionPoint--;
        InteractionOfTimes--;

        Cor_Facility = null;
    }


    void TimesCheck()
    {
        if (InteractionOfTimes <= 0)
        {
            Managers.Placement.PlacementClear(this);
        }
    }
}
