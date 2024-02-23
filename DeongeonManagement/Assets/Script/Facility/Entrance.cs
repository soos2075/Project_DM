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
        Name = "입구";
        Detail_KR = "던전의 더 깊은곳으로 이어지는 출입구입니다.";
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


    IEnumerator FloorNext(NPC npc) //? 지하층으로 내려가는 입구에 도착했을 때 호출
    {
        yield return new WaitForSeconds(npc.ActionDelay);

        if (npc.State == NPC.NPCState.Next)
        {
            yield return new WaitForSeconds(0.5f);
            Main.Instance.CurrentDay.AddMana(3);
            var dm = Main.Instance.dmMesh_dungeon.Spawn(transform.position, $"+{3} mana");
            dm.SetColor(Color.blue);
            npc.FloorNext();
        }
        else
        {
            npc.State = npc.StateRefresh();
        }
    }
}
