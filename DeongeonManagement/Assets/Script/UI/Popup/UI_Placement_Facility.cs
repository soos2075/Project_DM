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

    public enum FacilityMode
    {
        Single,
        All,
    }
    public FacilityMode Mode { get; set; }



    enum Preview
    {
        Preview_Image,
        Preview_Text_Title,
        Preview_Text_Contents,
        Preview_Option_1,
        Preview_Option_2,
        Preview_Option_3,
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


    //public UI_Floor parents { get; set; }
    public ContentData Current { get; set; }

    List<UI_Facility_Content> childList;


    public override void Init()
    {
        base.Init();
        childList = new List<UI_Facility_Content>();

        AddRightClickCloseAllEvent();

        Bind<GameObject>(typeof(Preview));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Info));

        Init_Preview();
        Init_Buttons();
        Init_Contents();
        Init_Texts();
        Clear_NeedText();
    }

    void Init_Texts()
    {
        GetTMP((int)Info.CurrentMana).text = $"마나\t{Main.Instance.Player_Mana}";
        GetTMP((int)Info.CurrentMana).text += $"\n골드\t{Main.Instance.Player_Gold}";
        GetTMP((int)Info.CurrentMana).text += $"\n행동력\t{Main.Instance.Player_AP}";
    }
    void Clear_NeedText()
    {
        GetTMP((int)Info.NeedMana).text = "";
    }
    void Set_NeedTexts(int mana, int gold, int ap)
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

        if (ap == 0)
        {
            GetTMP((int)Info.NeedMana).text += $"\n";
        }
        else
        {
            GetTMP((int)Info.NeedMana).text += $"\n-{ap}";
        }
    }

    void Init_Preview()
    {
        for (int i = 0; i < 3; i++)
        {
            GetObject(i + 3).transform.parent.GetComponent<Image>().color = Color.clear;
            GetObject(i + 3).GetComponent<Image>().color = Color.clear;
            GetObject(i + 3).GetComponentInChildren<TextMeshProUGUI>().text = "";
        }

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

        for (int i = 0; i < GameManager.Content.Contents.Count; i++)
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Facility/Facility_Content", pos).GetComponent<UI_Facility_Content>();
            content.Content = GameManager.Content.Contents[i];
            content.Parent = this;

            content.gameObject.name = GameManager.Content.Contents[i].contentName;
            childList.Add(content);
        }
    }




    public void SelectContent(ContentData content)
    {
        Current = content;
        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].ChangePanelColor(Define.Color_Gamma_4);
        }

        ViewCurrentContents(content);
        Set_NeedTexts(content.need_Mana, content.need_Gold, content.need_AP);
    }

    void ViewCurrentContents(ContentData content)
    {
        PreviewRefresh(content, 0);

        for (int i = 0; i < 3; i++)
        {
            GetObject(i + 3).RemoveUIEventAll();
        }


        //? 옵션 버튼 초기화
        for (int i = 0; i < 3; i++)
        {
            GetObject(i + 3).transform.parent.GetComponent<Image>().color = Color.clear;
            GetObject(i + 3).GetComponent<Image>().color = Color.clear;
            GetObject(i + 3).GetComponentInChildren<TextMeshProUGUI>().text = "";
        }

        for (int i = 0; i < content.boundaryOption.Count; i++)
        {
            //? i + 3은 각각 Option_1,2,3의 인덱스임
            GetObject(i + 3).AddUIEvent((data) => PreviewRefresh(content, data.selectedObject.transform.parent.GetSiblingIndex() - 3));

            GetObject(i + 3).transform.parent.GetComponent<Image>().color = Color.white;
            GetObject(i + 3).GetComponent<Image>().color = Color.white;
            GetObject(i + 3).GetComponentInChildren<TextMeshProUGUI>().text += content.boundaryOption[i].addMana != 0? 
                $"마나 +{content.boundaryOption[i].addMana}" : "";
            GetObject(i + 3).GetComponentInChildren<TextMeshProUGUI>().text += content.boundaryOption[i].addGold != 0?
                $"\n골드 +{content.boundaryOption[i].addGold}" : "";
            GetObject(i + 3).GetComponentInChildren<TextMeshProUGUI>().text += content.boundaryOption[i].addAP != 0 ?
                $"\n행동력 +{content.boundaryOption[i].addAP}" : "";
        }
    }

    void PreviewRefresh(ContentData content, int optionIndex)
    {
        GetObject((int)Preview.Preview_Image).GetComponent<Image>().sprite = content.sprite;
        GetObject((int)Preview.Preview_Text_Title).GetComponent<TextMeshProUGUI>().text = content.name_Placement;
        GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text = content.name_Detail;

        GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text += content.boundaryOption[optionIndex].optionText;

        GetButton((int)Buttons.Confirm).gameObject.RemoveUIEventAll();
        //GetButton((int)Buttons.Confirm).gameObject.AddUIEvent(content.boundaryOption[optionIndex].Action);

        int mana = content.need_Mana + content.boundaryOption[optionIndex].addMana;
        int gold = content.need_Gold + content.boundaryOption[optionIndex].addGold;
        int ap = content.need_AP + content.boundaryOption[optionIndex].addAP;


        Set_NeedTexts(mana, gold, ap);

        GetButton((int)Buttons.Confirm).gameObject.AddUIEvent((data) => ConfirmCheck(mana, gold, content.need_LV, ap, 
            content.boundaryOption[optionIndex].Action));
    }



    bool ConfirmCheck(int mana, int gold, int lv, int ap, Action action)
    {
        if (Main.Instance.Player_Mana < mana)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = "마나가 부족합니다";
            return false;
        }
        if (Main.Instance.Player_Gold < gold)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = "골드가 부족합니다";
            return false;
        }
        if (Main.Instance.DungeonRank < lv)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = "던전 등급이 부족합니다";
            return false;
        }
        if (Main.Instance.Player_AP < ap)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = "행동력이 부족합니다";
            return false;
        }



        Main.Instance.CurrentAction = () => SubAction(mana, gold, ap);
        action.Invoke();
        return true;
    }


    void SubAction(int mana, int gold, int ap)
    {
        Main.Instance.CurrentDay.SubtractMana(mana);
        Main.Instance.CurrentDay.SubtractGold(gold);
        Main.Instance.Player_AP -= ap;
    }



    //private void OnEnable()
    //{
    //    Time.timeScale = 0;
    //}
    //private void OnDestroy()
    //{
    //    Time.timeScale = 1;
    //}
}
