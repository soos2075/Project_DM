using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        Type = FacilityType.RestZone;
        InteractionOfTimes = 2;
        Name = "���";
    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, 2, "ü�� ȸ����...", ap: 1, mp: 0, hp: -15));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}�� �̺�Ʈ Ƚ������");
            return null;
        }
    }
}
