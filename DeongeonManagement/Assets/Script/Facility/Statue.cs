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
        Statue_Ravi = 3905,
        Statue_Cat = 3906,
        Statue_Demon = 3907,
        Statue_Hero = 3908,
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

        //if (Main.Instance.Player_AP <= 0)
        //{
        //    var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
        //    msg.Message = UserData.Instance.LocaleText("Message_No_AP");
        //    return;
        //}

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
                //? 1행동력 / 마나 혹은 골드를 좀 더 많은양
                ui.SetText($"{UserData.Instance.LocaleText("Confirm_Pray_신뢰의상")}", () => Statue_Dog());
                ui.SetMode_Calculation(Define.DungeonRank.F, "0", "0", "1");
                break;

            case StatueType.Statue_Dragon:
                //? 2행동력 / 위험도 상승
                ui.SetText($"{UserData.Instance.LocaleText("Confirm_Pray_파멸의상")}", () => Statue_Dragon());
                ui.SetMode_Calculation(Define.DungeonRank.F, "0", "0", "2");
                break;

            case StatueType.Statue_Ravi:
                //? 2행동력 / 인기도 상승
                ui.SetText($"{UserData.Instance.LocaleText("Confirm_Pray_평온의상")}", () => Statue_Ravi());
                ui.SetMode_Calculation(Define.DungeonRank.F, "0", "0", "2");
                break;

            case StatueType.Statue_Cat:
                //? 3행동력 / 모든 유닛 레벨 +1
                ui.SetText($"{UserData.Instance.LocaleText("Confirm_Pray_친화의상")}", () => Statue_Cat());
                ui.SetMode_Calculation(Define.DungeonRank.F, "0", "0", "3");
                break;

            case StatueType.Statue_Demon:
                //? 4행동력 / 영구적으로 모든 유닛 강화
                ui.SetText($"{UserData.Instance.LocaleText("Confirm_Pray_공포의상")}", () => Statue_Demon());
                ui.SetMode_Calculation(Define.DungeonRank.F, "0", "0", "4");
                break;

            case StatueType.Statue_Hero:
                //? 4행동력 / 랜덤한 아티팩트를 하나 획득
                ui.SetText($"{UserData.Instance.LocaleText("Confirm_Pray_영웅의상")}", () => Statue_Hero());
                ui.SetMode_Calculation(Define.DungeonRank.F, "0", "0", "4");
                break;
        }
    }


    bool AP_Check(int value)
    {
        if (Main.Instance.Player_AP >= value)
        {
            return true;
        }
        else
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_AP");
            return false;
        }
    }



    void Statue_Gold()
    {
        if (AP_Check(1) == false)
        {
            return;
        }

        Main.Instance.Player_AP--;

        int gold = Random.Range(50, 100);
        Main.Instance.CurrentDay.AddGold(gold, Main.DayResult.EventType.Etc);

        var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
        msg.Message = $"{gold} {UserData.Instance.LocaleText("Message_Get_Gold")}";
    }

    void Statue_Mana()
    {
        if (AP_Check(1) == false)
        {
            return;
        }

        Main.Instance.Player_AP--;

        int mana = Random.Range(75, 150);
        Main.Instance.CurrentDay.AddMana(mana, Main.DayResult.EventType.Etc);

        var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
        msg.Message = $"{mana} {UserData.Instance.LocaleText("Message_Get_Mana")}";
    }

    void Statue_Dog()
    {
        if (AP_Check(1) == false)
        {
            return;
        }

        Main.Instance.Player_AP--;

        //? 마나랑 골드 둘중에 하나만 오르게
        if (Random.Range(0,2) == 0)
        {
            int mana = Random.Range(150, 300);
            Main.Instance.CurrentDay.AddMana(mana, Main.DayResult.EventType.Etc);
            var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
            msg.Message = $"{mana} {UserData.Instance.LocaleText("Message_Get_Mana")}";
        }
        else
        {
            int gold = Random.Range(100, 200);
            Main.Instance.CurrentDay.AddGold(gold, Main.DayResult.EventType.Etc);
            var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
            msg.Message = $"{gold} {UserData.Instance.LocaleText("Message_Get_Gold")}";
        }
    }
    void Statue_Dragon()
    {
        if (AP_Check(2) == false)
        {
            return;
        }

        Main.Instance.Player_AP -= 2;

        int value = Random.Range(20, 51);
        Main.Instance.CurrentDay.AddDanger_Directly(value);
        var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
        msg.Message = $"{value} {UserData.Instance.LocaleText("Message_Get_Danger")}";

    }
    void Statue_Ravi()
    {
        if (AP_Check(2) == false)
        {
            return;
        }

        Main.Instance.Player_AP -= 2;

        int value = Random.Range(20, 51);
        Main.Instance.CurrentDay.AddPop_Directly(value);
        var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
        msg.Message = $"{value} {UserData.Instance.LocaleText("Message_Get_Pop")}";
    }

    void Statue_Cat()
    {
        if (AP_Check(3) == false)
        {
            return;
        }

        Main.Instance.Player_AP -= 3;

        foreach (var item in GameManager.Monster.GetMonsterAll())
        {
            item.Statue_Cat();
        }

    }

    void Statue_Demon()
    {
        if (AP_Check(4) == false)
        {
            return;
        }

        Main.Instance.Player_AP -= 4;

        //? 영구적으로 모든 유닛 강화
        foreach (var item in GameManager.Monster.GetMonsterAll())
        {
            item.Statue_Demon();
        }
    }

    void Statue_Hero()
    {
        if (AP_Check(4) == false)
        {
            return;
        }

        Main.Instance.Player_AP -= 4;

        //? 랜덤 아티팩트 획득
        GameManager.Artifact.Add_RandomArtifact();
    }
}
