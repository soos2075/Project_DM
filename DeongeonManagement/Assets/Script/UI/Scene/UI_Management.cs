using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Management : UI_Base
{
    enum ButtonEvent
    {
        Summon,
        Training,
        Special,
        Guild,
        Placement,

        Test1,
        Test2,
        Test3,


        DayChange,
    }

    enum Panels
    {
        ClosePanel,
    }

    enum Texts
    {
        Mana,
        Gold,
        AP,
        Day,
    }

    void Start()
    {
        Init();

        placement = Managers.UI.ShowSceneUI<UI_ScenePlacement>("UI_ScenePlacement");
        eventBox = Managers.UI.ShowSceneUI<UI_EventBox>("UI_EventBox");
    }
    void Update()
    {
        GetTMP(((int)Texts.Mana)).text = $"마나 : {Main.Instance.Player_Mana}";
        GetTMP(((int)Texts.Gold)).text = $"골드 : {Main.Instance.Player_Gold}";
        GetTMP(((int)Texts.AP)).text = $"행동력 : {Main.Instance.Player_AP}";
    }

    public override void Init()
    {
        Bind<Button>(typeof(ButtonEvent));
        Bind<Image>(typeof(Panels));
        Bind<TextMeshProUGUI>(typeof(Texts));


        Init_Texts();
        Init_Button();
        Init_Image();
    }


    void Init_Texts()
    {
        GetTMP(((int)Texts.Day)).text = $"{Main.Instance.Turn}일차";
        GetTMP(((int)Texts.AP)).text = $"행동력 : {Main.Instance.Player_AP}";
        GetTMP(((int)Texts.Mana)).text = $"마나 : {Main.Instance.Player_Mana}";
        GetTMP(((int)Texts.Gold)).text = $"골드 : {Main.Instance.Player_Gold}";

    }

    void Init_Button()
    {
        GetButton((int)ButtonEvent.Summon).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_Summon>());
        GetButton((int)ButtonEvent.Training).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_Training>());
        GetButton((int)ButtonEvent.Placement).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_DungeonPlacement>());

        GetButton((int)ButtonEvent.Test1).gameObject.AddUIEvent((data) => Main.Instance.AddAndActive("Adventurer"));
        GetButton((int)ButtonEvent.Test2).gameObject.AddUIEvent((data) => Main.Instance.AddAndActive("Herbalist"));
        GetButton((int)ButtonEvent.Test3).gameObject.AddUIEvent((data) => Main.Instance.AddAndActive("Miner"));

        GetButton((int)ButtonEvent.DayChange).gameObject.AddUIEvent((data) => DayChange());
    }
    void Init_Image()
    {
        GetImage((int)Panels.ClosePanel).gameObject.AddUIEvent((data) => LeftClickEvent(), Define.UIEvent.LeftClick);
        GetImage((int)Panels.ClosePanel).gameObject.AddUIEvent((data) => RightClickEvent(), Define.UIEvent.RightClick);
    }



    UI_ScenePlacement placement;
    UI_EventBox eventBox;



    void DayChange()
    {
        Main.Instance.DayChange();
        Init_Texts();

        if (!Main.Instance.Management)
        {
            eventBox.BoxActive(true);
            eventBox.TextClear();
        }
    }




    void LeftClickEvent()
    {
        if (Main.Instance.CurrentAction != null) return;

        Managers.UI.CloseAll();
    }


    void RightClickEvent()
    {
        if (Managers.UI._paused != null)
        {
            Managers.UI.PauseOpen();
        }
        else
        {
            Managers.UI.CloseAll();
        }
    }

}
