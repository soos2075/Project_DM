using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Management : UI_Base
{
    public enum ButtonEvent
    {
        Summon,
        Management,
        Special,
        Guild,
        Placement,

        Test1,
        Test2,
        Test3,


        DayChange,
    }

    enum Texts
    {
        Mana,
        Gold,
        AP,
        Day,
    }

    Canvas canvas;

    void Start()
    {
        Init();

        placement = Managers.UI.ShowSceneUI<UI_ScenePlacement>("UI_ScenePlacement");
        eventBox = Managers.UI.ShowSceneUI<UI_EventBox>("UI_EventBox");

        canvas = GetComponent<Canvas>();

        DayZero();
    }
    void Update()
    {
        GetTMP(((int)Texts.Mana)).text = $"마나 : {Main.Instance.Player_Mana}";
        GetTMP(((int)Texts.Gold)).text = $"골드 : {Main.Instance.Player_Gold}";
        GetTMP(((int)Texts.AP)).text = $"행동력 : {Main.Instance.Player_AP}";


        if (Managers.UI._popupStack.Count > 0)
        {
            canvas.sortingOrder = -1;
        }
        else
        {
            canvas.sortingOrder = 5;
        }
    }

    public override void Init()
    {
        Bind<Button>(typeof(ButtonEvent));
        Bind<TextMeshProUGUI>(typeof(Texts));


        Init_Texts();
        Init_Button();

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
        GetButton((int)ButtonEvent.Summon).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_Summon_Monster>());

        GetButton((int)ButtonEvent.Management).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_Monster_Management>());
        //GetButton((int)ButtonEvent.Management).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_Placement_Monster>());

        GetButton((int)ButtonEvent.Placement).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_DungeonPlacement>());

        GetButton((int)ButtonEvent.Test1).gameObject.AddUIEvent((data) => Main.Instance.AddAndActive("Adventurer"));
        GetButton((int)ButtonEvent.Test2).gameObject.AddUIEvent((data) => Main.Instance.AddAndActive("Herbalist"));
        GetButton((int)ButtonEvent.Test3).gameObject.AddUIEvent((data) => Main.Instance.AddAndActive("Miner"));

        GetButton((int)ButtonEvent.DayChange).gameObject.AddUIEvent((data) => DayChange());
    }

    UI_ScenePlacement placement;
    UI_EventBox eventBox;



    void DayChange()
    {
        if (Main.Instance.Management)
        {
            eventBox.BoxActive(true);
            eventBox.TextClear();
        }

        Main.Instance.DayChange();
        Init_Texts();
    }


    void DayZero()
    {
        GetButton((int)ButtonEvent.Special).gameObject.SetActive(false);
        GetButton((int)ButtonEvent.Guild).gameObject.SetActive(false);
        //GetButton((int)ButtonEvent.DayChange).gameObject.SetActive(false);
        //GetButton((int)ButtonEvent.Test1).gameObject.SetActive(false);
        //GetButton((int)ButtonEvent.Test2).gameObject.SetActive(false);
        //GetButton((int)ButtonEvent.Test3).gameObject.SetActive(false);
    }

    public void Show_Button(ButtonEvent button) //? 메인에서 하나씩 풀면 됨
    {
        GetButton((int)button).gameObject.SetActive(true);
    }


}
