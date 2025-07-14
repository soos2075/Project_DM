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


    UI_TooltipBox.ShowPosition position;


    public void SetTooltipContents(string _title, string _detail, UI_TooltipBox.ShowPosition _position = UI_TooltipBox.ShowPosition.RightDown)
    {
        title = _title;
        detail = _detail;
        position = _position;
    }


    int titleSize = 25;
    int contentSize = 22;

    public void SetFontSize(int _titleSize = 25, int _contentSize = 22)
    {
        titleSize = _titleSize;
        contentSize = _contentSize;
    }


    void ShowTooltip()
    {
        //Debug.Log(gameObject.name + "ShowTooltipBox");

        var ui = Managers.UI.ShowPopUpAlone<UI_TooltipBox>();
        ui.Init_Tooltip(title, detail, position);
        ui.Init_TooltipSize(titleSize, contentSize);
    }

    void TooltipBoxClose()
    {
        Managers.UI.ClosePopupPickType(typeof(UI_TooltipBox));
    }

}
