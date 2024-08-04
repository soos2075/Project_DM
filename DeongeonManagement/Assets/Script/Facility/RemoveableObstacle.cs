using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class RemoveableObstacle : Facility, IWall
{
    public override void Init_Personal()
    {
        //GetComponentInChildren<SpriteRenderer>().enabled = false;
        ObstacleType = (Obj_Label)CategoryIndex;

        if (isInit)
        {
            CategorySelect(Data.SLA_category, Data.SLA_label);
        }
        else
        {
            //Set_ObstacleType();
        }
    }
    public override void Init_FacilityEgo()
    {
        isOnlyOne = false;
        isClearable = true;
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        throw new System.NotImplementedException();
    }





    public override void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;

        int ap = 0;
        int mana = 0;
        int gold = 0;

        switch (ObstacleType)
        {
            case Obj_Label.RO_F3_01:
                ap = 1; mana = 200; gold = 150;
                break;

            case Obj_Label.RO_F3_02:
                ap = 1; mana = 200; gold = 150;
                break;

            case Obj_Label.RO_F3_03:
                ap = 1; mana = 200; gold = 150;
                break;

            case Obj_Label.RO_F3_04:
                ap = 1; mana = 200; gold = 150;
                break;

            case Obj_Label.RO_F3_05:
                ap = 1; mana = 200; gold = 150;
                break;

            case Obj_Label.RO_F3_06:
                ap = 1; mana = 200; gold = 150;
                break;
        }

        string confirm = $"<size=25>(" +
                    $"-{ap}{UserData.Instance.LocaleText("AP")}, " +
                    $"-{mana}{UserData.Instance.LocaleText("Mana")}, " +
                    $"+{gold}{UserData.Instance.LocaleText("Gold")})";

        var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"{UserData.Instance.LocaleText("Confirm_RemoveObstacle")}\n{confirm}");
        StartCoroutine(WaitForAnswer(ui, ap, mana, gold));
    }


    IEnumerator WaitForAnswer(UI_Confirm confirm, int _ap, int _mana, int _gold)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            if (ConfirmCheck(ap: _ap, mana: _mana))
            {
                Main.Instance.CurrentDay.AddGold(_gold, Main.DayResult.EventType.Etc);
                Main.Instance.CurrentDay.SubtractMana(_mana, Main.DayResult.EventType.Etc);
                Main.Instance.Player_AP -= _ap;

                GameManager.Facility.RemoveFacility(this);
                //Debug.Log($"장애물 제거 {ObstacleType}");
            }
            else
            {
                Managers.UI.ClosePopupPick(confirm);
            }
        }
    }

    bool ConfirmCheck(int mana, int gold = 0, int lv = 0, int ap = 0)
    {
        if (Main.Instance.Player_Mana < mana)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_Mana");
            return false;
        }
        if (Main.Instance.Player_Gold < gold)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_Gold");
            return false;
        }
        if (Main.Instance.DungeonRank < lv)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_Rank");
            return false;
        }
        if (Main.Instance.Player_AP < ap)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_AP");
            return false;
        }

        return true;
    }


    public override void MouseMoveEvent()
    {
        if (Main.Instance.Management == false) return;
        CategorySelect(Data.SLA_category + "_Outline", Data.SLA_label);
    }
    public override void MouseExitEvent()
    {
        if (Main.Instance.Management == false) return;
        CategorySelect(Data.SLA_category, Data.SLA_label);
    }




    public enum Obj_Label
    {

        // Floor_3
        RO_F3_01 = 3531,
        RO_F3_02 = 3532,
        RO_F3_03 = 3533,
        RO_F3_04 = 3534,
        RO_F3_05 = 3535,
        RO_F3_06 = 3536,
    }

    Obj_Label ObstacleType { get; set; }


    public void Set_ObstacleType(Obj_Label type)
    {
        isInit = true;
        ObstacleType = type;

        Data = GameManager.Facility.GetData(type.ToString());
        SetData();

        CategorySelect(Data.SLA_category, Data.SLA_label);
    }


    void CategorySelect(string category, string label)
    {
        //GetComponentInChildren<SpriteRenderer>().enabled = true;

        var resolver = GetComponentInChildren<SpriteResolver>();
        resolver.SetCategoryAndLabel(category, label);
    }

}
