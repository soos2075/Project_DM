using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_PlaySelect : UI_PopUp
{
    private void Start()
    {
        Init();
    }

    enum Contents
    {
        NoTouch,
        Panel,
        Yes,
        No,
        Content,
    }


    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<GameObject>(typeof(Contents));

        GetObject(((int)Contents.Yes)).gameObject.AddUIEvent((data) => SayYes());
        GetObject(((int)Contents.No)).gameObject.AddUIEvent((data) => SayNo());


        Cor_ComfirmAction = StartCoroutine(ConfirmAction());
    }

    Coroutine Cor_ComfirmAction;

    enum State
    {
        Wait,
        Yes,
        No,
    }

    State state = State.Wait;


    void SayYes()
    {
        //DemoManager.Instance.Demo_30_Clear = false;
        state = State.Yes;
    }
    void SayNo()
    {
        //DemoManager.Instance.Demo_30_Clear = true;
        state = State.No;
    }

    IEnumerator ConfirmAction()
    {
        yield return new WaitUntil(() => state != State.Wait);

        Cor_ComfirmAction = null;
        Managers.UI.ClosePopupPick(this);
    }


}
