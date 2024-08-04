using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Placement_Facility : UI_PopUp
{
    void Start()
    {
        Init();
    }

    enum Preview
    {
        Preview_Image,
        Preview_Text_Title,
        Preview_Text_Contents,
        Preview_Area,

        //PlacementOption,
    }

    enum Buttons
    {
        Return,
        Confirm,
    }

    enum Info
    {
        CurrentMana,
        NeedMana,
    }

    enum Panels
    {
        Panel,
        ClosePanel,
    }


    Toggle placeOptionToggle;

    public SO_Contents Current { get; set; }

    List<UI_Facility_Content> childList;


    public override void Init()
    {
        //base.Init();
        Managers.UI.SetCanvas(gameObject);
        childList = new List<UI_Facility_Content>();

        Bind<Image>(typeof(Panels));
        Bind<GameObject>(typeof(Preview));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Info));

        placeOptionToggle = GetComponentInChildren<Toggle>();
        placeOptionToggle.isOn = Main.Instance.isContinueOption;

        Init_Panels();
        Init_Preview();
        Init_Buttons();
        Init_Contents();
        Init_Texts();
        Clear_NeedText();
    }


    void Init_Panels()
    {
        GetImage(((int)Panels.ClosePanel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);
        GetImage(((int)Panels.ClosePanel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);
        GetImage(((int)Panels.Panel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);
    }

    void Init_Texts()
    {
        GetTMP((int)Info.CurrentMana).text = $"{UserData.Instance.LocaleText("Mana")}\t{Main.Instance.Player_Mana}";
        GetTMP((int)Info.CurrentMana).text += $"\n{UserData.Instance.LocaleText("Gold")}\t{Main.Instance.Player_Gold}";
        //GetTMP((int)Info.CurrentMana).text += $"\n행동력\t{Main.Instance.Player_AP}";
    }
    void Clear_NeedText()
    {
        GetTMP((int)Info.NeedMana).text = "";
    }
    void Set_NeedTexts(int mana, int gold)
    {
        if (mana == 0)
        {
            GetTMP((int)Info.NeedMana).text = $"";
        }
        else
        {
            GetTMP((int)Info.NeedMana).text = $"-{mana}";
        }

        if (gold == 0)
        {
            GetTMP((int)Info.NeedMana).text += $"\n";
        }
        else
        {
            GetTMP((int)Info.NeedMana).text += $"\n-{gold}";
        }
    }

    void Init_Preview()
    {
        GetObject((int)Preview.Preview_Image).GetComponent<Image>().sprite = Managers.Sprite.GetClear();
        GetObject((int)Preview.Preview_Text_Title).GetComponent<TextMeshProUGUI>().text = "";
        GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text = "";
        GetObject((int)Preview.Preview_Area).GetComponent<Image>().sprite = Managers.Sprite.Get_Area("");
    }
    void Init_Buttons()
    {
        GetButton((int)Buttons.Return).gameObject.AddUIEvent(data => CloseAll());
    }
    void Init_Contents()
    {
        var pos = GetComponentInChildren<ContentSizeFitter>().transform;

        //GameManager.Content.Contents.Sort(new ContentComparer());
        var contentsList = GameManager.Content.GetContentsList(Main.Instance.DungeonRank);

        for (int i = 0; i < contentsList.Count; i++)
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Facility/Facility_Content", pos).GetComponent<UI_Facility_Content>();
            content.Content = contentsList[i];
            content.Parent = this;
            content.gameObject.name = contentsList[i].labelName;
            childList.Add(content);
        }
    }




    public void SelectContent(SO_Contents content)
    {
        if (Current == content)
        {
            Debug.Log($"중복선택됨 - {content.name}");
            ConfirmCheck(content.Mana, content.Gold, content.UnlockRank, content.action);
            return;
        }


        Current = content;
        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].ChangePanelColor(Color.clear);
        }

        PreviewRefresh(content);
        Set_NeedTexts(content.Mana, content.Gold);
    }



    void PreviewRefresh(SO_Contents content)
    {
        GetObject((int)Preview.Preview_Image).GetComponent<Image>().sprite = Managers.Sprite.GetSprite(content.spritePath);
        GetObject((int)Preview.Preview_Text_Title).GetComponent<TextMeshProUGUI>().text = content.labelName;
        GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text = content.detail;

        //GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text += "\n" + content.boundary;
        GetObject((int)Preview.Preview_Area).GetComponent<Image>().sprite = Managers.Sprite.Get_Area(content.area_name);


        GetButton((int)Buttons.Confirm).gameObject.RemoveUIEventAll();
        GetButton((int)Buttons.Confirm).gameObject.AddUIEvent((data) => ConfirmCheck(content.Mana, content.Gold, content.UnlockRank,
            content.action));
    }


    bool ConfirmCheck(int mana, int gold, int lv, Action action)
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


        FindAnyObjectByType<UI_Management>().Hide_MainCanvas();
        //Main.Instance.PurchaseAction = () => SubAction(mana, gold);
        Main.Instance.CurrentPurchase = new Main.PurchaseInfo(mana, gold, placeOptionToggle.isOn);
        action.Invoke();
        return true;
    }


    //void SubAction(int mana, int gold)
    //{
    //    Main.Instance.CurrentDay.SubtractMana(mana);
    //    Main.Instance.CurrentDay.SubtractGold(gold);
    //}




    public override void PauseRefresh()
    {
        Init_Texts();
    }




    public override bool EscapeKeyAction()
    {
        return true;
    }




    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    //private void OnDisable()
    //{
    //    Time.timeScale = 1;
    //}
    private void OnDestroy()
    {
        PopupUI_OnDestroy();
    }
}
