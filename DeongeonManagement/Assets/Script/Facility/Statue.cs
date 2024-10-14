using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Statue : Facility, IWall
{
    public override void Init_Personal()
    {
        statueType = (StatueType)CategoryIndex;

        if (isInit)
        {
            CategorySelect(Data.SLA_category, Data.SLA_label);
        }
    }
    void CategorySelect(string category, string label)
    {
        var resolver = GetComponentInChildren<SpriteResolver>();
        resolver.SetCategoryAndLabel(category, label);
    }


    public override void Init_FacilityEgo()
    {
        isOnlyOne = false;
        isClearable = false;
    }


    public enum StatueType
    {
        Statue_Gold = 3901,
        Statue_Mana = 3902,
        Statue_Dog = 3903,
        Statue_Dragon = 3904,
    }
    public StatueType statueType { get; set; }


    public void Set_StatueType(StatueType type)
    {
        isInit = true;
        statueType = type;
        Data = GameManager.Facility.GetData(type.ToString());
        SetData();
        CategorySelect(Data.SLA_category, Data.SLA_label);
    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        throw new System.NotImplementedException();
    }


    public override void MouseMoveEvent()
    {
        if (Main.Instance.Management == false) return;
        if (Main.Instance.CurrentAction != null) return;
        CategorySelect(Data.SLA_category, Data.SLA_label + "_Outline");
    }
    public override void MouseExitEvent()
    {
        if (Main.Instance.Management == false) return;
        if (Main.Instance.CurrentAction != null) return;
        CategorySelect(Data.SLA_category, Data.SLA_label);
    }

    public override void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;
        if (Main.Instance.CurrentAction != null) return;

        if (Main.Instance.Player_AP <= 0)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_AP");
            return;
        }

        UI_Confirm ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();

        switch (statueType)
        {
            //$"<size=25>(1{UserData.Instance.LocaleText("AP")} {UserData.Instance.LocaleText("필요")})

            case StatueType.Statue_Gold:
                ui.SetText($"{UserData.Instance.LocaleText("Confirm_GoldStatue")}", () => Statue_Gold());
                ui.SetMode_Calculation(Define.DungeonRank.F, "0", "+50~100", "1");
                break;

            case StatueType.Statue_Mana:
                ui.SetText($"{UserData.Instance.LocaleText("Confirm_ManaStatue")}", () => Statue_Mana());
                ui.SetMode_Calculation(Define.DungeonRank.F, "+75~150", "0", "1");
                break;

            case StatueType.Statue_Dog:
                break;

            case StatueType.Statue_Dragon:
                break;
        }
    }

    void Statue_Gold()
    {
        int gold = Random.Range(50, 100);
        Main.Instance.Player_AP--;
        Main.Instance.CurrentDay.AddGold(gold, Main.DayResult.EventType.Etc);

        var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
        msg.Message = $"{gold} {UserData.Instance.LocaleText("Message_Get_Gold")}";
    }

    void Statue_Mana()
    {
        int mana = Random.Range(75, 150);
        Main.Instance.Player_AP--;
        Main.Instance.CurrentDay.AddMana(mana, Main.DayResult.EventType.Etc);

        var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
        msg.Message = $"{mana} {UserData.Instance.LocaleText("Message_Get_Mana")}";
    }

    void Statue_Dog()
    {
        Main.Instance.Player_AP--;
        //? 행동력 이벤트는 추후에 기획에 따라

        var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
    }
}
