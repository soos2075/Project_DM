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




    void AddUIEvent_ContentsImage()
    {
        gameObject.AddUIEvent((data) => ChangePanelColor(Define.Color_White), Define.UIEvent.Enter);
        gameObject.AddUIEvent((data) => ChangePanelColor(Define.Color_Gray), Define.UIEvent.Exit);

        gameObject.AddUIEvent((data) => LeftClick(), Define.UIEvent.LeftClick);
    }
    public void ChangePanelColor(Color color)
    {
        if (Parent.Current == Content) return;
        GetComponent<Image>().color = color;
    }


    void LeftClick()
    {
        ChangePanelColor(Define.Color_Green);
        Parent.SelectContent(Content);
    }

}

