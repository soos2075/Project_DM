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
        Detail_KR = "���Ϸ� ���ϴ� ���Ա��Դϴ�.";
        Name_prefab = this.GetType().Name;
    }
    public override void Init_FacilityEgo()
    {
        isOnlyOne = true;
        isClearable = true;

        if (PlacementInfo.Place_Floor.FloorIndex == 3)
        {
            GameManager.Facility.TurnOverAction += () => GameManager.Facility.RemoveFacility(this);
        }
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        Cor_Facility = StartCoroutine(FloorNext(npc));
        return Cor_Facility;
    } 


    IEnumerator FloorNext(NPC npc) //? ���������� �������� �Ա��� �������� �� ȣ��
    {
        if (npc.State == NPC.NPCState.Next)
        {
            yield return new WaitForSeconds(npc.ActionDelay);
            Main.Instance.CurrentDay.AddMana(npc.Rank + 2);
            Main.Instance.ShowDM(npc.Rank + 2, Main.TextType.mana, transform);

            npc.FloorNext();
        }
        else
        {
            npc.State = npc.StateRefresh();
        }
    }
}
