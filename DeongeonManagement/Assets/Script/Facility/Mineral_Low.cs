using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mineral_Low : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        Type = FacilityType.Mineral;
        InteractionOfTimes = 3;
        Name = "ÈçÇÑ ¹ÙÀ§";
    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, 2, 1, 2, "±¤¼® Ã¤±¼Áß..."));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}ÀÇ ÀÌº¥Æ® È½¼ö¾øÀ½");
            return null;
        }
    }


}
