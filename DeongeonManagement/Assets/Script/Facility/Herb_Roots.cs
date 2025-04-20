using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Herb_Roots : Facility
{
    public override void Init_Personal()
    {
        Init_Herb();

        Roots_Init();

        AddEvent();
        OnRemoveEvent += () => RemoveEvent();
    }



    void Roots_Init()
    {
        if (Data.id == 2010)
        {
            Set_HerbRoots("Herb_High", 4, 3);
        }
    }




    string targetHerbDataName;
    int instanceCount;
    int maxLenght;
    public void Set_HerbRoots(string target, int inst, int maxLeng)
    {
        targetHerbDataName = target;
        instanceCount = inst;
        maxLenght = maxLeng;
    }

    Action<int> Inst_Herb;
    void AddEvent()
    {
        Inst_Herb = (value) => Herb_Seeding();
        AddTurnEvent(Inst_Herb, DayType.Night);
    }

    void RemoveEvent()
    {
        RemoveTurnEvent(Inst_Herb, DayType.Night);
    }

    void Herb_Seeding()
    {
        bool isFind = false;
        var emptyList = PlacementInfo.Place_Floor.GetAroundTile(PlacementInfo.Place_Tile, maxLenght, out isFind);

        if (isFind == false)
        {
            Debug.Log("주변에 빈칸 없음");
            return;
        }

        for (int i = 0; i < instanceCount; i++)
        {
            if (emptyList.Count <= i)
            {
                break;
            }

            //? 소환하기
            PlacementInfo info = new PlacementInfo(PlacementInfo.Place_Floor, emptyList[i]);
            var obj = GameManager.Facility.CreateFacility(targetHerbDataName, info);
        }
    }





    void Init_Herb()
    {
        var SLA = GetComponentInChildren<SpriteResolver>();
        //SLA.SetCategoryAndLabel(herbType.ToString(), "Entry");
        SLA.SetCategoryAndLabel(Data.SLA_category, Data.SLA_label);

        Orb_Bonus();
    }

    public void Orb_Bonus()
    {
        if (GameManager.Buff.CurrentBuff.Orb_green >= 3)
        {
            IOT_Temp = 1;
        }
    }



    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            //? 고정수치 보너스 (가장 마지막에 더함)
            int addMP = GameManager.Buff.ManaAdd_Herb;
            //? 배율수치 보너스 (모든 배율 수치를 더한 뒤 가장 먼저 곱함)
            float multipleMP = 1;
            multipleMP += (GameManager.Buff.ManaUp_Herb * 0.01f);

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
                multipleMP += 0.1f;
            }
            if (PlacementInfo.Place_Floor.FloorIndex == (int)Define.DungeonFloor.Floor_4)
            {
                multipleMP += 0.2f;
            }

            if (GameManager.Buff.CurrentBuff.Orb_green >= 1)
            {
                multipleMP += 0.1f;
            }
            if (GameManager.Buff.CurrentBuff.Orb_green >= 2)
            {
                multipleMP += 0.1f;
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
            Main.Instance.CurrentStatistics.Interaction_Herb++;

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
