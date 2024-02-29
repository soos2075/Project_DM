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
            //Debug.Log($"{name} : 우클릭 창닫기 이벤트 추가됨");
        }

        
    }
    public void AddRightClickCloseAllEvent()
    {
        var obj = Util.FindChild(gameObject, "Panel");
        if (obj)
        {
            obj.AddUIEvent((data) => Managers.UI.CloseAll(), Define.UIEvent.RightClick);
            //Debug.Log($"{name}우클릭 전체닫기 이벤트 추가됨");
        }
    }
}
