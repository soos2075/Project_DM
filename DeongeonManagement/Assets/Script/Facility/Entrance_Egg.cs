using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance_Egg : Facility
{
    public override void Init_Personal()
    {

    }
    public override void Init_FacilityEgo()
    {
        isOnlyOne = true;
        isClearable = false;
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        Cor_Facility = StartCoroutine(Portal(npc));

        return Cor_Facility;
    }


    IEnumerator Portal(NPC npc) //? 지하층으로 내려가는 입구에 도착했을 때 호출
    {
        yield return new WaitForSeconds(npc.ActionDelay);

        if (npc.State == NPC.NPCState.Priority)
        {
            yield return new WaitForSeconds(0.5f);

            var dm = Main.Instance.dm_small.Spawn(transform.position, $"+{5} danger");
            dm.SetColor(Color.red);
            Main.Instance.CurrentDay.AddDanger(5);

            npc.FloorPortal(3);
        }
        else
        {
            npc.State = npc.StateRefresh();
        }
    }

}
