using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        InteractionOfTimes = 10000;
        Type = FacilityType.Exit;
        Name = "�ⱸ";
        Detail_KR = "���� ������ ������ ���Ա� �Դϴ�.";
        Name_prefab = this.GetType().Name;
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        Cor_Facility = StartCoroutine(FacilityEvent(npc, 1, "���������� �̵���...", ap: 0, mp: 0, hp: 0));
        return Cor_Facility;
    }

}
