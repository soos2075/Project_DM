using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mineral_Low : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        Type = FacilityType.Mineral;
        InteractionOfTimes = 3;
        Name = "���� ����";
        Detail_KR = "���� ������ �����Դϴ�. �׷����� ������ �����ʹ� ������ �ٸ� �����̿���.";
        Name_prefab = this.GetType().Name;
    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, 2, "���� ä����...", ap: 1, mp: 3, hp: 0));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}�� �̺�Ʈ Ƚ������");
            return null;
        }
    }


}
