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
        Name = "�ϱ� ����";
    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, 3, "���� ä����...", ap: 1, mp: 5, hp: 0));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}�� �̺�Ʈ Ƚ������");
            return null;
        }
    }





}
