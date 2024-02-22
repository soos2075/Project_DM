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

    public ContentData Content { get; set; }
    public UI_Placement_Facility Parent { get; set; }



    enum Texts
    {
        Name,
        Mana,
        Gold,
        LV,
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));

        scroll = GetComponentInParent<ScrollRect>();

        AddUIEvent_ContentsImage();

        TextUpdate(Content.name_Placement, Content.need_Mana, Content.need_Gold, Content.need_LV);
    }



    public void TextUpdate(string name, int mana, int gold, int lv)
    {
        GetTMP((int)Texts.Name).text = name;
        GetTMP((int)Texts.Mana).text = $"{mana}";
        GetTMP((int)Texts.Gold).text = $"{gold}";
        GetTMP((int)Texts.LV).text = $"{lv}";
    }



    ScrollRect scroll;

    void AddUIEvent_ContentsImage()
    {
        gameObject.AddUIEvent((data) => ChangePanelColor(Color.cyan), Define.UIEvent.Enter);
        gameObject.AddUIEvent((data) => ChangePanelColor(Color.clear), Define.UIEvent.Exit);

        gameObject.AddUIEvent((data) => LeftClick(), Define.UIEvent.LeftClick);

        //? �θ��� ��ũ�ѷ�Ʈ�� �巡���̺�Ʈ ����
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
