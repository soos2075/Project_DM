using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Placement_Technical : UI_PopUp
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
    }

    enum Buttons
    {
        Return,
        Confirm,
    }

    enum Info
    {
        CurrentMana,
        CurrentGold,
        CurrentAp,

        NeedMana,
        NeedGold,
        NeedAp,
    }

    enum Panels
    {
        Panel,
        ClosePanel,
    }



    //public UI_Technical parents { get; set; }
    public SO_Technical Current { get; set; }

    List<UI_Technical_Content> childList;


    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);
        childList = new List<UI_Technical_Content>();

        Bind<Image>(typeof(Panels));
        Bind<GameObject>(typeof(Preview));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Info));

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
        GetTMP((int)Info.CurrentMana).text = $"{Main.Instance.Player_Mana}";
        GetTMP((int)Info.CurrentGold).text = $"{Main.Instance.Player_Gold}";
        GetTMP((int)Info.CurrentAp).text = $"{Main.Instance.Player_AP}";
    }
    void Clear_NeedText()
    {
        GetTMP((int)Info.NeedMana).text = "";
        GetTMP((int)Info.NeedGold).text = "";
        GetTMP((int)Info.NeedAp).text = "";
    }

    void Set_NeedTexts(int mana, int gold, int ap = 0)
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
            GetTMP((int)Info.NeedGold).text = $"";
        }
        else
        {
            GetTMP((int)Info.NeedGold).text = $"-{gold}";
        }

        if (ap == 0)
        {
            GetTMP((int)Info.NeedAp).text = $"";
        }
        else
        {
            GetTMP((int)Info.NeedAp).text = $"-{ap}";
        }
    }
    void Init_Preview()
    {
        GetObject((int)Preview.Preview_Image).GetComponent<Image>().sprite = Managers.Sprite.GetClear();
        GetObject((int)Preview.Preview_Text_Title).GetComponent<TextMeshProUGUI>().text = "";
        GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text = "";
    }
    void Init_Buttons()
    {
        GetButton((int)Buttons.Return).gameObject.AddUIEvent(data => CloseAll());
    }
    void Init_Contents()
    {
        var pos = GetComponentInChildren<ContentSizeFitter>().transform;

        var techList = GameManager.Technical.GetTechnicalList(Main.Instance.DungeonRank);
        for (int i = 0; i < techList.Count; i++)
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Technical/Technical_Content", pos).GetComponent<UI_Technical_Content>();
            content.Content = techList[i];
            content.Parent = this;

            content.gameObject.name = techList[i].keyName;
            childList.Add(content);
        }
    }

    public void SelectContent(SO_Technical content)
    {
        Current = content;
        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].ChangePanelColor(Color.clear);
        }
        PreviewRefresh(content);
        Set_NeedTexts(content.Mana, content.Gold, content.Ap);
    }


    void PreviewRefresh(SO_Technical content)
    {
        GetObject((int)Preview.Preview_Image).GetComponent<Image>().sprite = 
            Managers.Sprite.Get_SLA(SpriteManager.Library.Technical, content.SLA_category, content.SLA_label);
        GetObject((int)Preview.Preview_Text_Title).GetComponent<TextMeshProUGUI>().text = content.labelName;
        GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text = content.detail;

        //? 이거 나중에 시설업그레이드같은거 추가하면 옵션추가하면됨
        //GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text += content.boundaryOption[optionIndex].optionText;

        GetButton((int)Buttons.Confirm).gameObject.RemoveUIEventAll();
        GetButton((int)Buttons.Confirm).gameObject.AddUIEvent((data) => ConfirmCheck(content.Mana, content.Gold, content.UnlockRank, content.Ap,
            content.action));
    }


    bool ConfirmCheck(int mana, int gold, int lv, int ap, Action action)
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


        Main.Instance.CurrentDay.SubtractMana(mana, Main.DayResult.EventType.Etc);
        Main.Instance.CurrentDay.SubtractGold(gold, Main.DayResult.EventType.Etc);
        Main.Instance.Player_AP -= ap;
        action.Invoke();
        return true;
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
