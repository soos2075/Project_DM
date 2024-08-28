using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_InputField : UI_PopUp
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

        InputField,
    }





    //public UI_Base Parent { get; set; }
    string textContent;
    string defaultName;

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);
        Bind<GameObject>(typeof(Contents));

        GetObject(((int)Contents.Yes)).gameObject.AddUIEvent((data) => SayYes());

        GetObject(((int)Contents.NoTouch)).gameObject.AddUIEvent((data) => SayNo(), Define.UIEvent.RightClick);
        GetObject(((int)Contents.Panel)).gameObject.AddUIEvent((data) => SayNo(), Define.UIEvent.RightClick);
        GetObject(((int)Contents.No)).gameObject.AddUIEvent((data) => SayNo());

        GetObject(((int)Contents.Content)).GetComponent<TextMeshProUGUI>().text = textContent;

        field = GetObject(((int)Contents.InputField)).GetComponent<TMP_InputField>();
        placeholder = GetObject(((int)Contents.InputField)).GetComponent<TMP_InputField>().placeholder as TextMeshProUGUI;


        field.text = defaultName;
        placeholder.text = defaultName;
    }


    public void SetAction(Action<string> action, string _fieldName, string _defaultValue)
    {
        yesAction = action;
        textContent = _fieldName;
        defaultName = _defaultValue;
    }

    Action<string> yesAction;

    TMP_InputField field;
    TextMeshProUGUI placeholder;

    void SayYes()
    {
        // 인풋필드의 메세지를 받아와서 리턴해주면 댐. 근데 어디로?

        yesAction.Invoke(field.text);
        ClosePopUp();
    }
    void SayNo()
    {
        ClosePopUp();
    }









    #region Default

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

    #endregion
}
