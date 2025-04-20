using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Treasure : Facility
{
    public enum TreasureCategory
    {
        Swords = 2200,
        Rings = 2210,
        Hats = 2220,
        Scrolls = 2230,
        //Artifacts = 2240,
        Crowns = 2250,
        Chests = 2260,
    }
    public TreasureCategory treasureType;


    public override void Init_Personal()
    {
        int category = (CategoryIndex / 10) * 10;
        if (category == 2270)
        {
            category = 2260;
        }

        //int category = CategoryIndex;
        treasureType = (TreasureCategory)category;

        if (isInit)
        {
            CategorySelect(Data.SLA_label);
        }
        else
        {
            First_Instantiate();
        }

        //if (treasureType == TreasureCategory.Artifacts)
        //{
        //    AddEvent_Artifact();
        //}
    }

    public void First_Instantiate()
    {
        var SLA = GetComponentInChildren<SpriteLibrary>().spriteLibraryAsset;

        var labels = SLA.GetCategoryLabelNames(treasureType.ToString());
        List<string> labelString = new List<string>();
        foreach (var item in labels)
        {
            labelString.Add(item);
        }

        int label = UnityEngine.Random.Range(0, labelString.Count);
        string dataKeyName = labelString[label];
        Data = GameManager.Facility.GetData($"Treasure_{dataKeyName}");
        SetData();
        isInit = true;

        CategorySelect(Data.SLA_label);
    }

    void CategorySelect(string _dataKeyName)
    {
        var resolver = GetComponentInChildren<SpriteResolver>();
        //resolver.SetCategoryAndLabel(treasureType.ToString(), "Entry");
        resolver.SetCategoryAndLabel(treasureType.ToString(), _dataKeyName);
    }



    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Main.Instance.CurrentStatistics.Interaction_Treasure++;

            Cor_Facility = StartCoroutine(TreasureEvent(npc));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}의 이벤트 횟수없음");
            return null;
        }
    }

    protected IEnumerator TreasureEvent(NPC npc)
    {
        UI_EventBox.AddEventText($"●{npc.Name_Color} {UserData.Instance.LocaleText("Event_Treasure")}");
        PlacementState = PlacementState.Busy;

        bool isLastInteraction = false;
        if (InteractionOfTimes <= 0)
        {
            isLastInteraction = true;
        }

        yield return new WaitForSeconds(durationTime);

        //int changeMP = mp_value + GameManager.Buff.ManaAdd_Facility;
        int addMP = GameManager.Buff.ManaAdd_Facility;

        float multipleMP = 1;
        multipleMP += (GameManager.Buff.EffectUp_Treasure * 0.01f);
        ap_value += Mathf.RoundToInt(ap_value * (GameManager.Buff.EffectUp_Treasure * 0.01f));
        gold_value += Mathf.RoundToInt(gold_value * (GameManager.Buff.EffectUp_Treasure * 0.01f));
        pop_value += Mathf.RoundToInt(pop_value * (GameManager.Buff.EffectUp_Treasure * 0.01f));
        danger_value += Mathf.RoundToInt(danger_value * (GameManager.Buff.EffectUp_Treasure * 0.01f));

        if (npc.TraitCheck(TraitGroup.Overflow))
        {
            multipleMP += 0.2f;
        }

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

        int changeMP = Mathf.RoundToInt(mp_value * multipleMP) + addMP;
        int applyMana = Mathf.Clamp(changeMP, 0, npc.Mana); //? 높은 마나회수여도 npc가 가진 마나 이상으로 얻진 못함. - 앵벌이 방지용

        npc.Change_Mana(-applyMana);
        npc.Change_ActionPoint(-ap_value);

        if (applyMana > 0)
        {
            Main.Instance.CurrentDay.AddMana(applyMana, Main.DayResult.EventType.Facility);
            Main.Instance.ShowDM(applyMana, Main.TextType.mana, transform);
        }

        if (gold_value != 0) 
        {
            Main.Instance.CurrentDay.AddGold(gold_value, Main.DayResult.EventType.Facility);
            Main.Instance.ShowDM(gold_value, Main.TextType.gold, transform);
        }
        if (pop_value != 0)
        {
            Main.Instance.CurrentDay.AddPop(pop_value);
            Main.Instance.ShowDM(pop_value, Main.TextType.pop, transform);
        }
        if (danger_value != 0)
        {
            Main.Instance.CurrentDay.AddDanger(danger_value);
            Main.Instance.ShowDM(danger_value, Main.TextType.danger, transform);
        }

        OverCor(npc, isLastInteraction);
    }


    protected override void OverCor(NPC npc, bool isRemove)
    {
        base.OverCor(npc, isRemove);
    }



    public override void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;

        var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"[{Name}] {UserData.Instance.LocaleText("Confirm_Remove")}", () => GameManager.Facility.RemoveFacility(this));
    }




}
