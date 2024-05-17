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


    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);


        Bind<Image>(typeof(Images));

        StartCoroutine(WaitAddEvent());
    }



    IEnumerator WaitAddEvent()
    {
        yield return new WaitForSecondsRealtime(1);

        GetImage((int)Images.Close).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);
        GetImage((int)Images.Close).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);
    }
}
