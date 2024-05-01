using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Herb : Facility
{
    public override void Init_Personal()
    {
        herbType = (HerbType)CategoryIndex;
        Init_Herb();
    }

    public enum HerbType
    {
        Low = 2000,
        High = 2001,
        Pumpkin = 2002,
    }
    public HerbType herbType { get; set; } = HerbType.Low;

    void Init_Herb()
    {
        var SLA = GetComponentInChildren<SpriteResolver>();
        SLA.SetCategoryAndLabel(herbType.ToString(), "Entry");
    }



    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            int changeMP = mp_value;

            switch (GetTarget(npc))
            {
                case Target.Main:
                    changeMP = mp_value;
                    break;

                case Target.Sub:
                    changeMP = (int)(mp_value * 0.7f);
                    break;

                case Target.Weak:
                    changeMP = (int)(mp_value * 0.3f);
                    break;

                case Target.Invalid:
                    changeMP = 0;
                    break;

                case Target.Nothing:
                    changeMP = (int)(mp_value * 0.5f);
                    break;
            }


            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, durationTime, UserData.Instance.GetLocaleText("Event_Herb"), 
                ap: ap_value, mp: changeMP, hp: hp_value));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}의 이벤트 횟수없음");
            return null;
        }
    }


    public override void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;

        var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"[{Name}] {UserData.Instance.GetLocaleText("Confirm_Remove")}");
        StartCoroutine(WaitForAnswer(ui));
    }


    IEnumerator WaitForAnswer(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            GameManager.Facility.RemoveFacility(this);
        }
    }




}
