using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Confirm : UI_PopUp
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


    //public UI_Base Parent { get; set; }
    string textContent;

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);
        CloseFloorUI();


        Bind<GameObject>(typeof(Contents));

        GetObject(((int)Contents.NoTouch)).gameObject.AddUIEvent((data) => SayNo(), Define.UIEvent.RightClick);
        GetObject(((int)Contents.Panel)).gameObject.AddUIEvent((data) => SayNo(), Define.UIEvent.RightClick);

        GetObject(((int)Contents.Yes)).gameObject.AddUIEvent((data) => SayYes());
        GetObject(((int)Contents.No)).gameObject.AddUIEvent((data) => SayNo());

        GetObject(((int)Contents.Content)).GetComponent<TextMeshProUGUI>().text = textContent;
    }

    public enum State
    {
        Wait,
        Yes,
        No,
    }
    State state = State.Wait;



    public State GetAnswer()
    {
        return state;
    }

    public void SetText(string _content)
    {
        textContent = _content;
    }


    void SayYes()
    {
        state = State.Yes;
        //ClosePopUp();
        StartCoroutine(WaitAndClose());
    }

    void SayNo()
    {
        state = State.No;
        //ClosePopUp();
        StartCoroutine(WaitAndClose());
    }

    IEnumerator WaitAndClose()
    {
        //yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        ClosePopUp();
    }

    void CloseFloorUI()
    {
        var typeUI = FindAnyObjectByType<UI_Placement_TypeSelect>();
        if (typeUI)
        {
            Managers.UI.ClosePopupPick(typeUI);
            FindObjectOfType<UI_Management>().FloorPanelClear();
        }
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

///////////////////////////////////////////////////////////////////////
/*                       복붙용
 ////////////////////////////////////////////////////////
 /*
 *  
    void Demolition_Technical()
    {
        var ui = Managers.UI.ShowPopUp<UI_Confirm>();
        ui.SetText($"{Parent.Current.Data.name_Placement}(을)를 철거할까요?");
        StartCoroutine(WaitForAnswer(ui));
    }

    IEnumerator WaitForAnswer(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            Managers.Technical.RemoveTechnical(Parent.Current);
        }
        //else if (confirm.GetAnswer() == UI_Confirm.State.No)
        //{

        //}
    }
 * 
 * 
 * */////////////////////////////////////////////////////////////////