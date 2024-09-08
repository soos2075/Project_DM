using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Summon_Monster : UI_PopUp
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

        label,
        mana,
        hp,
        atk,
        def,
        agi,
        luk,
        lv,
        maxlv,
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

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<Image>(typeof(Panels));
        Bind<GameObject>(typeof(Preview));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Info));

        Init_Panels();
        Init_Preview();
        Init_Buttons();
        Init_Texts();
        Clear_NeedText();
        Init_Contents();
    }

    void Init_Panels()
    {
        GetImage(((int)Panels.ClosePanel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);
        GetImage(((int)Panels.ClosePanel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);
        GetImage(((int)Panels.Panel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);
    }

    void Init_Preview()
    {
        GetObject((int)Preview.Preview_Image).GetComponent<Image>().sprite = Managers.Sprite.GetClear();
        GetObject((int)Preview.Preview_Text_Title).GetComponent<TextMeshProUGUI>().text = "";
        GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text = "";

        GetObject((int)Preview.label).AddUIEvent((data) => Sort_Contents(Preview.label));
        GetObject((int)Preview.mana).AddUIEvent((data) => Sort_Contents(Preview.mana));
        GetObject((int)Preview.hp).AddUIEvent((data) => Sort_Contents(Preview.hp));
        GetObject((int)Preview.atk).AddUIEvent((data) => Sort_Contents(Preview.atk));
        GetObject((int)Preview.def).AddUIEvent((data) => Sort_Contents(Preview.def));
        GetObject((int)Preview.agi).AddUIEvent((data) => Sort_Contents(Preview.agi));
        GetObject((int)Preview.luk).AddUIEvent((data) => Sort_Contents(Preview.luk));
        GetObject((int)Preview.lv).AddUIEvent((data) => Sort_Contents(Preview.lv));
        GetObject((int)Preview.maxlv).AddUIEvent((data) => Sort_Contents(Preview.maxlv));
    }
    void Init_Buttons()
    {
        GetButton((int)Buttons.Return).gameObject.AddUIEvent(data => ClosePopUp());
    }

    void Init_Contents()
    {
        var pos = GetComponentInChildren<ContentSizeFitter>().transform;

        var list = GameManager.Monster.GetSummonList(Main.Instance.DungeonRank);
        for (int i = 0; i < list.Count; i++)
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Monster/Monster_Content", pos).GetComponent<UI_Monster_Content>();
            content.Content = list[i];
            content.Parent = this;

            childList.Add(content);
        }
    }

    Preview currentOption;
    bool ascending = true;

    void Sort_Contents(Preview sortOption)
    {
        foreach (var item in childList)
        {
            Managers.Resource.Destroy(item.gameObject);
        }
        childList = new List<UI_Monster_Content>();


        var pos = GetComponentInChildren<ContentSizeFitter>().transform;

        var list = GameManager.Monster.GetSummonList(Main.Instance.DungeonRank);
        switch (sortOption)
        {
            case Preview.label:
                list.Sort((a, b) => a.labelName.CompareTo(b.labelName));
                break;
            case Preview.mana:
                list.Sort((a, b) => a.manaCost.CompareTo(b.manaCost));
                break;
            case Preview.hp:
                list.Sort((a, b) => a.hp.CompareTo(b.hp));
                break;
            case Preview.atk:
                list.Sort((a, b) => a.atk.CompareTo(b.atk));
                break;
            case Preview.def:
                list.Sort((a, b) => a.def.CompareTo(b.def));
                break;
            case Preview.agi:
                list.Sort((a, b) => a.agi.CompareTo(b.agi));
                break;
            case Preview.luk:
                list.Sort((a, b) => a.luk.CompareTo(b.luk));
                break;
            case Preview.lv:
                list.Sort((a, b) => a.startLv.CompareTo(b.startLv));
                break;
            case Preview.maxlv:
                list.Sort((a, b) => a.maxLv.CompareTo(b.maxLv));
                break;
        }

        ascending = currentOption == sortOption ? !ascending : true;
        if (ascending == false)
        {
            list.Reverse();
        }
        currentOption = sortOption;

        for (int i = 0; i < list.Count; i++)
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Monster/Monster_Content", pos).GetComponent<UI_Monster_Content>();
            content.Content = list[i];
            content.Parent = this;

            childList.Add(content);
        }
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





    public SO_Monster Current { get; set; }
    List<UI_Monster_Content> childList = new List<UI_Monster_Content>();

    public void SelectContent(SO_Monster content)
    {
        if (Current == content)
        {
            Debug.Log($"중복선택됨 - {content.name}");
            MonsterSummon(content);
            return;
        }

        Current = content;
        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].ChangePanelColor(Color.clear);
        }
        PreviewRefresh(content);
        Set_NeedTexts(content.manaCost, 0, 0);
    }


    void PreviewRefresh(SO_Monster content)
    {
        GetObject((int)Preview.Preview_Image).GetComponent<Image>().sprite = Managers.Sprite.GetSprite_SLA(content.SLA_category, content.SLA_label);
        GetObject((int)Preview.Preview_Text_Title).GetComponent<TextMeshProUGUI>().text = content.labelName;
        GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text = content.detail;


        GetButton((int)Buttons.Confirm).gameObject.RemoveUIEventAll();
        GetButton((int)Buttons.Confirm).gameObject.AddUIEvent((data) => MonsterSummon(content));
    }



    void MonsterSummon(SO_Monster data)
    {
        if (GameManager.Monster.MaximumCheck())
        {
            if (ConfirmCheck(data.manaCost))
            {
                SummonConfirm(data);
            }
        }
        else
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_Slot");
        }
    }


    void SummonConfirm(SO_Monster data)
    {
        var mon = GameManager.Placement.CreatePlacementObject(data.prefabPath, null, PlacementType.Monster) as Monster;
        mon.MonsterInit();
        mon.Initialize_Status();

        mon.AddCollectionPoint();

        GameManager.Monster.AddMonster(mon);

        //Debug.Log($"{data.ManaCost}마나를 사용하여 {data.Name_KR}을 소환");
        Main.Instance.CurrentDay.SubtractMana(data.manaCost, Main.DayResult.EventType.Monster);

        Init_Texts();

        var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
        msg.Message = $"{data.labelName} {UserData.Instance.LocaleText("Message_Summon")}";

        SoundManager.Instance.PlaySound("SFX/Action_Summon");
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

