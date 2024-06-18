using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Facility : Facility, IWall
{
    public Facility OriginalTarget;


    public override int InteractionOfTimes { get => OriginalTarget.InteractionOfTimes; }
    //public override int InteractionOfTimes 
    //{
    //    get { return base.InteractionOfTimes; } 
    //    set { base.InteractionOfTimes = value; } 
    //}
    public override void Init_Personal()
    {
        if (OriginalTarget == null)
        {
            StartCoroutine(GetOriginal());
            return;
        }

        Data = OriginalTarget.Data;
        SetData();
        Data_KeyName = "Clone_Facility";
        instanceIndex = OriginalTarget.instanceIndex;
    }

    IEnumerator GetOriginal()
    {
        yield return null;
        Facility facility = GameManager.Facility.GetInstanceOfIndex(instanceIndex);
        OriginalTarget = facility;

        Data = OriginalTarget.Data;
        SetData();
        Data_KeyName = "Clone_Facility";
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        return OriginalTarget.NPC_Interaction(npc);
    }



    public override void MouseClickEvent()
    {
        OriginalTarget.MouseClickEvent();
    }


}
