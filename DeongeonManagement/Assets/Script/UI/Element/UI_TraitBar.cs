using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_TraitBar : UI_Base
{
    void Start()
    {
        //Init();
    }

    public override void Init()
    {
        traitName = GetComponentInChildren<TextMeshProUGUI>();

        var tool = gameObject.GetOrAddComponent<UI_Tooltip>();
        tool.SetTooltipContents("", Data.detail, UI_TooltipBox.ShowPosition.LeftUp);
    }


    TextMeshProUGUI traitName;

    SO_Trait Data;


    public void Init_TraitBar(SO_Trait _data)
    {
        Data = _data;
        Init();
        traitName.text = _data.labelName;
    }




}
