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
    }

    public UI_Technical parents { get; set; }
    public TechnicalData Current { get; set; }

    List<UI_Technical_Content> childList;


    public override void Init()
    {
        base.Init();
        childList = new List<UI_Technical_Content>();

        AddRightClickCloseAllEvent();

        Bind<GameObject>(typeof(Preview));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Info));


        Init_Texts();

        Init_Preview();
        Init_Buttons();
        Init_Contents();
    }

    void Init_Texts()
    {
        GetTMP((int)Info.CurrentMana).text = $"���� ���� : {Main.Instance.Player_Mana}";
        GetTMP((int)Info.CurrentMana).text += $"\n���� ��� : {Main.Instance.Player_Gold}";
    }

    void Init_Preview()
    {
        for (int i = 0; i < 3; i++)
        {
            GetObject(i + 3).GetComponent<Image>().color = Color.clear;
            GetObject(i + 3).GetComponentInChildren<TextMeshProUGUI>().text = "";
        }

        GetObject((int)Preview.Preview_Image).GetComponent<Image>().sprite = Managers.Sprite.GetSprite("Nothing");
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
            childList[i].ChangePanelColor(Define.Color_Gray);
        }
        PreviewRefresh(content);
    }


    void PreviewRefresh(TechnicalData content)
    {
        GetObject((int)Preview.Preview_Image).GetComponent<Image>().sprite = content.sprite;
        GetObject((int)Preview.Preview_Text_Title).GetComponent<TextMeshProUGUI>().text = content.name_Placement;
        GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text = content.name_Detail;

        //? �̰� ���߿� �ü����׷��̵尰���� �߰��ϸ� �ɼ��߰��ϸ��
        //GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text += content.boundaryOption[optionIndex].optionText;

        GetButton((int)Buttons.Confirm).gameObject.RemoveUIEventAll();
        GetButton((int)Buttons.Confirm).gameObject.AddUIEvent(content.action);

        //GetButton((int)Buttons.Confirm).gameObject.AddUIEvent(content.boundaryOption[optionIndex].Action);
    }

}
