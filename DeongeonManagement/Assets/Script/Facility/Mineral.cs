using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Mineral : Facility
{
    public override void Init_Personal()
    {
        //mineralType = (MineralCategory)CategoryIndex;
        Init_Mineral();
    }

    //public enum MineralCategory
    //{
    //    Rock = 2100,
    //    Sand = 2101,
    //    Stone = 2102,
    //    Iron = 2103,
    //    Coal = 2104,
    //    Diamond = 2105,
    //}
    //public MineralCategory mineralType { get; set; } = MineralCategory.Rock;



    void Init_Mineral()
    {
        var SLA = GetComponentInChildren<SpriteResolver>();
        //SLA.SetCategoryAndLabel(mineralType.ToString(), "Entry");
        SLA.SetCategoryAndLabel(Data.SLA_category, Data.SLA_label);

        Orb_Bonus();
    }


    public void Orb_Bonus()
    {
        if (GameManager.Buff.CurrentBuff.Orb_yellow > 0)
        {
            IOT_Temp = 1;
        }
    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            int changeMP = mp_value + GameManager.Buff.MineralBonus;

            switch (TagCheck(npc))
            {
                case Target.Bonus:
                    changeMP = Mathf.RoundToInt(mp_value * 1.3f);
                    break;

                case Target.Weak:
                    changeMP = Mathf.RoundToInt(mp_value * 0.7f);
                    break;

                case Target.Invalid:
                    changeMP = Mathf.RoundToInt(mp_value * 0.1f);
                    break;

                case Target.Normal:
                    break;
            }

            if (IOT_Temp > 0)
            {
                IOT_Temp--;
            }
            else
            {
                InteractionOfTimes--;
            }

            Cor_Facility = StartCoroutine(FacilityEvent(npc, durationTime, UserData.Instance.LocaleText("Event_Mineral"), 
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
        ui.SetText($"[{Name}] {UserData.Instance.LocaleText("Confirm_Remove")}");
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
