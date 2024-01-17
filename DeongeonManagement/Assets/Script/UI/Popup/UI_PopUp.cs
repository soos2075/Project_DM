using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PopUp : UI_Base
{

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, true);
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

}
