using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SaveLoad_Confirm : UI_PopUp
{
    private void Start()
    {
        Init();
    }

    enum Contents
    {
        NoTouch,
        Panel,
        Content,
    }

    enum Buttons
    {
        Save,
        Load,
        Cancel,
    }

    string textContent;

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<GameObject>(typeof(Contents));
        Bind<Button>(typeof(Buttons));


        GetObject(((int)Contents.NoTouch)).gameObject.AddUIEvent((data) => Close(), Define.UIEvent.RightClick);
        GetObject(((int)Contents.Panel)).gameObject.AddUIEvent((data) => Close(), Define.UIEvent.RightClick);

        GetButton(((int)Buttons.Cancel)).gameObject.AddUIEvent((data) => Close());

        GetButton(((int)Buttons.Save)).gameObject.AddUIEvent((data) => state = State.Save);
        GetButton(((int)Buttons.Load)).gameObject.AddUIEvent((data) => state = State.Load);


        GetObject(((int)Contents.Content)).GetComponent<TextMeshProUGUI>().text = textContent;


        Cor_ComfirmAction = StartCoroutine(ConfirmAction());
    }


    void Close()
    {
        Managers.UI.ClosePopupPick(this);
    }


    Coroutine Cor_ComfirmAction;

    enum State
    {
        Wait,
        Save,
        Load,
    }

    State state = State.Wait;

    public void SetAction(string _content, Action saveAction, Action loadAction)
    {
        textContent = _content;
        SaveAction = saveAction;
        LoadAction = loadAction;
    }

    Action SaveAction;
    Action LoadAction;


    IEnumerator ConfirmAction()
    {
        yield return new WaitUntil(() => state != State.Wait);

        if (state == State.Save)
        {
            SaveAction?.Invoke();
        }
        else if (state == State.Load)
        {
            LoadAction?.Invoke();
        }

        Cor_ComfirmAction = null;
        Managers.UI.ClosePopupPick(this);
    }




    public override bool EscapeKeyAction()
    {
        return true;
    }


    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    private void OnDestroy()
    {
        PopupUI_OnDestroy();
    }


}
