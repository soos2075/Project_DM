using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ClearPanel : UI_Scene
{
    void Start()
    {
        Init();
    }

    void Update()
    {

    }

    enum Panels
    {
        ClosePanel,
    }

    public override void Init()
    {
        Bind<Image>(typeof(Panels));

        Init_Image();


    }

    void Init_Image()
    {
        GetImage((int)Panels.ClosePanel).gameObject.AddUIEvent((data) => LeftClickEvent(), Define.UIEvent.LeftClick);
        GetImage((int)Panels.ClosePanel).gameObject.AddUIEvent((data) => RightClickEvent(), Define.UIEvent.RightClick);
    }


    void LeftClickEvent()
    {
        if (Main.Instance.CurrentAction != null) return;

        Managers.UI.CloseAll();
    }

    void RightClickEvent()
    {
        if (Managers.UI._paused != null)
        {
            Managers.UI.PauseOpen();
        }
        else
        {
            Managers.UI.CloseAll();
        }
    }

}
