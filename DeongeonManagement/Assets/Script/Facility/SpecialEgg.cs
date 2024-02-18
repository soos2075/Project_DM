using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEgg : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        Type = FacilityType.NPCEvent;
        InteractionOfTimes = 1000;
        Name = "������ ��";
        Detail_KR = "��ü�� ���������� ���Դϴ�. ��࿡ ���� ���� ���Ѿ߸� �մϴ�.";
        Name_prefab = this.GetType().Name;
    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, 3, "�� ������...", ap: 1, mp: 5, hp: 0));

            Managers.UI.ClearAndShowPopUp<UI_GameOver>();
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}�� �̺�Ʈ Ƚ������");
            return null;
        }
    }
}
