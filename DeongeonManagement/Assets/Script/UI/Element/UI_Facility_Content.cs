using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Facility_Content : UI_Base
{
    void Start()
    {
        Init();
    }

    public SO_Contents Content { get; set; }
    public UI_Placement_Facility Parent { get; set; }


    enum Texts
    {
        Category,
        Name,
        Mana,
        Gold,
        AP,
        Rank,
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));

        scroll = GetComponentInParent<ScrollRect>();

        AddUIEvent_ContentsImage();

        TextUpdate(Content.labelName, Content.Mana, Content.Gold, Content.UnlockRank);
    }



    void TextUpdate(string name, int mana, int gold, int lv)
    {
        switch (Content.priority)
        {
            case Facility_Priority.System:
                GetTMP((int)Texts.Category).text = UserData.Instance.LocaleText_Label("Interaction_Dungeon");
                break;

            case Facility_Priority.Herb:
                GetTMP((int)Texts.Category).text = UserData.Instance.LocaleText_Label("Interaction_Herb");
                break;

            case Facility_Priority.Mineral:
                GetTMP((int)Texts.Category).text = UserData.Instance.LocaleText_Label("Interaction_Mineral");
                break;

            case Facility_Priority.Trap:
                GetTMP((int)Texts.Category).text = UserData.Instance.LocaleText_Label("Interaction_Trap");
                break;

            case Facility_Priority.Treasure:
                GetTMP((int)Texts.Category).text = UserData.Instance.LocaleText_Label("Interaction_Treasure");
                break;

            case Facility_Priority.Artifact:
                GetTMP((int)Texts.Category).text = UserData.Instance.LocaleText_Label("Interaction_Artifact");
                break;

            case Facility_Priority.Etc:
                GetTMP((int)Texts.Category).text = UserData.Instance.LocaleText_Label("Interaction_Etc");
                break;
        }


        GetTMP((int)Texts.Name).text = name;
        GetTMP((int)Texts.Mana).text = $"{mana}";
        GetTMP((int)Texts.Gold).text = $"{gold}";
        GetTMP((int)Texts.AP).text = $"{Content.Ap}";
        GetTMP((int)Texts.Rank).text = $"{(Define.DungeonRank)lv}";
    }



    ScrollRect scroll;

    void AddUIEvent_ContentsImage()
    {
        gameObject.AddUIEvent((data) => ChangePanelColor(Color.cyan), Define.UIEvent.Enter);
        gameObject.AddUIEvent((data) => ChangePanelColor(Color.clear), Define.UIEvent.Exit);

        gameObject.AddUIEvent((data) => LeftClick(), Define.UIEvent.LeftClick);

        //? 부모의 스크롤렉트의 드래그이벤트 연결
        gameObject.AddUIEvent((data) => scroll.OnDrag(data), Define.UIEvent.Drag);
        gameObject.AddUIEvent((data) => scroll.OnBeginDrag(data), Define.UIEvent.BeginDrag);
        gameObject.AddUIEvent((data) => scroll.OnEndDrag(data), Define.UIEvent.EndDrag);
    }
    public void ChangePanelColor(Color color)
    {
        if (Parent.Current == Content) return;
        GetComponent<Image>().color = color;
    }


    void LeftClick()
    {
        ChangePanelColor(Color.yellow);
        Parent.SelectContent(Content);
    }


}
