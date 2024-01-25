using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Management : UI_Base
{
    public enum ButtonEvent
    {
        Summon,
        Training,
        Special,
        Guild,
        Placement,
        TurnOver,

        Test2,
        Test3,
    }

    public enum Panels
    {
        ClosePanel,
    }


    public override void Init()
    {
        Bind<Button>(typeof(ButtonEvent));
        Bind<Image>(typeof(Panels));

        GetImage((int)Panels.ClosePanel).gameObject.AddUIEvent((data) => LeftClickEvent(), Define.UIEvent.LeftClick);
        GetImage((int)Panels.ClosePanel).gameObject.AddUIEvent((data) => RightClickEvent(), Define.UIEvent.RightClick);
        
        GetButton((int)ButtonEvent.Summon).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_Summon>());
        GetButton((int)ButtonEvent.Training).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_Training>());
        GetButton((int)ButtonEvent.Placement).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_DungeonPlacement>());

        GetButton((int)ButtonEvent.TurnOver).gameObject.AddUIEvent((data) => TurnOverEvent());
        GetButton((int)ButtonEvent.Test2).gameObject.AddUIEvent((data) => Main.Instance.AddAndActive("Herbalist"));
        GetButton((int)ButtonEvent.Test3).gameObject.AddUIEvent((data) => Main.Instance.AddAndActive("Miner"));
    }

    void Start()
    {
        Init();

        Managers.UI.ShowSceneUI<UI_ScenePlacement>("UI_ScenePlacement");
        Managers.UI.ShowSceneUI<UI_EventBox>("UI_EventBox");
    }

    void Update()
    {
        
    }

    void TurnOverEvent()
    {
        //Main.Instance.ActiveNPC();
        //Main.Instance.ManagementOver();

        Main.Instance.AddAndActive("Adventurer");
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
