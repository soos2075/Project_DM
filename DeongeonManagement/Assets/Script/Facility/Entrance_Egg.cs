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


    IEnumerator Portal(NPC npc)
    {
        yield return new WaitForSeconds(npc.ActionDelay);

        if (npc.State == NPC.NPCState.Priority)
        {
            yield return new WaitForSeconds(0.5f);

            var dm = Main.Instance.dm_small.Spawn(Main.Instance.Floor[0].Exit.PlacementInfo.Place_Tile.worldPosition, $"+{5} danger");
            dm.SetColor(Color.red);
            Main.Instance.CurrentDay.AddDanger(5);

            npc.FloorPortal((int)Define.DungeonFloor.Egg);

            if (Camera.main.GetComponent<CameraControl>().AutoChasing)
            {
                Camera.main.GetComponent<CameraControl>().ChasingTarget_Continue(npc.transform);
            }

            Main.Instance.CurrentStatistics.Interaction_Secret++; //? 비밀방 입장
        }
        else
        {
            npc.State = npc.StateRefresh();
        }
    }

}
