using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Tutorial : UI_PopUp
{
    void Start()
    {
        Init();
    }

    enum Images
    {
        Background,
        Close,
    }


    Coroutine Wait_Cor;

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);


        Bind<Image>(typeof(Images));

        Wait_Cor = StartCoroutine(WaitAddEvent());
    }



    IEnumerator WaitAddEvent()
    {
        yield return new WaitForSecondsRealtime(1);

        GetImage((int)Images.Close).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);
        GetImage((int)Images.Close).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);

        Wait_Cor = null;
    }


    public override bool EscapeKeyAction()
    {
        if (Wait_Cor == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}
