using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Technical : UI_Scene, IWorldSpaceUI
{
    void Start()
    {
        SetCanvasWorldSpace();
    }
    public void SetCanvasWorldSpace()
    {
        Managers.UI.SetCanvas(gameObject, RenderMode.WorldSpace, false);

        AddUIEvent(gameObject, (data) => MoveEvent(data), Define.UIEvent.Move);
        AddUIEvent(gameObject, (data) => CloseView(), Define.UIEvent.Exit);

        AddUIEvent(gameObject, (data) => LeftClickEvent(data), Define.UIEvent.LeftClick);
    }


    public TechnicalFloor Parent { get; set; }


    UI_TileView view;

    void MoveEvent(PointerEventData data)
    {
        if (Parent.Current == null) return;

        if (view == null)
        {
            view = Managers.UI.ShowPopUpAlone<UI_TileView>();
        }

        var pos = Camera.main.ScreenToWorldPoint(data.position);
        view.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        view.ViewContents($"[{Parent.Current.Data.name_Placement}]", $"{Parent.Current.Data.name_Detail}");
    }
    void CloseView()
    {
        if (view)
        {
            Managers.UI.ClosePopUp(view);
        }
    }

    void LeftClickEvent(PointerEventData data)
    {
        if (Parent.Current != null || !Main.Instance.Management)
        {
            return;
        }

        Main.Instance.CurrentTechnical = Parent;

        var popup = Managers.UI.ShowPopUpAlone<UI_Technical_Select>("Technical/UI_Technical_Select");
        var pos = Camera.main.ScreenToWorldPoint(data.position);
        popup.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        popup.parents = this;
    }

}
