using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }

    public override void FacilityInit()
    {
        InteractionOfTimes = 100;
        Type = FacilityType.Entrance;
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        Cor_Facility = StartCoroutine(FacilityEvent(npc));

        return Cor_Facility;
    }


    Coroutine Cor_Facility;

    IEnumerator FacilityEvent(NPC npc)
    {
        Debug.Log("°èÃþÀÌµ¿Áß");

        yield return new WaitForSeconds(1);

        Debug.Log($"{npc.name} ÀÇ AP : {npc.ActionPoint} - 1, {name} ÀÇ È½¼ö : {InteractionOfTimes} - 1");
        npc.ActionPoint--;

        Cor_Facility = null;
    }
}
