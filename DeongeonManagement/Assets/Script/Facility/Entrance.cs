using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : Facility
{
    public override FacilityEventType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        InteractionOfTimes = 10000;
        Type = FacilityEventType.NPC_Event;
        Name = "�Ա�";
        Detail_KR = "������ �� ���������� �̾����� ���Ա��Դϴ�.";
        Name_prefab = this.GetType().Name;
    }
    public override void SetFacilityBool()
    {
        isOnlyOne = true;
        isClearable = true;
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        Cor_Facility = StartCoroutine(FloorNext(npc));
        return Cor_Facility;
    }


    IEnumerator FloorNext(NPC npc) //? ���������� �������� �Ա��� �������� �� ȣ��
    {
        yield return new WaitForSeconds(npc.ActionDelay);

        if (npc.State == NPC.NPCState.Next)
        {
            yield return new WaitForSeconds(0.5f);
            Main.Instance.CurrentDay.AddMana(3);
            Main.Instance.ShowDM(3, Main.TextType.mana, transform);

            npc.FloorNext();
        }
        else
        {
            npc.State = npc.StateRefresh();
        }
    }
}
