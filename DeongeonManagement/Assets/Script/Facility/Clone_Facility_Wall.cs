using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Facility_Wall : Clone_Facility, IWall
{
    //Facility _original;
    //public Facility OriginalTarget { 
    //    get { return _original; } 
    //    set 
    //    {
    //        if (_original != null && _original.isActiveAndEnabled)
    //        {
    //            Debug.Log("이미 오리지널이 존재함");
    //            return;
    //        }
    //        _original = value;
    //        _original.OnRemoveEvent += RemoveSelf_Clone;
    //    } }



    //void RemoveSelf_Clone()
    //{
    //    Debug.Log("클론삭제");
    //    GameManager.Facility.RemoveFacility(this);
    //}


    //public override int InteractionOfTimes { get => OriginalTarget.InteractionOfTimes; }

    public override void Init_Personal()
    {
        if (OriginalTarget == null)
        {
            StartCoroutine(GetOriginal());
            return;
        }

        Data = OriginalTarget.Data;
        SetData();
        Data_KeyName = "Clone_Facility_Wall";
        instanceIndex = OriginalTarget.instanceIndex;
        isOnlyOne = false;
        Name = OriginalTarget.Name;
    }

    protected override IEnumerator GetOriginal()
    {
        yield return null;
        Facility facility = GameManager.Facility.GetInstanceOfIndex(instanceIndex);
        OriginalTarget = facility;

        Data = OriginalTarget.Data;
        SetData();
        Data_KeyName = "Clone_Facility_Wall";
        isOnlyOne = false;
        Name = OriginalTarget.Name;
    }

    //public override Coroutine NPC_Interaction(NPC npc)
    //{
    //    return OriginalTarget.NPC_Interaction(npc);
    //}



    //public override void MouseClickEvent()
    //{
    //    OriginalTarget.MouseClickEvent();
    //}

    //public override void MouseMoveEvent()
    //{
    //    OriginalTarget.MouseMoveEvent();
    //}

    //public override void MouseExitEvent()
    //{
    //    OriginalTarget.MouseExitEvent();
    //}

}
