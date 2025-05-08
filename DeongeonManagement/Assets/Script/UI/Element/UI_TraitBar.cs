using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TraitBar : UI_Base
{
    void Start()
    {
        //Init();
    }

    public override void Init()
    {
        traitName = GetComponentInChildren<TextMeshProUGUI>();

        if (string.IsNullOrEmpty(Data.detail) == false)
        {
            var tool = gameObject.GetOrAddComponent<UI_Tooltip>();
            tool.SetTooltipContents("", Data.detail, UI_TooltipBox.ShowPosition.LeftUp);
        }
    }


    TextMeshProUGUI traitName;

    SO_Trait Data;


    public void Init_TraitBar(SO_Trait _data)
    {
        Data = _data;
        Init();
        traitName.text = _data.labelName;


        string category = string.IsNullOrEmpty(Data.SLA_Category) ? "Basic" : Data.SLA_Category;
        string label = string.IsNullOrEmpty(Data.SLA_Label) ? "Entry" : Data.SLA_Label;

        GetComponentInChildren<Image>().sprite = Managers.Sprite.Get_SLA(SpriteManager.Library.Trait, category, label);
    }




}
