using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PopUp : UI_Base
{

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, true);

        AddRightClickCloseEvent();
    }

    public void SetCanvasSortOrder(bool onoff)
    {
        Managers.UI.SetCanvas(gameObject, onoff);
    }


    public virtual void ClosePopUp()
    {
        Managers.UI.ClosePopUp(this);
        //Debug.Log($"Close Popup : {gameObject.name}");
    }

    public void CloseAll()
    {
        Managers.UI.CloseAll();
        Debug.Log("Close Popup All");
    }

    public void CloseSelf(float delay)
    {
        Invoke("ClosePopUp", delay);
    }

    public void AddRightClickCloseEvent()
    {
        Util.FindChild(gameObject, "Panel").AddUIEvent((data) => Managers.UI.ClosePopUp(), Define.UIEvent.RightClick);
    }
    public void AddRightClickCloseAllEvent()
    {
        Util.FindChild(gameObject, "Panel").AddUIEvent((data) => Managers.UI.CloseAll(), Define.UIEvent.RightClick);
    }
}
