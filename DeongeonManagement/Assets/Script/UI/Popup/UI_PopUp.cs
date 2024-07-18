using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PopUp : UI_Base
{

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        AddRightClickCloseEvent();
    }

    public virtual void PauseRefresh()
    {

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
        var obj = Util.FindChild(gameObject, "Panel");
        if (obj)
        {
            obj.AddUIEvent((data) => Managers.UI.ClosePopUp(), Define.UIEvent.RightClick);
            //Debug.Log($"{name} : ��Ŭ�� â�ݱ� �̺�Ʈ �߰���");
        }

        
    }
    public void AddRightClickCloseAllEvent()
    {
        var obj = Util.FindChild(gameObject, "Panel");
        if (obj)
        {
            obj.AddUIEvent((data) => Managers.UI.CloseAll(), Define.UIEvent.RightClick);
            //Debug.Log($"{name}��Ŭ�� ��ü�ݱ� �̺�Ʈ �߰���");
        }
    }



    protected void PopupUI_OnDestroy()
    {
        if (Managers.UI._popupStack.Count == 0)
        {
            UserData.Instance.GamePlay();
        }
        else if (Managers.UI._popupStack.Count == 1 && Managers.UI._popupStack.Peek().GetType() == typeof(UI_TileView))
        {
            //Debug.Log("Ÿ���̶���ش�");
            UserData.Instance.GamePlay();
        }
    }

    public virtual bool EscapeKeyAction()
    {
        return false;
    }


    public event Action OnPopupCloseEvent;
    public void PopupCloseCallback()
    {
        if (OnPopupCloseEvent != null)
        {
            OnPopupCloseEvent();
            OnPopupCloseEvent = null;
        }
    }
}
