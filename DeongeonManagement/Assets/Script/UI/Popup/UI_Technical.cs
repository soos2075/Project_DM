using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Technical : UI_Scene, IWorldSpaceUI
{
    void Start()
    {
        //Init();
    }


    public void SetCanvasWorldSpace()
    {
        Managers.UI.SetCanvas(gameObject, RenderMode.WorldSpace, false);
    }
    public override void Init()
    {
        SetCanvasWorldSpace();

        AddUIEvent(gameObject, (data) => MoveEvent(data), Define.UIEvent.Move);
        AddUIEvent(gameObject, (data) => CloseView(), Define.UIEvent.Exit);

        AddUIEvent(gameObject, (data) => LeftClickEvent(data), Define.UIEvent.LeftClick);
        AddUIEvent(gameObject, (data) => Managers.UI.CloseAll(), Define.UIEvent.RightClick);
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
        //view.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        view.ViewContents($"[{Parent.Current.Data.labelName}]", $"{Parent.Current.Data.detail}");
    }
    void CloseView()
    {
        if (view)
        {
            Managers.UI.ClosePopupPick(view);
        }
    }

    void LeftClickEvent(PointerEventData data)
    {
        if (!Main.Instance.Management) return;
        if (Main.Instance.CurrentAction != null) return;


        if (Parent.Current != null)
        {
            Demolition_Technical();
            CloseView();
        }
        else
        {
            Main.Instance.CurrentTechnical = Parent;

            var popup = Managers.UI.ClearAndShowPopUp<UI_Technical_Select>("Technical/UI_Technical_Select");
            var pos = Camera.main.ScreenToWorldPoint(data.position);
            popup.transform.localPosition = new Vector3(pos.x, pos.y, 5);
            popup.parents = this;
        }
    }



    void Demolition_Technical()
    {
        var ui = Managers.UI.ShowPopUp<UI_Confirm>();
        ui.SetText($"[{Parent.Current.Data.labelName}] {UserData.Instance.LocaleText("Confirm_Remove")}");
        StartCoroutine(WaitForAnswer(ui));
    }

    IEnumerator WaitForAnswer(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            GameManager.Technical.RemoveTechnical(Parent.Current);
        }
        //else if (confirm.GetAnswer() == UI_Confirm.State.No)
        //{

        //}
    }
}
