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
        AP,
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));

        AddUIEvent_ContentsImage();

        TextUpdate(Content.name_Placement, Content.need_Mana, Content.need_Gold, Content.need_LV, Content.need_AP);
    }



    public void TextUpdate(string name, int mana, int gold, int lv, int ap)
    {
        GetTMP((int)Texts.Name).text = name;
        GetTMP((int)Texts.Mana).text = $"{mana}";
        GetTMP((int)Texts.Gold).text = $"{gold}";
        GetTMP((int)Texts.LV).text = $"{lv}";
        GetTMP((int)Texts.AP).text = $"{ap}";
    }




    void AddUIEvent_ContentsImage()
    {
        gameObject.AddUIEvent((data) => ChangePanelColor(Define.Color_Gamma_2), Define.UIEvent.Enter);
        gameObject.AddUIEvent((data) => ChangePanelColor(Define.Color_Gamma_4), Define.UIEvent.Exit);

        gameObject.AddUIEvent((data) => LeftClick(), Define.UIEvent.LeftClick);
    }
    public void ChangePanelColor(Color color)
    {
        if (Parent.Current == Content) return;
        GetComponent<Image>().color = color;
    }


    void LeftClick()
    {
        ChangePanelColor(Define.Color_Gamma_0);
        Parent.SelectContent(Content);
    }

}
