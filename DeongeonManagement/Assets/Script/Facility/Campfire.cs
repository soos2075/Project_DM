using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : Facility
{
    public override void Init_Personal()
    {
        EventType = FacilityEventType.NPC_Interaction;
        Name = "��ں�";
        Detail_KR = "���谡���� ����� ü�°� ����� ȸ���� �� �ֽ��ϴ�.";

        if (InteractionOfTimes <= 0)
        {
            InteractionOfTimes = 4;
        }
    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, 5, UserData.Instance.GetLocaleText("Event_Resting"), ap: 1, mp: -10, hp: -5));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}�� �̺�Ʈ Ƚ������");
            return null;
        }
    }
}
