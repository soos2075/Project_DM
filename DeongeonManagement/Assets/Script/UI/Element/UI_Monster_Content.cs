using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Monster_Content : UI_Base
{
    void Start()
    {
        Init();
    }

    public MonsterData Content { get; set; }
    public UI_Summon_Monster Parent;
    enum Texts
    {
        LV,
        MANA,
        NAME,
        HP,
        ATK,
        DEF,
        AGI,
        LUK,
        MAXLV,
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));

        scroll = GetComponentInParent<ScrollRect>();

        AddUIEvent_ContentsImage();

        TextUpdate();
    }



    public void TextUpdate()
    {
        GetTMP((int)Texts.LV).text = $"{Content.LV}";
        GetTMP((int)Texts.MANA).text = $"{Content.ManaCost}";
        GetTMP((int)Texts.NAME).text = $"{Content.Name_KR}";
        GetTMP((int)Texts.HP).text = $"{Content.HP}";
        GetTMP((int)Texts.ATK).text = $"{Content.ATK}";
        GetTMP((int)Texts.DEF).text = $"{Content.DEF}";
        GetTMP((int)Texts.AGI).text = $"{Content.AGI}";
        GetTMP((int)Texts.LUK).text = $"{Content.LUK}";
        GetTMP((int)Texts.MAXLV).text = $"{Content.MAXLV}";
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

