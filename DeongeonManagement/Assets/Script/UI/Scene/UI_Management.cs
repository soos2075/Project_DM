using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        DayChange_Temp,
        Save,
    }

    enum Texts
    {
        Mana,
        Gold,
        AP,
        Day,
        Fame,
        Danger,
    }

    Canvas canvas;

    void Start()
    {

    }
    public void Start_Main()
    {
        Init();
    }


    void Update()
    {
        GetTMP(((int)Texts.Mana)).text = $"���� : {Main.Instance.Player_Mana}";
        GetTMP(((int)Texts.Gold)).text = $"��� : {Main.Instance.Player_Gold}";
        GetTMP(((int)Texts.AP)).text = $"�ൿ�� : {Main.Instance.Player_AP}";


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
        Managers.UI.SetCanvas(gameObject, RenderMode.ScreenSpaceCamera, false);

        placement = Managers.UI.ShowSceneUI<UI_ScenePlacement>("UI_ScenePlacement");
        placement.Init();
        eventBox = Managers.UI.ShowSceneUI<UI_EventBox>("UI_EventBox");
        canvas = GetComponent<Canvas>();

        Bind<Button>(typeof(ButtonEvent));
        Bind<TextMeshProUGUI>(typeof(Texts));

        Texts_Refresh();
        Init_Button();

        DayZero();
    }


    public void Texts_Refresh()
    {
        GetTMP(((int)Texts.Day)).text = $"{Main.Instance.Turn}����";
        GetTMP(((int)Texts.AP)).text = $"�ൿ�� : {Main.Instance.Player_AP}";
        GetTMP(((int)Texts.Mana)).text = $"���� : {Main.Instance.Player_Mana}";
        GetTMP(((int)Texts.Gold)).text = $"��� : {Main.Instance.Player_Gold}";
        GetTMP(((int)Texts.Fame)).text = $"���� : {Main.Instance.FameOfDungeon}";
        GetTMP(((int)Texts.Danger)).text = $"���赵 : {Main.Instance.DangerOfDungeon}";
    }

    void Init_Button()
    {
        GetButton((int)ButtonEvent.Summon).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_Summon_Monster>());
        GetButton((int)ButtonEvent.Management).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_Monster_Management>());
        GetButton((int)ButtonEvent.Guild).gameObject.AddUIEvent((data) => Visit_Guild());
        GetButton((int)ButtonEvent.Placement).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_DungeonPlacement>());




        GetButton((int)ButtonEvent.Test1).gameObject.AddUIEvent((data) => GameManager.NPC.TestCreate("Adventurer"));
        GetButton((int)ButtonEvent.Test2).gameObject.AddUIEvent((data) => GameManager.NPC.TestCreate("Herbalist"));
        GetButton((int)ButtonEvent.Test3).gameObject.AddUIEvent((data) => GameManager.NPC.TestCreate("Miner"));

        GetButton((int)ButtonEvent.DayChange).gameObject.AddUIEvent((data) => DayStart());
        GetButton((int)ButtonEvent.DayChange_Temp).gameObject.AddUIEvent((data) => DayChange_Temp());

        GetButton((int)ButtonEvent.Save).gameObject.AddUIEvent((data) => Managers.UI.ShowPopUp<UI_SaveLoad>());
    }

    UI_ScenePlacement placement;
    UI_EventBox eventBox;



    void DayStart()
    {
        if (Main.Instance.Management)
        {
            eventBox.BoxActive(true);
            eventBox.TextClear();
            Main.Instance.DayChange();
            Texts_Refresh();
        }
    }
    [System.Obsolete]
    void DayChange_Temp()
    {
        eventBox.BoxActive(true);
        eventBox.TextClear();
        Main.Instance.DayChange();
        Texts_Refresh();
    }


    void DayZero()
    {
        GetButton((int)ButtonEvent.Special).gameObject.SetActive(false);
        //GetButton((int)ButtonEvent.Guild).gameObject.SetActive(false);
        //GetButton((int)ButtonEvent.DayChange).gameObject.SetActive(false);
        GetButton((int)ButtonEvent.Test1).gameObject.SetActive(false);
        GetButton((int)ButtonEvent.Test2).gameObject.SetActive(false);
        GetButton((int)ButtonEvent.Test3).gameObject.SetActive(false);
    }

    public void Show_Button(ButtonEvent button) //? ���ο��� �ϳ��� Ǯ�� ��
    {
        GetButton((int)button).gameObject.SetActive(true);
    }




    void Visit_Guild()
    {
        var ui = Managers.UI.ClearAndShowPopUp<UI_Confirm>();
        ui.SetText($"��忡 �湮�ұ��?\n(���� �ൿ���� ��� �Ҹ�˴ϴ�)");
        StartCoroutine(WaitForAnswer(ui));
    }


    IEnumerator WaitForAnswer(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            Managers.Data.SaveToJson("AutoSave", 0);
            Managers.Scene.LoadSceneAsync("3_Guild");
        }
        //else if (confirm.GetAnswer() == UI_Confirm.State.No)
        //{

        //}
    }
}
