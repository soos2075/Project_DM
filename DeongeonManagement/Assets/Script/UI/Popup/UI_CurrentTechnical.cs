using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CurrentTechnical : UI_PopUp
{
    private void Start()
    {
        Init();
    }

    enum Contents
    {
        NoTouch,
        Panel,
        Content,
        Close,
    }

    enum Buttons
    {
        Upgrade,
        Change,
        Remove,
    }


    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);
        CloseFloorUI();

        Bind<GameObject>(typeof(Contents));
        Bind<Button>(typeof(Buttons));


        GetObject(((int)Contents.NoTouch)).AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);
        GetObject(((int)Contents.Panel)).AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);
        GetObject(((int)Contents.Close)).AddUIEvent(data => ClosePopUp());

        GetObject(((int)Contents.Content)).GetComponent<TextMeshProUGUI>().text = tech.Data.labelName;

        GetButton((int)Buttons.Upgrade).gameObject.AddUIEvent(data => Upgrade());
        GetButton((int)Buttons.Change).gameObject.AddUIEvent(data => ChangeTechnical());
        GetButton((int)Buttons.Remove).gameObject.AddUIEvent(data => RemoveTechnical());

        if (!tech.Data.upgradePossible)
        {
            GetButton((int)Buttons.Upgrade).gameObject.SetActive(false);
        }
    }


    TechnicalFloor parent;
    Technical tech;


    public void SetCurrentTechnical(TechnicalFloor _tech)
    {
        parent = _tech;
        tech = parent.Current;
    }




    void Upgrade()
    {
        if (tech.Data.upgradePossible)
        {
            var nextTech = GameManager.Technical.GetData(tech.Data.upgradeKeyName);

            var confirm = Managers.UI.ShowPopUp<UI_Confirm>();
            confirm.SetText($"{UserData.Instance.LocaleText("Confirm_Upgrade")}\n" +
                $"{nextTech.labelName}",
                () => Upgrade_Confirm(nextTech.Mana, nextTech.Gold, nextTech.UnlockRank, nextTech.Ap, nextTech.action));

            confirm.SetMode_Calculation(nextTech.UnlockRank, $"{nextTech.Mana}", $"{nextTech.Gold}", $"{nextTech.Ap}");
        }
    }


    void ChangeTechnical()
    {
        var tech = Managers.UI.ShowPopUp<UI_Placement_Technical>("Technical/UI_Placement_Technical");
        Managers.UI.ClosePopupPick(this);
    }

    void RemoveTechnical()
    {
        GameManager.Technical.RemoveTechnical(tech);
        Managers.UI.CloseAll();
    }



    void Upgrade_Confirm(int mana, int gold, int lv, int ap, Action action)
    {
        if (ConfirmCheck(mana, gold, lv, ap))
        {
            action.Invoke();
        }
    }


    bool ConfirmCheck(int mana, int gold, int lv, int ap)
    {
        if (Main.Instance.DungeonRank < lv)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_Rank");
            return false;
        }
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
        if (Main.Instance.Player_AP < ap)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_AP");
            return false;
        }

        Main.Instance.CurrentDay.SubtractMana(mana, Main.DayResult.EventType.Technical);
        Main.Instance.CurrentDay.SubtractGold(gold, Main.DayResult.EventType.Technical);
        Main.Instance.Player_AP -= ap;

        return true;
    }









    void CloseFloorUI()
    {
        var typeUI = FindAnyObjectByType<UI_Placement_TypeSelect>();
        if (typeUI)
        {
            Managers.UI.ClosePopupPick(typeUI);
            FindObjectOfType<UI_Management>().FloorPanelClear();
        }
    }


    public override bool EscapeKeyAction()
    {
        return true;
    }


    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    private void OnDestroy()
    {
        PopupUI_OnDestroy();
    }


}
