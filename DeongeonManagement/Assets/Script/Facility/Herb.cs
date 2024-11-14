using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Herb : Facility
{
    public override void Init_Personal()
    {
        //herbType = (HerbType)CategoryIndex;
        Init_Herb();
    }

    //public enum HerbType
    //{
    //    Low = 2000,
    //    High = 2001,
    //    Pumpkin = 2002,
    //}
    //public HerbType herbType { get; set; } = HerbType.Low;



    void Init_Herb()
    {
        var SLA = GetComponentInChildren<SpriteResolver>();
        //SLA.SetCategoryAndLabel(herbType.ToString(), "Entry");
        SLA.SetCategoryAndLabel(Data.SLA_category, Data.SLA_label);

        Orb_Bonus();
    }

    public void Orb_Bonus()
    {
        if (GameManager.Buff.CurrentBuff.Orb_green > 0)
        {
            IOT_Temp = 1;
        }
    }



    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            //? 고정수치 보너스 (가장 마지막에 더함)
            int addMP = GameManager.Buff.HerbBonus;
            //? 배율수치 보너스 (모든 배율 수치를 더한 뒤 가장 먼저 곱함)
            float multipleMP = 1;

            switch (TagCheck(npc))
            {
                case Target.Bonus:
                    multipleMP += 0.2f;
                    break;

                case Target.Weak:
                    multipleMP -= 0.5f;
                    break;

                case Target.Invalid:
                    multipleMP -= 0.9f;
                    break;

                case Target.Normal:
                    break;
            }

            if (PlacementInfo.Place_Floor.FloorIndex == (int)Define.DungeonFloor.Floor_1)
            {
                multipleMP += 0.15f;
            }
            if (PlacementInfo.Place_Floor.FloorIndex == (int)Define.DungeonFloor.Floor_4)
            {
                multipleMP += 0.3f;
            }


            int changeMP = Mathf.RoundToInt(mp_value * multipleMP) + addMP;



            if (IOT_Temp > 0)
            {
                IOT_Temp--;
            }
            else
            {
                InteractionOfTimes--;
            }

            if (npc.TraitCheck(TraitGroup.Trample))
            {
                InteractionOfTimes = 0;
            }

            Cor_Facility = StartCoroutine(FacilityEvent(npc, durationTime, UserData.Instance.LocaleText("Event_Herb"), 
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
        ui.SetText($"[{Name}] {UserData.Instance.LocaleText("Confirm_Remove")}", () => GameManager.Facility.RemoveFacility(this));
    }


}
