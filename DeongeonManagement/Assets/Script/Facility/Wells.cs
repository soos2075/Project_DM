using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Wells : Facility, IWall
{
    public override void Init_Personal()
    {
        Init();

        AddEvent();
        OnRemoveEvent += () => RemoveEvent();
    }

    void Init()
    {
        SLA = GetComponentInChildren<SpriteResolver>();
        Wells_SpriteSwap();
    }

    SpriteResolver SLA;
    void Wells_SpriteSwap()
    {
        if (InteractionOfTimes == 0)
        {
            SLA.SetCategoryAndLabel("Wells", "Wells_4");
        }
        else if (InteractionOfTimes < 5)
        {
            SLA.SetCategoryAndLabel("Wells", "Wells_3");
        }
        else if (InteractionOfTimes < 9)
        {
            SLA.SetCategoryAndLabel("Wells", "Wells_2");
        }
        else
        {
            SLA.SetCategoryAndLabel("Wells", "Wells_1");
        }
    }



    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (npc.isWellsCheck)
        {
            Debug.Log($"이미 우물 먹음 꺼졍");
            if (npc.GetComponentInChildren<SpriteRenderer>(true).enabled)
            {
                npc.SetPriorityList_Update();
            }
            return null;
        }

        if (InteractionOfTimes > 0)
        {
            Debug.Log("우물이벤트 발생");

            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent_Wells(npc, durationTime, UserData.Instance.LocaleText("Event_Wells"),
                ap: ap_value, mp: mp_value, hp: hp_value));


            Wells_SpriteSwap();
            npc.isWellsCheck = true;
            return Cor_Facility;
        }
        else
        {//? 만약 이용횟수가 없어도, 이미 한걸로 쳐야지 다시 돌아감. 안그럼 우물앞에서 평생 머무르게된다.
            npc.isWellsCheck = true;
            if (npc.GetComponentInChildren<SpriteRenderer>(true).enabled)
            {
                npc.SetPriorityList_Update();
            }
            Debug.Log($"{Name}의 이벤트 횟수없음");
            return null;
        }
    }

    protected IEnumerator FacilityEvent_Wells(NPC npc, float durationTime, string text, int ap = 0, int mp = 0, int hp = 0)
    {
        UI_EventBox.AddEventText($"●{npc.Name_Color} {text}");
        //? 이용 인원수 제한 없음
        //PlacementState = PlacementState.Busy;
        yield return new WaitForSeconds(durationTime);

        npc.Change_Mana(mp);
        npc.Change_ActionPoint(ap);
        npc.Change_HP(hp);

        //npc.ActionPoint -= ap;
        //npc.Mana -= mp;
        //npc.HP -= hp;

        Main.Instance.CurrentDay.AddMana(10, Main.DayResult.EventType.Facility);
        Main.Instance.ShowDM(10, Main.TextType.mana, transform);

        OverCor_Wells(npc);
    }

    void OverCor_Wells(NPC npc)
    {
        Cor_Facility = null;
        //PlacementState = PlacementState.Standby;
        if (npc.GetComponentInChildren<SpriteRenderer>(true).enabled)
        {
            //npc.OverWell_Interaction();
            npc.SetPriorityList_Update();
        }

        AddCollectionPoint();
    }



    public override void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;

        if (InteractionOfTimes > 0) return;

        var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"[{Name}] {UserData.Instance.LocaleText("Confirm_Refill")}", () => WellsRefill());
    }

    void WellsRefill()
    {
        InteractionOfTimes = 12;
        Wells_SpriteSwap();
        //GameManager.Facility.RemoveFacility(this);
    }





    Action<int> RefillAction;
    void AddEvent()
    {
        RefillAction = (value) => WellsRefill();
        AddTurnEvent(RefillAction, DayType.Day);
    }

    void RemoveEvent()
    {
        RemoveTurnEvent(RefillAction, DayType.Day);
    }

}
