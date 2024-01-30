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

    public TechnicalData Content { get; set; }
    public UI_Placement_Technical Parent { get; set; }

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
