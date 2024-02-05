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


    public UI_Floor parents { get; set; }
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
    }

    void Init_Texts()
    {
        GetTMP((int)Info.CurrentMana).text = $"현재 마나 : {Main.Instance.Player_Mana}";
        GetTMP((int)Info.CurrentMana).text += $"\n현재 골드 : {Main.Instance.Player_Gold}";
    }

    void Init_Preview()
    {
        GetObject((int)Preview.Preview_Image);

        for (int i = 0; i < 3; i++)
        {
            GetObject(i + 3).GetComponent<Image>().color = Color.clear;
            GetObject(i + 3).GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
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
            childList[i].ChangePanelColor(Define.Color_Gray);
        }

        ViewCurrentContents(content);
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
            GetObject(i + 3).GetComponent<Image>().color = Color.clear;
            GetObject(i + 3).GetComponentInChildren<TextMeshProUGUI>().text = "";
        }

        for (int i = 0; i < content.boundaryOption.Count; i++)
        {
            //? i + 3은 각각 Option_1,2,3의 인덱스임
            GetObject(i + 3).AddUIEvent((data) => PreviewRefresh(content, data.selectedObject.transform.GetSiblingIndex() - 3));

            GetObject(i + 3).GetComponent<Image>().color = Color.white;
            GetObject(i + 3).GetComponentInChildren<TextMeshProUGUI>().text += content.boundaryOption[i].addMana != 0? 
                $"마나 +{content.boundaryOption[i].addMana}" : "";
            GetObject(i + 3).GetComponentInChildren<TextMeshProUGUI>().text += content.boundaryOption[i].addGold != 0?
                $"골드 +{content.boundaryOption[i].addGold}" : "";
        }
    }

    void PreviewRefresh(ContentData content, int optionIndex)
    {
        GetObject((int)Preview.Preview_Image).GetComponent<Image>().sprite = content.sprite;
        GetObject((int)Preview.Preview_Text_Title).GetComponent<TextMeshProUGUI>().text = content.name_Placement;
        GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text = content.name_Detail;

        GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text += content.boundaryOption[optionIndex].optionText;

        GetButton((int)Buttons.Confirm).gameObject.RemoveUIEventAll();
        GetButton((int)Buttons.Confirm).gameObject.AddUIEvent(content.boundaryOption[optionIndex].Action);
    }







}
