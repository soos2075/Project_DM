using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Technical_Content : UI_Base
{
    void Start()
    {
        Init();
    }

    public SO_Technical Content { get; set; }
    public UI_Placement_Technical Parent { get; set; }

    enum Texts
    {
        Name,
        Mana,
        Gold,
        LV,
        AP,
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));

        scroll = GetComponentInParent<ScrollRect>();

        AddUIEvent_ContentsImage();

        TextUpdate(Content.labelName, Content.Mana, Content.Gold, Content.UnlockRank, Content.Ap);
    }



    public void TextUpdate(string name, int mana, int gold, int lv, int ap)
    {
        GetTMP((int)Texts.Name).text = name;
        GetTMP((int)Texts.Mana).text = $"{mana}";
        GetTMP((int)Texts.Gold).text = $"{gold}";
        GetTMP((int)Texts.LV).text = $"{lv}";
        GetTMP((int)Texts.AP).text = $"{ap}";
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
