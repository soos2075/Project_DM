using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mineral_Low : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }


    public override void FacilityInit()
    {
        Type = FacilityType.Mineral;
        InteractionOfTimes = 2;
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{name}�� �̺�Ʈ Ƚ������");
            return null;
        }
    }


    Coroutine Cor_Facility;
    IEnumerator FacilityEvent(NPC npc)
    {
        Debug.Log($"{name} �̺�Ʈ ����");

        yield return new WaitForSeconds(1);


        Debug.Log($"{npc.name} �� AP : {npc.ActionPoint} - 1, {name} �� Ƚ�� : {InteractionOfTimes}");
        npc.ActionPoint--;
        TimesCheck();

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
