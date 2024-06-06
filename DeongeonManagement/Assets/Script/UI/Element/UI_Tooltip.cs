using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Tooltip : UI_Base
{
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        gameObject.AddUIEvent((data) => ShowTooltip(), Define.UIEvent.Move);

        gameObject.AddUIEvent((data) => TooltipBoxClose(), Define.UIEvent.Exit);
    }


    public string title;

    [TextArea(3,10)]
    public string detail;



    public void SetTooltipContents(string _title, string _detail)
    {
        title = _title;
        detail = _detail;
    }


    void ShowTooltip()
    {
        //Debug.Log(gameObject.name + "ShowTooltipBox");

        var ui = Managers.UI.ShowPopUpAlone<UI_TooltipBox>();
        ui.Init_Tooltip(title, detail);
    }

    void TooltipBoxClose()
    {
        Managers.UI.ClosePopupPickType(typeof(UI_TooltipBox));
    }

}
