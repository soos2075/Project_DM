using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Wall : Facility, IWall
{
    public override void Init_Personal()
    {

    }
    public override void Init_FacilityEgo()
    {
        isOnlyOne = false;
        isClearable = false;
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        throw new System.NotImplementedException();
    }


}
