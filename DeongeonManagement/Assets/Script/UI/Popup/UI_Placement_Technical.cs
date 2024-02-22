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
        NeedMana,
    }

    enum Panels
    {
        Panel,
        ClosePanel,
    }



    public UI_Technical parents { get; set; }
    public TechnicalData Current { get; set; }

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
            GetTMP((int)Info.NeedMana).text = $"\n";
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

        for (int i = 0; i < GameManager.Technical.TechnicalDataList.Count; i++)
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Technical/Technical_Content", pos).GetComponent<UI_Technical_Content>();
            content.Content = GameManager.Technical.TechnicalDataList[i];
            content.Parent = this;

            content.gameObject.name = GameManager.Technical.TechnicalDataList[i].contentName;
            childList.Add(content);
        }
    }

    public void SelectContent(TechnicalData content)
    {
        Current = content;
        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].ChangePanelColor(Color.clear);
        }
        PreviewRefresh(content);
        Set_NeedTexts(content.need_Mana, content.need_Gold, content.need_AP);
    }


    void PreviewRefresh(TechnicalData content)
    {
        GetObject((int)Preview.Preview_Image).GetComponent<Image>().sprite = content.sprite;
        GetObject((int)Preview.Preview_Text_Title).GetComponent<TextMeshProUGUI>().text = content.name_Placement;
        GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text = content.name_Detail;

        //? 이거 나중에 시설업그레이드같은거 추가하면 옵션추가하면됨
        //GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text += content.boundaryOption[optionIndex].optionText;

        GetButton((int)Buttons.Confirm).gameObject.RemoveUIEventAll();
        GetButton((int)Buttons.Confirm).gameObject.AddUIEvent(content.action);
    }


    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    private void OnDestroy()
    {
        Time.timeScale = 1;
    }

}
