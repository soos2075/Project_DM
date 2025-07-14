using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_OptionBox : UI_PopUp
{
    void Start()
    {
        Init();
    }

    enum Contents
    {
        Panel,
        OptionBox,
    }

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);
        Bind<GameObject>(typeof(Contents));

        //GetObject((int)Contents.Panel).AddUIEvent((data) => CloseBox(), Define.UIEvent.RightClick);
    }

    void CloseBox()
    {
        Managers.UI.CloseAll();
        Managers.Dialogue.currentDialogue = null;

        //Time.timeScale = 1;
        //UserData.Instance.GamePlay();
        UserData.Instance.GameMode = Define.TimeMode.Normal;
    }



    public Transform GetTransform()
    {
        return transform.GetChild(0).GetChild(0).transform;
    }

}
