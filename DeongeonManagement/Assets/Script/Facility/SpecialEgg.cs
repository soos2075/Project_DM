using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEgg : Facility
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
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, 3, "�� ������...", ap: 1, mp: 5, hp: 0));

            Managers.UI.ClearAndShowPopUp<UI_GameOver>();
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}�� �̺�Ʈ Ƚ������");
            return null;
        }
    }
}
