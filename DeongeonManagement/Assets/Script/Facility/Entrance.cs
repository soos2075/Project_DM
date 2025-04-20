using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : Facility
{
    public override void Init_Personal()
    {

    }
    public override void Init_FacilityEgo()
    {
        isOnlyOne = true;
        isClearable = true;

        if (PlacementInfo.Place_Floor.FloorIndex == (int)Define.DungeonFloor.Egg)
        {
            GameManager.Facility.TurnOverAction += () => GameManager.Facility.RemoveFacility(this);
        }
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        Cor_Facility = StartCoroutine(FloorNext(npc));
        return Cor_Facility;
    } 


    IEnumerator FloorNext(NPC npc) //? 지하층으로 내려가는 입구에 도착했을 때 호출
    {
        if (npc.State == NPC.NPCState.Next)
        {
            yield return new WaitForSeconds(npc.ActionDelay);

            int applyMana = Mathf.Clamp(((npc.PlacementInfo.Place_Floor.FloorIndex) * 2), 0, npc.Mana);
            applyMana += GameManager.Buff.ManaAdd_Portal;
            if (applyMana > 0)
            {
                Main.Instance.CurrentDay.AddMana(applyMana, Main.DayResult.EventType.Etc);
                Main.Instance.ShowDM(applyMana, Main.TextType.mana, transform);
            }

            npc.FloorNext();
        }
        else
        {
            npc.State = npc.StateRefresh();
        }
    }
}
