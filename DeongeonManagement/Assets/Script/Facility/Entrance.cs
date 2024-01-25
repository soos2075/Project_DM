using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        InteractionOfTimes = 100;
        Type = FacilityType.Exit;
        Name = "�������";
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        Debug.Log($"�߸��� �̺�Ʈ - {Name}");
        return null;
    }
}
